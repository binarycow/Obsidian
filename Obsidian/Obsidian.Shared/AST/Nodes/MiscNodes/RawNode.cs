using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Obsidian.AST.Nodes.MiscNodes
{
    public class RawNode : ASTNode
    {
        public RawNode(ParsingNode? startParsingNode, IEnumerable<ParsingNode> contents, ParsingNode? endParsingNode)
            : base(startParsingNode, contents, endParsingNode)
        {
            Children = contents.ToArrayWithoutInstantiation();
        }

        public ParsingNode[] Children { get; }

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

        public static bool TryParseRaw(Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            var conditions = new Queue<ConditionalNode>();
            parsedNode = default;
            

            if (RawParser.StartBlock.TryParse(enumerator.Current) == false)
            {
                return false;
            }
            var startParsingNode = enumerator.Current;
            var children = new List<ParsingNode>();

            while(true)
            {
                if(enumerator.MoveNext() == false)
                {
                    throw new NotImplementedException();
                }
                if (RawParser.EndBlock.TryParse(enumerator.Current))
                {
                    break;
                }
                children.Add(enumerator.Current);
            }
            var endParsingNode = enumerator.Current;
            parsedNode = new RawNode(startParsingNode, children, endParsingNode);
            return true;
        }

    }
}
