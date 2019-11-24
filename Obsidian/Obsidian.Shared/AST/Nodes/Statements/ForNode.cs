using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.Statements
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ForNode : StatementNode
    {
        public ForNode(ContainerNode primaryBlock, ContainerNode? elseBlock, string[] variableNames, ExpressionNode expression, WhiteSpaceControlMode startWhiteSpace, WhiteSpaceControlMode endWhiteSpace)
            : base(new[] { primaryBlock }.Concat(elseBlock != null ? Enumerable.Repeat(elseBlock, 1) : Enumerable.Empty<ContainerNode>()), startWhiteSpace, endWhiteSpace)
        {
            PrimaryBlock = primaryBlock;
            ElseBlock = elseBlock;
            VariableNames = variableNames;
            Expression = expression;
        }

        public ContainerNode PrimaryBlock { get; }
        public ContainerNode? ElseBlock { get; }
        public string[] VariableNames { get; }
        public ExpressionNode Expression { get; }

        private string DebuggerDisplay => $"{nameof(ForNode)} : Variables: \"{string.Join(", ", VariableNames)}\" Expression: \"{Expression}\"";

        public static bool TryParseFor(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            ContainerNode? elseBlock = null;
            WhiteSpaceControlMode primaryBlockEndWhiteSpace;
            WhiteSpaceControlMode outerBlockEndWhiteSpace;
            parsedNode = default;
            if (ForNodeParser.TryParseStartingBlock(enumerator.Current, out var outerBlockStartWhiteSpace, out var primaryBlockStartWhiteSpace, out var variableNames, out var expression, out var isLineMode) == false)
            {
                return false;
            }
            enumerator.MoveNext();
            var primaryBlockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();

            ContainerNode primaryBlock;
            if (ForNodeParser.TryParseElseOrEndingBlock(enumerator.Current, TokenTypes.Keyword_Else, out primaryBlockEndWhiteSpace, out var elseBlockStartWhiteSpace, isLineMode))
            {
                enumerator.MoveNext();
                primaryBlock = new ContainerNode(primaryBlockChildren, primaryBlockStartWhiteSpace, primaryBlockEndWhiteSpace);
                var elseBlockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();
                if (ForNodeParser.TryParseElseOrEndingBlock(enumerator.Current, TokenTypes.Keyword_EndFor, out var elseBlockEndWhiteSpace, out outerBlockEndWhiteSpace, isLineMode) == false)
                {
                    throw new NotImplementedException();
                }
                elseBlock = new ContainerNode(elseBlockChildren, elseBlockStartWhiteSpace, elseBlockEndWhiteSpace);
                parsedNode = new ForNode(primaryBlock, elseBlock, variableNames, ExpressionNode.FromString(expression), outerBlockStartWhiteSpace, outerBlockEndWhiteSpace);
                return true;
            }
            else
            {
                if (ForNodeParser.TryParseElseOrEndingBlock(enumerator.Current, TokenTypes.Keyword_EndFor, out primaryBlockEndWhiteSpace, out outerBlockEndWhiteSpace, isLineMode) == false)
                {
                    throw new NotImplementedException();
                }
                primaryBlock = new ContainerNode(primaryBlockChildren, primaryBlockStartWhiteSpace, primaryBlockEndWhiteSpace);
                parsedNode = new ForNode(primaryBlock, elseBlock, variableNames, ExpressionNode.FromString(expression), outerBlockStartWhiteSpace, outerBlockEndWhiteSpace);
                return true;
            }
        }


        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        private static class ForNodeParser
        {
            private enum States
            {
                StartJinja,
                WhiteSpaceOrKeyword,
                Keyword,
                Variables,
                Expression,
                EndJinja,
                Done,
                WhiteSpaceOrEndJinja,
            }


            public static bool TryParseElseOrEndingBlock(ParsingNode endingBlock, TokenTypes tokenType, out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace, bool isLineMode)
            {
                var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(endingBlock.Tokens, 1);
                return TryParseEndingBlock(enumerator, tokenType, out startWhiteSpace, out endWhiteSpace, isLineMode);
            }

            public static bool TryParseEndingBlock(ILookaroundEnumerator<Token> enumerator, TokenTypes tokenType, out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace, bool isLineMode)
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
                                case TokenTypes.LineStatement:
                                    if (isLineMode)
                                    {
                                        state = States.Keyword; // TODO: Can line statements do manual trimming?
                                        continue;
                                    }
                                    return false;
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
                                default:
                                    if(token.TokenType == tokenType)
                                    {
                                        state = States.WhiteSpaceOrEndJinja;
                                        continue;
                                    }
                                    return false;
                            }
                        case States.Keyword:
                            switch (token.TokenType)
                            {
                                case TokenTypes.WhiteSpace:
                                    continue;
                                default:
                                    if (token.TokenType == tokenType)
                                    {
                                        state = States.WhiteSpaceOrEndJinja;
                                        continue;
                                    }
                                    return false;
                            }
                        case States.WhiteSpaceOrEndJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.WhiteSpace:
                                    continue;
                                case TokenTypes.Minus:
                                    if(enumerator.TryGetNext(out var nextItem) && nextItem.TokenType == TokenTypes.StatementEnd)
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
                if (isLineMode == false && state != States.Done)
                {
                    throw new NotImplementedException();
                }
                return true;
            }

            public static bool TryParseStartingBlock(ParsingNode startingBlock, out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace, out string[] variableNames, out string expression, out bool isLineMode)
            {
                var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(startingBlock.Tokens, 1);
                return TryParseStartingBlock(enumerator, out startWhiteSpace, out endWhiteSpace, out variableNames, out expression, out isLineMode);
            }
            public static bool TryParseStartingBlock(ILookaroundEnumerator<Token> enumerator, out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace, out string[] variableNames, out string expression, out bool isLineMode)
            {
                isLineMode = false;
                variableNames = Array.Empty<string>();
                expression = string.Empty;
                startWhiteSpace = WhiteSpaceControlMode.Default;
                endWhiteSpace = WhiteSpaceControlMode.Default;
                var variables = new Queue<Queue<Token>>();
                variables.Enqueue(new Queue<Token>());
                var expressionQueue = new Queue<Token>();

                var state = States.StartJinja;
                while (enumerator.MoveNext())
                {
                    var token = enumerator.Current;
                    switch (state)
                    {
                        case States.StartJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.LineStatement:
                                    isLineMode = true;
                                    state = States.Keyword;  // TODO: Can line statements do manual trimming?
                                    continue;
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
                                case TokenTypes.Keyword_For:
                                    state = States.Variables;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.Keyword:
                            switch (token.TokenType)
                            {
                                case TokenTypes.WhiteSpace:
                                    continue;
                                case TokenTypes.Keyword_For:
                                    state = States.Variables;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.Variables:
                            switch (token.TokenType)
                            {
                                case TokenTypes.Comma:
                                    variables.Enqueue(new Queue<Token>());
                                    continue;
                                case TokenTypes.Keyword_In:
                                    state = States.Expression;
                                    continue;
                                default:
                                    variables.Peek().Enqueue(token);
                                    continue;
                            }
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
                                case TokenTypes.Colon:
                                    if (isLineMode)
                                    {
                                        state = States.Done;
                                        continue;
                                    }
                                    expressionQueue.Enqueue(token);
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
                if (isLineMode == false && state != States.Done)
                {
                    throw new NotImplementedException();
                }
                variableNames = variables.Select(queue => string.Join(string.Empty, queue.Select(token => token.Value)).Trim()).ToArray();
                expression = string.Join(string.Empty, expressionQueue.Select(token => token.Value)).Trim();
                return true;
            }
        }
    }
}
