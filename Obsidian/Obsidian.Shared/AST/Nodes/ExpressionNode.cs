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

namespace Obsidian.AST.Nodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ExpressionNode : ASTNode
    {
        public ExpressionNode(ParsingNode parsingNode, string expression, WhiteSpaceControlMode startWhiteSpace, WhiteSpaceControlMode endWhiteSpace) : base(parsingNode)
        {
            StartWhiteSpace = startWhiteSpace;
            EndWhiteSpace = endWhiteSpace;
            Expression = expression;
        }

        private ExpressionNode(string expression) : base(new ParsingNode(ParsingNodeType.Expression, new[] { new Token(TokenTypes.Unknown, expression) }))
        {
            Expression = expression;
            Output = false;
        }

        public string Expression { get; }
        public WhiteSpaceControlMode StartWhiteSpace { get; }
        public WhiteSpaceControlMode EndWhiteSpace { get; }
        public bool Output { get; } = true;

        private string DebuggerDisplay => $"{nameof(ExpressionNode)} : \"{ToString(debug: true)}\"";

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }
        public static bool TryParse(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            return ExpressionNodeParser.TryParse(enumerator, out parsedNode);
        }

        public static ExpressionNode FromString(string expression)
        {
            return new ExpressionNode(expression);
        }

        private static class ExpressionNodeParser
        {
            private enum States
            {
                StartJinja,
                Expression,
                WhiteSpaceOrExpression,
                Done,
                EndJinja,
            }


            public static bool TryParse(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
            {
                return TryParse(enumerator.Current, out parsedNode);
            }
            public static bool TryParse(ParsingNode node, [NotNullWhen(true)]out ASTNode? parsedNode)
            {
                parsedNode = default;
                var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(node.Tokens, 1);
                var state = States.StartJinja;
                var startWhiteSpace = WhiteSpaceControlMode.Default;
                var endWhiteSpace = WhiteSpaceControlMode.Default;
                var expressionQueue = new Queue<Token>();
                while (enumerator.MoveNext())
                {
                    var token = enumerator.Current;
                    switch (state)
                    {
                        case States.StartJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.ExpressionStart:
                                    state = States.WhiteSpaceOrExpression;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.WhiteSpaceOrExpression:
                            switch (token.TokenType)
                            {
                                case TokenTypes.Minus:
                                    startWhiteSpace = WhiteSpaceControlMode.Trim;
                                    continue;
                                default:
                                    state = States.Expression;
                                    expressionQueue.Enqueue(token);
                                    continue;
                            }
                        case States.Expression:
                            switch (token.TokenType)
                            {
                                case TokenTypes.Minus:
                                    if (enumerator.TryGetNext(out var nextToken) && nextToken.TokenType == TokenTypes.ExpressionEnd)
                                    {
                                        endWhiteSpace = WhiteSpaceControlMode.Trim;
                                        state = States.EndJinja;
                                        continue;
                                    }
                                    expressionQueue.Enqueue(token);
                                    continue;
                                case TokenTypes.ExpressionEnd:
                                    state = States.Done;
                                    continue;
                                default:
                                    expressionQueue.Enqueue(token);
                                    continue;
                            }
                        case States.EndJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.ExpressionEnd:
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
                var expression = string.Join(string.Empty, expressionQueue.Select(token => token.Value)).Trim();
                parsedNode = new ExpressionNode(node, expression, startWhiteSpace, endWhiteSpace);
                return true;
            }
        }
    }
}
