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

        public delegate bool TryParseDelegate(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior);

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

            if(TryParse(enumerator, out var parsedNode, AssignmentOperatorBehavior.Assign) == false || parsedNode == default)
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

        public bool TryParse(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior, int currentPrecedence = 0)
        {
            if(currentPrecedence == 0 && CustomParseDelegates != default)
            {
                var customResult = CustomParseDelegates.Select(del =>
                {
                    var Success = del(enumerator, out var Result, assignmentOperatorBehavior);
                    return new { Success, Result };
                }).FirstOrDefault(res => res.Success)?.Result;
                if(customResult != default)
                {
                    enumerator.MoveNext(); // TODO: Is this right?
                    parsedNode = customResult;
                    return true;
                }
            }

            if (currentPrecedence >= _PrecedenceGroups.Length)
            {
                return TryParseBraces(enumerator, out parsedNode, assignmentOperatorBehavior);
            }

            switch(_PrecedenceGroups[currentPrecedence].First().OperandCount)
            {
                case OperandCount.Binary:
                    return TryParseBinary(enumerator, currentPrecedence, out parsedNode, assignmentOperatorBehavior);
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
                    right.SetParent(parsedNode);
                    return true;
                }
            }
            return TryParse(enumerator, out parsedNode, AssignmentOperatorBehavior.Assign, currentPrecedence + 1);
        }

        private bool TryParseBinary(ILookaroundEnumerator<Token> enumerator, int currentPrecedence, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            parsedNode = null;
            var currentPrecedenceOperators = _PrecedenceGroups[currentPrecedence];
            var validOperatorTextValues = _ValidOperatorTextValues[currentPrecedence];

            // Check the next precedence level first...
            if (TryParse(enumerator, out var left, assignmentOperatorBehavior, currentPrecedence + 1) == false || left == default)
            {
                return false;
            }

            while(enumerator.Current.TokenType == TokenType.Operator && validOperatorTextValues.Contains(enumerator.Current.TextValue))
            {
                var operatorToken = enumerator.Current;
                var operatorDefinition = currentPrecedenceOperators.Where(op => op.Text == operatorToken.TextValue).First();


                if (enumerator.MoveNext() == false)
                {
                    throw new NotImplementedException();
                }

                ASTNode? right;
                if (operatorDefinition is SpecialOperatorDefinition special && special.MaximumArguments != 0)
                {
                    if(TryParseArgumentSet(enumerator, out right, special) == false || right == null)
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    if (TryParse(enumerator, out right, assignmentOperatorBehavior, currentPrecedence + 1) == false || right == null)
                    {
                        throw new NotImplementedException();
                    }
                }

                var @operator = CreateBinaryOperator(enumerator, operatorToken, operatorDefinition, assignmentOperatorBehavior);
                var newNode = new BinaryASTNode(left, @operator, right);
                left.SetParent(newNode);
                right.SetParent(newNode);
                left = newNode;
            }
            parsedNode = left;
            return true;
        }

        private Operator CreateBinaryOperator(ILookaroundEnumerator<Token> enumerator, Token operatorToken, OperatorDefinition operatorDefinition, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            return operatorDefinition switch
            {
                SpecialOperatorDefinition specialOperator => Operator.CreateSpecial(specialOperator, operatorToken),
                _ => Operator.CreateBinary(operatorDefinition, operatorToken, assignmentOperatorBehavior),
            };
        }

        private bool TryParseArgumentSet(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, SpecialOperatorDefinition operatorDefinition)
        {
            var argSeperator = operatorDefinition.ArgumentSeperator ?? throw new NotImplementedException();
            var endingToken = operatorDefinition.EndingToken ?? throw new NotImplementedException();
            var arguments = new Queue<ASTNode>();

            if(operatorDefinition.MaximumArguments > 0)
            {
                while (TryParse(enumerator, out var argItem, assignmentOperatorBehavior: AssignmentOperatorBehavior.NamedParameter))
                {
                    arguments.Enqueue(argItem);
                    if (enumerator.Current.TokenType != argSeperator) break;
                    enumerator.MoveNext();
                }
            }
            if (enumerator.Current.TokenType != endingToken) throw new NotImplementedException();
            parsedNode = new ArgumentSetNode(arguments);
            return true;
        }


        private bool TryParseBraces(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            parsedNode = default;
            if (enumerator.Current.TokenType.IsOpenBrace() == false && enumerator.Current.SecondaryTokenType?.IsOpenBrace() != true)
            {
                if(TryParseTerminal(enumerator, out parsedNode, assignmentOperatorBehavior))
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

            if(TryParse(enumerator, out parsedNode, assignmentOperatorBehavior) == false || parsedNode == default)
            {
                throw new NotImplementedException();
            }

            if(enumerator.State != EnumeratorState.Active)
            {
                //throw new NotImplementedException(); // Did not find closing brace
            }
            if (enumerator.Current.TokenType.IsMatchingBrace(braceToken.TokenType) == false && enumerator.Current.TokenType.IsMatchingBrace(braceToken.SecondaryTokenType) != true)
            {
                enumerator.TryGetNextArray(5, out var nextTokens);
                throw new NotImplementedException(); // Did not find the right closing brace
            }
            enumerator.MoveNext();  // Consume the closing brace
            return true;
        }



        private bool TryParseTerminal(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior)
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
                var Success = del(enumerator, out var Result, assignmentOperatorBehavior);
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
