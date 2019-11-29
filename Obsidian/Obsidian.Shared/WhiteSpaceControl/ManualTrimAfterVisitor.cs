using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian.WhiteSpaceControl
{
    public class ManualTrimAfterVisitor : BaseASTTransformer
    {
        public static Lazy<ManualTrimAfterVisitor> _Instance = new Lazy<ManualTrimAfterVisitor>();
        public static ManualTrimAfterVisitor Instance => _Instance.Value;

        private bool _Trim = false;
        public override ASTNode Transform(NewLineNode item)
        {
            if (_Trim) return EmptyNode.Instance;
            return base.Transform(item);
        }
        public override ASTNode Transform(WhiteSpaceNode item)
        {
            if (_Trim) return EmptyNode.Instance;
            return base.Transform(item);
        }


        public override ASTNode Transform(ContainerNode item)
        {
            _Trim = false;
            if (item.EndWhiteSpace == WhiteSpaceControlMode.Trim)
            {
                _Trim = true;
            }
            return base.Transform(item);
        }

        public override ASTNode Transform(ExpressionNode item)
        {
            _Trim = false;
            if (item.EndWhiteSpace == WhiteSpaceControlMode.Trim)
            {
                _Trim = true;
            }
            return base.Transform(item);
        }
    }
}
