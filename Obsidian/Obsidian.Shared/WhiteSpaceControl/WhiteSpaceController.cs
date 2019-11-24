using System;
using Obsidian.AST;
using Obsidian.AST.Nodes;

namespace Obsidian.WhiteSpaceControl
{
    public static class WhiteSpaceController
    {
        internal static ASTNode ControlWhiteSpace(ASTNode templateNode)
        {
            var intermediateNode = templateNode.Transform(DisableStripBlocksVisitor.Instance);
            intermediateNode = intermediateNode.Transform(ManualTrimBeforeVisitor.Instance);
            intermediateNode = intermediateNode.Transform(ManualTrimAfterVisitor.Instance);
            return intermediateNode;
        }
    }

}
