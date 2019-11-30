using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes.Statements
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class BlockNode : AbstractContainerNode
    {

        public BlockNode(ParsingNode? startParsingNode, string name, ContainerNode blockContents, ParsingNode? endParsingNode)
            : base(startParsingNode, blockContents.YieldOne(), endParsingNode)
        {
            Name = name;
            BlockContents = blockContents;
        }

        private string DebuggerDisplay => $"{nameof(BlockNode)} : {Name}";

        public string Name { get; }
        public ContainerNode BlockContents { get; set; }

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }
        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }

        public static bool TryParseBlock(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;
            if(BlockParser.StartBlock.TryParse(enumerator.Current, out var accumulations) == false)
            {
                return false;
            }
            var startParsingNode = enumerator.Current;
            if (accumulations.TryGetValue(BlockParser.BlockState.BlockName, out var blockNameArray) == false || blockNameArray.Length == 0)
            {
                throw new NotImplementedException();
            }
            var startingBlockName = blockNameArray[0];
            if (string.IsNullOrEmpty(startingBlockName)) throw new NotImplementedException();
            enumerator.MoveNext();

            var contents = ASTGenerator.ParseUntilFailure(enumerator).ToArray();


            if (BlockParser.EndBlock.TryParse(enumerator.Current, out accumulations) == false)
            {
                return false;
            }
            if (accumulations.TryGetValue(BlockParser.BlockState.BlockName, out blockNameArray) &&
                blockNameArray.Length > 0 &&
                string.IsNullOrEmpty(blockNameArray[0]) == false &&
                blockNameArray[0] != startingBlockName)
            {
                throw new NotImplementedException();
            }

            var contentsBlock = new ContainerNode(null, contents, null);

            parsedNode = new BlockNode(startParsingNode, startingBlockName, contentsBlock, enumerator.Current);
            return true;
        }
    }
}
