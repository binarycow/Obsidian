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

namespace Obsidian.AST.Nodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class ExpressionNode : ASTNode
    {
        public ExpressionNode(ParsingNode parsingNode, string expression) : base(parsingNode)
        {
            Expression = expression;
        }

        private ExpressionNode(string expression) : base(new ParsingNode(ParsingNodeType.Expression, new[] { new Token(TokenType.Unknown, expression) }))
        {
            Expression = expression;
            Output = false;
        }

        public string Expression { get; }
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
        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
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
                using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(node.Tokens, 1);
                var state = States.StartJinja;
                var expressionQueue = new Queue<Token>();
                while (enumerator.MoveNext())
                {
                    var token = enumerator.Current;
                    switch (state)
                    {
                        case States.StartJinja:
                            switch (token.TokenType)
                            {
                                case TokenType.ExpressionStart:
                                    state = States.WhiteSpaceOrExpression;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.WhiteSpaceOrExpression:
                            switch (token.TokenType)
                            {
                                case TokenType.Minus:
                                    throw new NotImplementedException();
#pragma warning disable CS0162 // Unreachable code detected
                                    continue;
#pragma warning restore CS0162 // Unreachable code detected
                                default:
                                    state = States.Expression;
                                    expressionQueue.Enqueue(token);
                                    continue;
                            }
                        case States.Expression:
                            switch (token.TokenType)
                            {
                                case TokenType.Minus:
                                    if (enumerator.TryGetNext(out var nextToken) && nextToken.TokenType == TokenType.ExpressionEnd)
                                    {
                                        throw new NotImplementedException();
#pragma warning disable CS0162 // Unreachable code detected
                                        state = States.EndJinja;
                                        continue;
#pragma warning restore CS0162 // Unreachable code detected
                                    }
                                    expressionQueue.Enqueue(token);
                                    continue;
                                case TokenType.ExpressionEnd:
                                    state = States.Done;
                                    continue;
                                default:
                                    expressionQueue.Enqueue(token);
                                    continue;
                            }
                        case States.EndJinja:
                            switch (token.TokenType)
                            {
                                case TokenType.ExpressionEnd:
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
                parsedNode = new ExpressionNode(node, expression);
                return true;
            }
        }
    }
}
