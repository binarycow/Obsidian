using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes.Statements
{
    internal class IncludeNode : ASTNode
    {
        public IncludeNode(ParsingNode parsingNode, ExpressionNode templates, bool ignoreMissing, bool? withContext) : base(parsingNode)
        {
            Templates = templates;
            IgnoreMissing = ignoreMissing;
            WithContext = withContext;
        }

        public ExpressionNode Templates { get; }
        public bool IgnoreMissing { get; }
        public bool? WithContext { get; }

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

        public override void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false)
        {
            visitor.Transform(this, inner);
        }




        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        internal static bool TryParseInclude(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;
            if (IncludeParser.Parser.TryParse(enumerator.Current) == false)
            {
                return false;
            }
            if (IncludeParser.Parser.TryGetAccumulation(IncludeParser.IncludeState.TemplateNames, 0, out var templateNames) == false)
            {
                throw new NotImplementedException();
            }
            var templateNamesExpression = ExpressionNode.FromString(environment, templateNames);

            var ignoreMissing = IncludeParser.Parser.GetVariable<bool>("ignoreMissing", false);
            var context = IncludeParser.Parser.GetVariable<bool?>("context", null);
            parsedNode = new IncludeNode(enumerator.Current, templateNamesExpression, ignoreMissing, context);
            return true;
        }

    }
}
