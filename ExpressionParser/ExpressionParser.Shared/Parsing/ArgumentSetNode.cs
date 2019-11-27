using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    public class ArgumentSetNode : ASTNode
    {
        public ArgumentSetNode(IEnumerable<ASTNode> arguments) : base(arguments.SelectMany(arg => arg.Tokens))
        {
            Arguments = arguments.ToArrayWithoutInstantiation();
        }
        public ASTNode[] Arguments { get; }
        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            throw new NotImplementedException();
        }
    }
}
