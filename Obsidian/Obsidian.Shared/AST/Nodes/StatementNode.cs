using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Parsing;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes
{
    public abstract class StatementNode : ASTNode
    {
        public StatementNode(IEnumerable<ASTNode> children, WhiteSpaceControlMode startWhiteSpace, WhiteSpaceControlMode endWhiteSpace) : base(children.SelectMany(child => child.ParsingNodes))
        {
            Children = children.ToArrayWithoutInstantiation();
            StartWhiteSpace = startWhiteSpace;
            EndWhiteSpace = endWhiteSpace;
        }

        public ASTNode[] Children { get; }
        public WhiteSpaceControlMode StartWhiteSpace { get; }
        public WhiteSpaceControlMode EndWhiteSpace { get; }

        public static bool TryParse(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            if (ForNode.TryParseFor(enumerator, out parsedNode))
            {
                return true;
            }
            if (IfNode.TryParseIf(enumerator, out parsedNode))
            {
                return true;
            }
            if (BlockNode.TryParseBlock(enumerator, out parsedNode))
            {
                return true;
            }
            if (ExtendsNode.TryParseExtends(enumerator, out parsedNode))
            {
                return true;
            }
            return false;
        }

    }
}
