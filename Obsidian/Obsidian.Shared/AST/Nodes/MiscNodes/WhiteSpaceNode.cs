using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.MiscNodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class WhiteSpaceNode : ASTNode
    {
        public WhiteSpaceNode(IEnumerable<ParsingNode> parsingNodes, WhiteSpaceControlMode controlMode) : base(parsingNodes)
        {
            WhiteSpaceControlMode = controlMode;
        }
        public WhiteSpaceNode(ParsingNode parsingNode, WhiteSpaceControlMode controlMode) : base(parsingNode)
        {
            WhiteSpaceControlMode = controlMode;
        }

        public WhiteSpaceControlMode WhiteSpaceControlMode { get; set; }


        private string DebuggerDisplay => $"{nameof(WhiteSpaceNode)} : \"{ToString(debug: true)}\" Control: {WhiteSpaceControlMode}";
        public static WhiteSpaceNode Parse(ILookaroundEnumerator<ParsingNode> enumerator)
        {
            var nodes = new Queue<ParsingNode>();

            var currentMode = enumerator.Current.WhiteSpaceControlMode;

            nodes.Enqueue(enumerator.Current);

            while (enumerator.TryGetNext(out var nextNode) && nextNode.NodeType == ParsingNodeType.WhiteSpace)
            {
                enumerator.MoveNext();
                nodes.Enqueue(nextNode);
            }
            return new WhiteSpaceNode(nodes, currentMode);
        }
        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
