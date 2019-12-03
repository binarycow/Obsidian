using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Parsing;

namespace Obsidian.AST.Nodes
{
    public abstract class StatementNode : ASTNode, IWithChildren
    {
        public StatementNode(ParsingNode? startParsingNode, IEnumerable<ASTNode> children, ParsingNode? endParsingNode) 
            : base(startParsingNode, children.SelectMany(x => x.ParsingNodes), endParsingNode)
        {
            Children = children.ToArrayWithoutInstantiation();
        }

        public ASTNode[] Children { get; }

        public static bool TryParse(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            //if (ForNode.TryParseFor(enumerator, out parsedNode))
            //{
            //    return true;
            //}
            //if (IfNode.TryParseIf(enumerator, out parsedNode))
            //{
            //    return true;
            //}
            //if (BlockNode.TryParseBlock(enumerator, out parsedNode))
            //{
            //    return true;
            //}
            //if (ExtendsNode.TryParseExtends(enumerator, out parsedNode))
            //{
            //    return true;
            //}
            //if (RawNode.TryParseRaw(enumerator, out parsedNode))
            //{
            //    return true;
            //}
            if (MacroNode.TryParseMacro(enumerator, out parsedNode))
            {
                return true;
            }
            if (CallNode.TryParseCall(enumerator, out parsedNode))
            {
                return true;
            }
            return false;
        }

    }
}
