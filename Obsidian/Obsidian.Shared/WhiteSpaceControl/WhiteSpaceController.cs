using System;
using System.Linq;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian.WhiteSpaceControl
{
    public static class WhiteSpaceController
    {
        internal static ASTNode ControlWhiteSpace(ASTNode templateNode)
        {
            var intermediateNode = templateNode.Transform(DisableStripBlocksVisitor.Instance);
            intermediateNode = intermediateNode.Transform(ManualTrimBeforeVisitor.Instance).First();
            intermediateNode = intermediateNode.Transform(ManualTrimAfterVisitor.Instance);
            return intermediateNode;
        }
    }

}
