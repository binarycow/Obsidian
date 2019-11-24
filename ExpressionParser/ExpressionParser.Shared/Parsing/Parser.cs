using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using ExpressionParser.Configuration;
using ExpressionParser.Exceptions;
using ExpressionParser.Lexing;
using ExpressionParser.Operators;

namespace ExpressionParser.Parsing
{
    public class Parser
    {
        public Parser(ILanguageDefinition languageDefinition, byte minimumLookahead = 1, byte minimumLookbehind = 1)
        {
            LanguageDefinition = languageDefinition;
            _PrecedenceGroups = LanguageDefinition.Operators
                .Where(op => !(op is MethodOperatorDefinition))
                .GroupBy(op => op.Precedence)
                .OrderBy(grp => grp.Key)
                .ToArray();
            _ValidOperatorTextValues = _PrecedenceGroups.Select(group => group.Select(op => op.Text).ToArray()).ToArray();
            _LiteralParseDelegates = Array.Empty<TryParseDelegate>();
            MinimumLookahead = Math.Max(minimumLookahead, (byte)1);
            MinimumLookbehind = Math.Max(minimumLookbehind, (byte)1);
        }

        public byte MinimumLookahead { get; }
        public byte MinimumLookbehind { get; }

        public ILanguageDefinition LanguageDefinition { get; }

        public delegate bool TryParseDelegate(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode);

        private TryParseDelegate[] _CustomParseDelegates = Array.Empty<TryParseDelegate>();
        public virtual TryParseDelegate[] CustomParseDelegates => _CustomParseDelegates;

        private TryParseDelegate[] _LiteralParseDelegates = Array.Empty<TryParseDelegate>();
        public virtual TryParseDelegate[] LiteralParseDelegates => _LiteralParseDelegates;

        private IGrouping<int, OperatorDefinition>[] _PrecedenceGroups;
        private string[][] _ValidOperatorTextValues;

        public ASTNode Parse(IEnumerable<Token> tokens)
        {
            var tokensExcludingWhiteSpace = tokens.Where(token => token.TokenType != TokenType.WhiteSpace);
            using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(tokensExcludingWhiteSpace, 1, 1);

            if(enumerator.MoveNext() == false)
            {
                return LiteralNode.CreateNull();
            }

            if(TryParse(enumerator, out var parsedNode) == false || parsedNode == default)
            {
                throw new NotImplementedException(); // Couldn't parse data!
            }
            enumerator.MoveNext();
            if (enumerator.State != EnumeratorState.Complete)
            {
                throw new NotImplementedException(); // We have unparsed data - why!?
            }
            return parsedNode;
        }

        public bool TryParse(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, int currentPrecedence = 0)
        {
            if(currentPrecedence == 0 && CustomParseDelegates != default)
            {
                var customResult = CustomParseDelegates.Select(del =>
                {
                    var Success = del(enumerator, out var Result);
                    return new { Success, Result };
                }).FirstOrDefault(res => res.Success)?.Result;
                if(customResult != default)
                {
                    parsedNode = customResult;
                    return true;
                }
            }

            if (currentPrecedence >= _PrecedenceGroups.Length)
            {
                return TryParseBraces(enumerator, out parsedNode);
            }

            switch(_PrecedenceGroups[currentPrecedence].First().OperandCount)
            {
                case OperandCount.Binary:
                    return TryParseBinary(enumerator, currentPrecedence, out parsedNode);
                case OperandCount.Unary:
                    return TryParseUnary(enumerator, currentPrecedence, out parsedNode);
                default:
                    throw new NotImplementedException();
            }
        }

        private bool TryParseUnary(ILookaroundEnumerator<Token> enumerator, int currentPrecedence, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            var currentPrecedenceOperators = _PrecedenceGroups[currentPrecedence];
            var validOperatorTextValues = _ValidOperatorTextValues[currentPrecedence];

            while (enumerator.Current.TokenType == TokenType.Operator && validOperatorTextValues.Contains(enumerator.Current.TextValue))
            {
                var operatorToken = enumerator.Current;
                if (enumerator.MoveNext() == false)
                {
                    throw new NotImplementedException();
                }
                var @operator = Operator.CreateUnary(currentPrecedenceOperators.Where(op => op.Text == operatorToken.TextValue).First(), operatorToken);
                if(TryParseUnary(enumerator, currentPrecedence, out var right))
                {
                    parsedNode = new UnaryASTNode(@operator, right);
                    return true;
                }
            }
            return TryParse(enumerator, out parsedNode, currentPrecedence + 1);
        }

        private bool TryParseBinary(ILookaroundEnumerator<Token> enumerator, int currentPrecedence, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = null;
            var currentPrecedenceOperators = _PrecedenceGroups[currentPrecedence];
            var validOperatorTextValues = _ValidOperatorTextValues[currentPrecedence];

            // Check the next precedence level first...
            if (TryParse(enumerator, out var left, currentPrecedence + 1) == false || left == default)
            {
                return false;
            }

            while(enumerator.Current.TokenType == TokenType.Operator && validOperatorTextValues.Contains(enumerator.Current.TextValue))
            {
                var operatorToken = enumerator.Current;
                if(enumerator.MoveNext() == false)
                {
                    throw new NotImplementedException();
                }
                if (TryParse(enumerator, out var right, currentPrecedence + 1) == false || right == default)
                {
                    throw new NotImplementedException();
                }
                var operatorDefinition = currentPrecedenceOperators.Where(op => op.Text == operatorToken.TextValue).First();
                var @operator = CreateBinaryOperator(enumerator, operatorToken, operatorDefinition);
                left = new BinaryASTNode(left, @operator, right);
            }
            parsedNode = left;
            return true;
        }

        private Operator CreateBinaryOperator(ILookaroundEnumerator<Token> enumerator, Token operatorToken, OperatorDefinition operatorDefinition)
        {
            if (operatorDefinition is SpecialOperatorDefinition specialOperator && specialOperator.OperatorType == SpecialOperatorType.MemberAccess)
            {
                var subType = SpecialOperatorSubType.Property;
                if (enumerator.TryGetNext(out var nextToken))
                {
                    switch (nextToken.TokenType)
                    {
                        // TODO: Don't hardcode this - specify with operator definition
                        case TokenType.Paren_Open:
                            subType = SpecialOperatorSubType.MethodCall;
                            break;
                        case TokenType.SquareBrace_Open:
                            subType = SpecialOperatorSubType.Index;
                            break;
                    }
                }
                return Operator.CreateMemberAccess(specialOperator, operatorToken, subType);
            }
            return Operator.CreateBinary(operatorDefinition, operatorToken);
        }

        private bool TryParseBraces(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;
            if (enumerator.Current.TokenType.IsOpenBrace() == false)
            {
                if(TryParseTerminal(enumerator, out parsedNode))
                {
                    enumerator.MoveNext();
                    return true;
                }
                return false;
            }
            if(enumerator.TryGetPrevious(out var previousToken) && previousToken.TokenType != TokenType.Operator)
            {
                // This isn't a grouping parentheses - it's a function call.  We should have caught it by now.
                throw new NotImplementedException();
            }
            var braceToken = enumerator.Current;
            if (enumerator.MoveNext() == false) throw new NotImplementedException();

            if(TryParse(enumerator, out parsedNode) == false || parsedNode == default)
            {
                throw new NotImplementedException();
            }

            if(enumerator.State != EnumeratorState.Active)
            {
                throw new NotImplementedException(); // Did not find closing brace
            }
            if (enumerator.Current.TokenType.IsMatchingBrace(braceToken.TokenType) == false)
            {
                throw new NotImplementedException(); // Did not find the right closing brace
            }
            enumerator.MoveNext();  // Consume the closing brace
            return true;
        }



        private bool TryParseTerminal(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;

            switch(enumerator.Current.TokenType)
            {
                case TokenType.StringLiteral:
                    parsedNode = LiteralNode.CreateStringLiteral(enumerator.Current);
                    break;
                case TokenType.CharacterLiteral:
                    parsedNode = LiteralNode.CreateCharacterLiteral(enumerator.Current);
                    break;
                case TokenType.FloatingLiteral:
                    parsedNode = LiteralNode.CreateFloatLiteral(enumerator.Current);
                    break;
                case TokenType.IntegerLiteral:
                    parsedNode = LiteralNode.CreateIntegerLiteral(enumerator.Current);
                    break;
                case TokenType.Identifier:
                    parsedNode = new IdentifierNode(enumerator.Current);
                    break;
            }
            if(parsedNode != null)
            {
                return true;
            }

            var readNode = LiteralParseDelegates.Select(del =>
            {
                var Success = del(enumerator, out var Result);
                return new { Success, Result };
            }).FirstOrDefault(res => res.Success)?.Result;
            if(readNode != null)
            {
                parsedNode = readNode;
                return true;
            }
            return false;
        }
    }
}
