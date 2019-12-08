using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Transforming
{
    internal class TemplateContainerAssembler : BaseASTTransformer
    {
        private readonly static Lazy<TemplateContainerAssembler> _Instance = new Lazy<TemplateContainerAssembler>();
        internal static TemplateContainerAssembler Instance => _Instance.Value;

        public override ASTNode Transform(ExtendsNode item)
        {
            return base.Transform(item);
        }

        public override ASTNode Transform(BlockNode item)
        {
            return base.Transform(item);
        }
    }
}
