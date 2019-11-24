using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.Statements
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class IfNode : StatementNode
    {
        public IfNode(IEnumerable<ConditionalNode> conditions, WhiteSpaceControlMode startWhiteSpace, WhiteSpaceControlMode endWhiteSpace)
            : base(conditions, startWhiteSpace, endWhiteSpace)
        {
            Conditions = conditions.ToArrayWithoutInstantiation();
        }

        public ConditionalNode[] Conditions { get; }
        private string DebuggerDisplay => $"{nameof(IfNode)}";

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
        public static bool TryParseIf(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            WhiteSpaceControlMode previousConditionBlockEndWhiteSpace;
            WhiteSpaceControlMode currentConditionBlockStartWhiteSpace;

            var conditions = new Queue<ConditionalNode>();
            parsedNode = default;
            if (IfNodeParser.TryParseConditionBlock(enumerator.Current, TokenTypes.Keyword_If, out var outerBlockStartWhiteSpace,
                out var previousConditionBlockStartWhiteSpace, out var previousBlockExpression) == false)
            {
                return false;
            }
            enumerator.MoveNext();
            var blockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();

            while (IfNodeParser.TryParseConditionBlock(enumerator.Current, TokenTypes.Keyword_Elif, out previousConditionBlockEndWhiteSpace,
                out currentConditionBlockStartWhiteSpace, out var currentBlockExpression))
            {
                conditions.Enqueue(new ConditionalNode(ExpressionNode.FromString(previousBlockExpression), blockChildren, previousConditionBlockStartWhiteSpace, previousConditionBlockEndWhiteSpace));
                previousConditionBlockStartWhiteSpace = currentConditionBlockStartWhiteSpace;
                previousBlockExpression = currentBlockExpression;
                enumerator.MoveNext();
                blockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();
            }

            if (IfNodeParser.TryParseElseOrEndBlock(enumerator.Current, TokenTypes.Keyword_Else, out previousConditionBlockEndWhiteSpace, out currentConditionBlockStartWhiteSpace))
            {
                conditions.Enqueue(new ConditionalNode(ExpressionNode.FromString(previousBlockExpression), blockChildren, previousConditionBlockStartWhiteSpace, previousConditionBlockEndWhiteSpace));
                previousConditionBlockStartWhiteSpace = currentConditionBlockStartWhiteSpace;
                previousBlockExpression = JinjaEnvironment.TRUE;
                enumerator.MoveNext();
                blockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();
            }

            if (IfNodeParser.TryParseElseOrEndBlock(enumerator.Current, TokenTypes.Keyword_Endif, out previousConditionBlockEndWhiteSpace, out var outerBlockEndWhiteSpace) == false)
            {
                throw new NotImplementedException();
            }
            conditions.Enqueue(new ConditionalNode(ExpressionNode.FromString(previousBlockExpression), blockChildren, previousConditionBlockStartWhiteSpace, previousConditionBlockEndWhiteSpace));

            parsedNode = new IfNode(conditions, outerBlockStartWhiteSpace, outerBlockEndWhiteSpace);
            return true;
        }


        private static class IfNodeParser
        {
            private enum States
            {
                StartJinja,
                WhiteSpaceOrKeyword,
                Keyword,
                Expression,
                EndJinja,
                Done,
                WhiteSpaceOrEndJinja,
            }
            public static bool TryParseConditionBlock(ParsingNode startingBlock, TokenTypes keywordType,
                out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace,
                out string expression)
            {
                var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(startingBlock.Tokens, 1);
                return TryParseConditionBlock(enumerator, keywordType, out startWhiteSpace, out endWhiteSpace, out expression);
            }
            public static bool TryParseConditionBlock(ILookaroundEnumerator<Token> enumerator, TokenTypes keywordType,
                out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace,
                out string expression)
            {
                expression = string.Empty;
                var expressionQueue = new Queue<Token>();
                startWhiteSpace = WhiteSpaceControlMode.Default;
                endWhiteSpace = WhiteSpaceControlMode.Default;
                var state = States.StartJinja;
                while (enumerator.MoveNext())
                {
                    var token = enumerator.Current;
                    switch (state)
                    {
                        case States.StartJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.StatementStart:
                                    state = States.WhiteSpaceOrKeyword;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.WhiteSpaceOrKeyword:
                            switch (token.TokenType)
                            {
                                case TokenTypes.WhiteSpace:
                                    state = States.Keyword;
                                    continue;
                                case TokenTypes.Minus:
                                case TokenTypes.Plus:
                                    startWhiteSpace = token.TokenType == TokenTypes.Minus ? WhiteSpaceControlMode.Trim : WhiteSpaceControlMode.Keep;
                                    state = States.Keyword;
                                    continue;
                                case TokenTypes.Keyword_If:
                                    state = States.Expression;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.Keyword:
                            if (token.TokenType == TokenTypes.WhiteSpace)
                            {
                                continue;
                            }
                            if (token.TokenType == keywordType)
                            {
                                state = States.Expression;
                                continue;
                            }
                            return false;
                        case States.Expression:
                            switch (token.TokenType)
                            {
                                case TokenTypes.Minus:
                                    if (enumerator.TryGetNext(out var nextItem) && nextItem.TokenType == TokenTypes.StatementEnd)
                                    {
                                        endWhiteSpace = WhiteSpaceControlMode.Trim;
                                        state = States.EndJinja;
                                        continue;
                                    }
                                    expressionQueue.Enqueue(token);
                                    continue;
                                case TokenTypes.StatementEnd:
                                    state = States.Done;
                                    continue;
                                default:
                                    expressionQueue.Enqueue(token);
                                    continue;
                            }
                        case States.EndJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.StatementEnd:
                                    state = States.Done;
                                    continue;
                                default:
                                    throw new NotImplementedException();
                            }
                        case States.Done:
                            throw new NotImplementedException();
                        default:
                            throw new NotImplementedException();
                    }
                }
                if (state != States.Done)
                {
                    throw new NotImplementedException();
                }
                expression = string.Join(string.Empty, expressionQueue.Select(token => token.Value)).Trim();
                return true;
            }

            public static bool TryParseElseOrEndBlock(ParsingNode startingBlock, TokenTypes keywordType, out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace)
            {
                var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(startingBlock.Tokens, 1);
                return TryParseElseOrEndBlock(enumerator, keywordType, out startWhiteSpace, out endWhiteSpace);
            }
            public static bool TryParseElseOrEndBlock(ILookaroundEnumerator<Token> enumerator, TokenTypes keywordType, out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace)
            {
                startWhiteSpace = WhiteSpaceControlMode.Default;
                endWhiteSpace = WhiteSpaceControlMode.Default;
                var state = States.StartJinja;
                while (enumerator.MoveNext())
                {
                    var token = enumerator.Current;
                    switch (state)
                    {
                        case States.StartJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.StatementStart:
                                    state = States.WhiteSpaceOrKeyword;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.WhiteSpaceOrKeyword:
                            switch (token.TokenType)
                            {
                                case TokenTypes.WhiteSpace:
                                    state = States.Keyword;
                                    continue;
                                case TokenTypes.Minus:
                                case TokenTypes.Plus:
                                    startWhiteSpace = token.TokenType == TokenTypes.Minus ? WhiteSpaceControlMode.Trim : WhiteSpaceControlMode.Keep;
                                    state = States.Keyword;
                                    continue;
                                case TokenTypes.Keyword_Else:
                                    state = States.WhiteSpaceOrEndJinja;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.Keyword:
                            if (token.TokenType == TokenTypes.WhiteSpace)
                            {
                                continue;
                            }
                            if (token.TokenType == keywordType)
                            {
                                state = States.WhiteSpaceOrEndJinja;
                                continue;
                            }
                            return false;
                        case States.WhiteSpaceOrEndJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.WhiteSpace:
                                    continue;
                                case TokenTypes.Minus:
                                    if (enumerator.TryGetNext(out var nextItem) && nextItem.TokenType == TokenTypes.StatementEnd)
                                    {
                                        endWhiteSpace = WhiteSpaceControlMode.Trim;
                                        state = States.EndJinja;
                                        continue;
                                    }
                                    throw new NotImplementedException();
                                case TokenTypes.StatementEnd:
                                    state = States.Done;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.EndJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.StatementEnd:
                                    state = States.Done;
                                    continue;
                                default:
                                    throw new NotImplementedException();
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }
                if (state != States.Done)
                {
                    throw new NotImplementedException();
                }
                return true;
            }
        }
    }
}
