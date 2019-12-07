using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.AST;
using Obsidian.AST.Nodes.MiscNodes;

namespace Obsidian.Transforming
{
    internal class WhiteSpaceCounterVisitor : BaseASTTransformer
    {
        private int _Tally = 0;

        public int Test(ASTNode node)
        {
            _Tally = 0;
            node.Transform(this);
            return _Tally;
        }

        public override ASTNode Transform(WhiteSpaceNode item)
        {
            return base.Transform(item);
        }
    }
}
