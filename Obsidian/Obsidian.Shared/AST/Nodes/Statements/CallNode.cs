using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Obsidian.AST.Nodes.Statements
{
    public class CallNode : ASTNode, IWhiteSpaceControlling
    {
        public CallNode(ParsingNode? startParsingNode, ExpressionNode call, ContainerNode contents, ParsingNode? endParsingNode, WhiteSpaceControlSet? whiteSpace = null)
            : base(startParsingNode, contents.ParsingNodes, endParsingNode)
        {
            Contents = contents;
            WhiteSpaceControl = whiteSpace ?? new WhiteSpaceControlSet();
            Call = call;
        }

        public ContainerNode Contents { get; }
        public ExpressionNode Call { get; }
        public WhiteSpaceControlSet WhiteSpaceControl { get; }

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }

        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }

        public static bool TryParseCall(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;


            if (CallParser.StartBlock.TryParse(enumerator.Current, out var outsideStart, out var insideStart) == false)
            {
                return false;
            }
            if (CallParser.StartBlock.TryGetAccumulation(CallParser.CallState.CallDefinition, 0, out var callDefinition) == false)
            {
                throw new NotImplementedException();
            }
            var startParsingNode = enumerator.Current;
            enumerator.MoveNext();
            var contents = ASTGenerator.ParseUntilFailure(enumerator).ToArray();
            if (CallParser.EndBlock.TryParse(enumerator.Current, out var insideEnd, out var outsideEnd) == false)
            {
                return false;
            }
            var endParsingNode = enumerator.Current;
            var contentsNode = new ContainerNode(null, contents, null,
                new WhiteSpaceControlSet(insideStart, insideEnd)
            );
            var call = ExpressionNode.FromString(callDefinition);
            parsedNode = new CallNode(startParsingNode, call, contentsNode, endParsingNode,
                new WhiteSpaceControlSet(outsideStart, outsideEnd)
            );
            return true;
        }

    }

}
