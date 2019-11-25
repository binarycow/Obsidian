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
    public class ExtendsNode : ASTNode
    {
        public ExtendsNode(string templateName, ExpressionNode template, ParsingNode parsingNode) : base(parsingNode)
        {
            TemplateName = templateName;
            Template = template;
        }

        public string TemplateName { get; }
        public ExpressionNode Template { get; }

        private string DebuggerDisplay => $"{nameof(ExtendsNode)} : {TemplateName}";

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public static bool TryParseExtends(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            ExtendsParser.TryParse(enumerator.Current, out parsedNode);
            return parsedNode != default;
        }
        private static class ExtendsParser
        {
            public enum States
            {
                StartJinja,
                Keyword,
                Template,
                Done,
            }

            public static bool TryParse(ParsingNode currentNode, [NotNullWhen(true)]out ASTNode? parsedNode)
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
                                case TokenTypes.StatementStart:
                                    state = States.Keyword;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.Keyword:
                            switch(token.TokenType)
                            {
                                case TokenTypes.WhiteSpace:
                                    continue;
                                case TokenTypes.Keyword_Extends:
                                    state = States.Template;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.Template:
                            switch(token.TokenType)
                            {
                                case TokenTypes.StatementEnd:
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
                parsedNode = new ExtendsNode(templateName, ExpressionNode.FromString(templateName), currentNode);
                return true;
            }
        }
    }
}
