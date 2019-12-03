using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.Lexing;
using Obsidian.Parsing;

namespace Obsidian.AST
{
    public class ASTGenerator
    {
        public static TemplateNode ParseTemplate(Lexer lexer, IEnumerable<ParsingNode> source)
        {
            var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(source, 10);

            while (enumerator.MoveNext())
            {
                var nodes = ParseUntilFailure(lexer, enumerator).ToArray();
                if (enumerator.TryGetNext(out var nextNode))
                {
                    throw new NotImplementedException();
                }
                return new TemplateNode(nodes);
            }

            throw new NotImplementedException();
        }
        public static IEnumerable<ASTNode> ParseUntilFailure(Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator)
        {
            do
            {
                ASTNode? astNode = default;
                switch (enumerator.Current.NodeType)
                {
                    case ParsingNodeType.Statement:
                        StatementNode.TryParse(lexer, enumerator, out astNode);
                        break;
                    case ParsingNodeType.NewLine:
                        astNode = new NewLineNode(enumerator.Current);
                        break;
                    case ParsingNodeType.Comment:
                        astNode = new CommentNode(enumerator.Current);
                        break;
                    case ParsingNodeType.WhiteSpace:
                        astNode = WhiteSpaceNode.Parse(enumerator);
                        break;
                    case ParsingNodeType.Expression:
                        if (ExpressionNode.TryParse(enumerator, out astNode) == false)
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    case ParsingNodeType.Output:
                        astNode = new OutputNode(enumerator.Current);
                        break;
                }
                if (astNode == default)
                {
                    yield break;
                }
                yield return astNode;
            } while (enumerator.MoveNext());
        }
    }

}
