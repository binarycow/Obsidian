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
    internal class BlockNode : AbstractContainerNode
    {

        internal BlockNode(ParsingNode? startParsingNode, string name, ContainerNode blockContents, ParsingNode? endParsingNode)
            : base(startParsingNode, blockContents.YieldOne(), endParsingNode)
        {
            Name = name;
            BlockContents = blockContents;
        }

        private string DebuggerDisplay => $"{nameof(BlockNode)} : {Name}";

        internal string Name { get; }
        internal ContainerNode BlockContents { get; set; }

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false)
        {
            visitor.Transform(this, inner);
        }
        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }
        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }

        internal static bool TryParseBlock(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;
            if(BlockParser.StartBlock.TryParse(enumerator.Current) == false)
            {
                return false;
            }
            var startParsingNode = enumerator.Current;
            if(BlockParser.StartBlock.TryGetAccumulation(BlockParser.BlockState.BlockName, 0, out var startingBlockName) == false)
            {
                throw new NotImplementedException();
            }
            if (string.IsNullOrEmpty(startingBlockName)) throw new NotImplementedException();
            enumerator.MoveNext();

            var contents = ASTGenerator.ParseUntilFailure(environment, lexer, enumerator).ToArray();


            if (BlockParser.EndBlock.TryParse(enumerator.Current) == false)
            {
                throw new NotImplementedException();
            }
            if (BlockParser.EndBlock.TryGetAccumulation(BlockParser.BlockState.BlockName, 0, out var endBlockName) && !string.IsNullOrEmpty(endBlockName) && endBlockName != startingBlockName)
            {
                throw new NotImplementedException();
            }

            var contentsBlock = new ContainerNode(null, contents, null);

            parsedNode = new BlockNode(startParsingNode, startingBlockName, contentsBlock, enumerator.Current);
            return true;
        }
    }
}
