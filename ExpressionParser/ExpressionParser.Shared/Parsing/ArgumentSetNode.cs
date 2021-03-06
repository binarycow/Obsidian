using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Transforming.Nodes;
using System.Diagnostics;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class ArgumentSetNode : ASTNode
    {
        internal ArgumentSetNode(IEnumerable<ASTNode> arguments) : base(arguments.SelectMany(arg => arg.Tokens))
        {
            Arguments = arguments.ToArrayWithoutInstantiation();
        }
        internal ASTNode[] Arguments { get; }

        internal override string DebuggerDisplay => $"({string.Join(",", Arguments.Select(item => item.DebuggerDisplay))})";

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
