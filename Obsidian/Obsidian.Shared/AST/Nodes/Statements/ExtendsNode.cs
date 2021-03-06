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

namespace Obsidian.AST.Nodes.MiscNodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class ExtendsNode : ASTNode
    {
        internal ExtendsNode(string templateName, ExpressionNode template, ParsingNode parsingNode) : base(parsingNode)
        {
            TemplateName = templateName;
            Template = template;
        }

        internal string TemplateName { get; }
        internal ExpressionNode Template { get; }

        private string DebuggerDisplay => $"{nameof(ExtendsNode)} : {TemplateName}";

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false)
        {
            visitor.Transform(this, inner);
        }
        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }
        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        internal static bool TryParseExtends(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            _ = ExtendsParser.TryParse(environment, enumerator.Current, out parsedNode);
            return parsedNode != default;
        }
        private static class ExtendsParser
        {
            internal enum States
            {
                StartJinja,
                Keyword,
                Template,
                Done,
            }

            internal static bool TryParse(JinjaEnvironment environment, ParsingNode currentNode, [NotNullWhen(true)]out ASTNode? parsedNode)
            {
                using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(currentNode.Tokens);
                parsedNode = default;
                var state = States.StartJinja;
                var templateQueue = new Queue<Token>();
                while(enumerator.MoveNext())
                {
                    var token = enumerator.Current;
                    switch(state)
                    {
                        case States.StartJinja:
                            switch(token.TokenType)
                            {
                                case TokenType.StatementStart:
                                    state = States.Keyword;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.Keyword:
                            switch(token.TokenType)
                            {
                                case TokenType.WhiteSpace:
                                    continue;
                                case TokenType.Keyword_Extends:
                                    state = States.Template;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.Template:
                            switch(token.TokenType)
                            {
                                case TokenType.StatementEnd:
                                    state = States.Done;
                                    continue;
                                default:
                                    templateQueue.Enqueue(token);
                                    continue;
                            }
                        case States.Done:
                            throw new NotImplementedException();
                        default:
                            throw new NotImplementedException();
                    }
                }
                var templateName = string.Join(string.Empty, templateQueue.Select(token => token.Value)).Trim();
                parsedNode = new ExtendsNode(templateName, ExpressionNode.FromString(environment, templateName), currentNode);
                return true;
            }
        }
    }
}
