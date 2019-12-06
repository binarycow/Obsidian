using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Transforming.Nodes;
using System.Diagnostics;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ArgumentSetNode : ASTNode
    {
        public ArgumentSetNode(IEnumerable<ASTNode> arguments) : base(arguments.SelectMany(arg => arg.Tokens))
        {
            Arguments = arguments.ToArrayWithoutInstantiation();
        }
        public ASTNode[] Arguments { get; }

        public override string DebuggerDisplay => $"({string.Join(",", Arguments.Select(item => item.DebuggerDisplay))})";

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            throw new NotImplementedException();
        }
    }
}
