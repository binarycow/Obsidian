using ExpressionParser.Scopes;
using Obsidian.AST.Nodes.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian
{
    internal class DynamicContext : DynamicScope
    {
        internal DynamicContext(string name, DynamicContext parent) : base(name, parent)
        {

        }
        protected DynamicContext(DynamicContext parent) : base(parent)
        {

        }
        protected DynamicContext(string? name) : base(name)
        {

        }

        public override IScope CreateChild()
        {
            return new DynamicContext(this);
        }
        public override IScope CreateChild(string name)
        {
            return new DynamicContext(name, this);
        }
    }
}
