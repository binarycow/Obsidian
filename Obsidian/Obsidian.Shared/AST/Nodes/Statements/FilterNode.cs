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
    internal class FilterNode : AbstractContainerNode
    {

        internal FilterNode(ParsingNode? startParsingNode, string filter, ContainerNode filterContents, ParsingNode? endParsingNode)
            : base(startParsingNode, filterContents.YieldOne(), endParsingNode)
        {
            Filter = filter;
            FilterContents = filterContents;
        }

        private string DebuggerDisplay => $"{nameof(FilterNode)} : {Filter}";

        internal string Filter { get; }
        internal ContainerNode FilterContents { get; set; }

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

        internal static bool TryParseFilter(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;
            if (FilterParser.StartBlock.TryParse(enumerator.Current) == false)
            {
                return false;
            }
            var startParsingNode = enumerator.Current;
            if (FilterParser.StartBlock.TryGetAccumulation(FilterParser.FilterState.FilterName, 0, out var filterName) == false)
            {
                throw new NotImplementedException();
            }
            if (string.IsNullOrEmpty(filterName)) throw new NotImplementedException();
            enumerator.MoveNext();

            var contents = ASTGenerator.ParseUntilFailure(environment, lexer, enumerator).ToArray();


            if (FilterParser.EndBlock.TryParse(enumerator.Current) == false)
            {
                throw new NotImplementedException();
            }

            var contentsNode = new ContainerNode(null, contents, null);

            parsedNode = new FilterNode(startParsingNode, filterName, contentsNode, enumerator.Current);
            return true;
        }
    }
}
