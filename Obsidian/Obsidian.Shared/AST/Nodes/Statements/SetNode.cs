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
    internal class SetNode : AbstractContainerNode
    {

        internal SetNode(ParsingNode? startParsingNode, string[] variableNames, ContainerNode assignmentBlock, ParsingNode? endParsingNode)
            : base(startParsingNode, assignmentBlock.YieldOne(), endParsingNode)
        {
            VariableNames = variableNames;
            AssignmentBlock = assignmentBlock;
            AssignmentExpression = null;
        }
        internal SetNode(ParsingNode? startParsingNode, string[] variableNames, string assignmentExpression, ParsingNode? endParsingNode)
            : base(startParsingNode, Enumerable.Empty<ASTNode>(), endParsingNode)
        {
            VariableNames = variableNames;
            AssignmentBlock = null;
            AssignmentExpression = assignmentExpression;
        }

        private string DebuggerDisplay => $"{nameof(FilterNode)} : {VariableNames}";

        internal string[] VariableNames { get; }
        internal ContainerNode? AssignmentBlock { get; set; }
        internal string? AssignmentExpression { get; set; }

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

        internal static bool TryParseSet(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;
            if (SetParser.StartBlock.TryParse(enumerator.Current) == false)
            {
                return false;
            }
            var startParsingNode = enumerator.Current;
            if (SetParser.StartBlock.TryGetAccumulations(SetParser.SetState.VariableName, out var variableNames) == false)
            {
                throw new NotImplementedException();
            }
            if (variableNames == null || variableNames.Length == 0) throw new NotImplementedException();
            if (variableNames.Length != 1) throw new NotImplementedException(); // Not supported yet

            if (SetParser.StartBlock.TryGetAccumulation(SetParser.SetState.AssignmentExpression, 0, out var assignmentExpression))
            {
                parsedNode = new SetNode(startParsingNode, variableNames, assignmentExpression, null);
                return true;
            }

            enumerator.MoveNext();
            var contents = ASTGenerator.ParseUntilFailure(environment, lexer, enumerator).ToArray();


            if (SetParser.EndBlock.TryParse(enumerator.Current) == false)
            {
                throw new NotImplementedException();
            }

            var contentsNode = new ContainerNode(null, contents, null);
            parsedNode = new SetNode(startParsingNode, variableNames, contentsNode, enumerator.Current);
            return true;
        }
    }
}
