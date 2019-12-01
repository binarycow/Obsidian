using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.AST;

namespace Obsidian.WhiteSpaceControl
{
    internal static class WhiteSpaceController
    {
        public static ASTNode ControlWhiteSpace(JinjaEnvironment environment, ASTNode templateNode)
        {
            if (environment.Settings.TrimBlocks) templateNode.Transform(TrimBlocksVisitor.Instance);
            if (environment.Settings.LStripBlocks) templateNode.Transform(LStripBlocksVisitor.Instance);
            templateNode.Transform(ManualWhiteSpaceVisitor.Instance);
            return templateNode;
        }
    }
}
