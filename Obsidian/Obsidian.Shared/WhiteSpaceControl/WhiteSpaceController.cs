using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.AST;

namespace Obsidian.WhiteSpaceControl
{
    public static class WhiteSpaceController
    {
        public static ASTNode ControlWhiteSpace(JinjaEnvironment environment, ASTNode templateNode)
        {
            var trimVisitor = new TrimBlocksVisitor(environment);

            var trimmed = templateNode.Transform(trimVisitor);

            return trimmed;
        }
    }
}
