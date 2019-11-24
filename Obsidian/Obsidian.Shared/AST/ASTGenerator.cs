using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.Parsing;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST
{
    public class ASTGenerator
    {
        public static ContainerNode ParseTemplate(IEnumerable<ParsingNode> source)
        {
            var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(source, 10);

            while (enumerator.MoveNext())
            {
                var nodes = ParseUntilFailure(enumerator).ToArray();
                if (enumerator.TryGetNext(out var nextNode))
                {
                    throw new NotImplementedException();
                }
                return new ContainerNode(nodes, WhiteSpaceControlMode.Default, WhiteSpaceControlMode.Default);
            }

            throw new NotImplementedException();
        }
        public static IEnumerable<ASTNode> ParseUntilFailure(ILookaroundEnumerator<ParsingNode> enumerator)
        {
            do
            {
                ASTNode? astNode = default;
                switch (enumerator.Current.NodeType)
                {
                    case ParsingNodeType.Statement:
                        StatementNode.TryParse(enumerator, out astNode);
                        break;
                    case ParsingNodeType.NewLine:
                        astNode = new NewLineNode(enumerator.Current, enumerator.Current.WhiteSpaceControlMode);
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
