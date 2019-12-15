using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Lexing;
using Obsidian.Parsing;

namespace Obsidian.AST.Nodes
{
    internal abstract class StatementNode : ASTNode, IWithChildren
    {
        internal StatementNode(ParsingNode? startParsingNode, IEnumerable<ASTNode> children, ParsingNode? endParsingNode) 
            : base(startParsingNode, children.SelectMany(x => x.ParsingNodes), endParsingNode)
        {
            Children = children.ToArrayWithoutInstantiation();
        }

        public ASTNode[] Children { get; }

        internal delegate bool TryParseDelegate(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode);

        private static readonly TryParseDelegate[] _Delegates = new TryParseDelegate[]
        {
            ForNode.TryParseFor,
            IfNode.TryParseIf,
            BlockNode.TryParseBlock,
            ExtendsNode.TryParseExtends,
            RawNode.TryParseRaw,
            MacroNode.TryParseMacro,
            CallNode.TryParseCall,
            FilterNode.TryParseFilter,
            SetNode.TryParseSet,
            IncludeNode.TryParseInclude,
            ImportNode.TryParseImport,
        };

        internal static bool TryParse(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = _Delegates.Select(del =>
            {
                var Result = del(environment, lexer, enumerator, out var ParsedNode);
                return new
                {
                    Result,
                    ParsedNode,
                };
            }).FirstOrDefault(res => res.Result)?.ParsedNode;
            return parsedNode != default;
        }

    }
}
