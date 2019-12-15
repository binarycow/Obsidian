using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.AST.NodeParsers;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class ExpressionNode : ASTNode, IWhiteSpaceControlling
    {
        internal ExpressionNode(JinjaEnvironment environment, ParsingNode parsingNode, 
            WhiteSpaceMode startWhiteSpace, WhiteSpaceMode endWhiteSpace, string expression, string? ifClause, string? elseClause) : base(parsingNode)
        {
            Expression = expression;
            ExpressionParserNode = environment.Evaluation.Parse(expression);
            if(ifClause != null)
            {
                IfClause = environment.Evaluation.Parse(ifClause);
            }
            if(elseClause != null)
            {
                ElseClause = new ExpressionNode(environment, elseClause);
            }
            WhiteSpaceControl = new WhiteSpaceControlSet(startWhiteSpace, endWhiteSpace);
        }

        private ExpressionNode(JinjaEnvironment environment, string expression) : base(new ParsingNode(ParsingNodeType.Expression, new[] { new Token(TokenType.Unknown, expression) }))
        {
            Expression = expression;
            Output = false;
            ExpressionParserNode = environment.Evaluation.Parse(expression);
            WhiteSpaceControl = new WhiteSpaceControlSet();
        }


        public ExpressionParser.Parsing.ASTNode ExpressionParserNode { get; }
        public WhiteSpaceControlSet WhiteSpaceControl { get; }

        internal string Expression { get; }
        internal ExpressionParser.Parsing.ASTNode? IfClause { get; }
        internal ExpressionNode? ElseClause { get; }

        internal bool Output { get; } = true;

        private string DebuggerDisplay => $"{nameof(ExpressionNode)} : \"{ToString(debug: true)}\"";

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false)
        {
            visitor.Transform(this, inner);
        }
        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }
        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }
        internal static bool TryParse(JinjaEnvironment environment, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;
            string? ifClause = null;
            string? elseClause = null;
            if (ExpressionNodeParser.Parser.TryParse(enumerator.Current, out var startWhiteSpace, out var endWhiteSpace) == false)
            {
                return false;
            }
            if (ExpressionNodeParser.Parser.TryGetAccumulation(ExpressionNodeParser.ExpressionState.Expression, 0, out var expression) == false)
            {
                throw new NotImplementedException();
            }
            if (ExpressionNodeParser.Parser.TryGetAccumulation(ExpressionNodeParser.ExpressionState.IfClause, 0, out var ifClauseString))
            {
                ifClause = ifClauseString;
            }
            if (ExpressionNodeParser.Parser.TryGetAccumulation(ExpressionNodeParser.ExpressionState.ElseClause, 0, out var elseClauseString))
            {
                elseClause = elseClauseString;
            }
            if (string.IsNullOrEmpty(expression)) throw new NotImplementedException();

            parsedNode = new ExpressionNode(environment, enumerator.Current, startWhiteSpace, endWhiteSpace, expression, ifClause, elseClause);
            return true;
        }

        internal static ExpressionNode FromString(JinjaEnvironment environment, string expression)
        {
            return new ExpressionNode(environment, expression);
        }

    }
}
