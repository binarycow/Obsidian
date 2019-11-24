using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST
{
    public abstract class ASTNode : ITransformable
    {
        public ASTNode(IEnumerable<ParsingNode> parsingNodes)
        {
            ParsingNodes = parsingNodes.ToArray();
        }
        public ASTNode(ParsingNode parsingNode) : this(Enumerable.Repeat(parsingNode, 1))
        {
        }

        public ParsingNode[] ParsingNodes { get; }

        public abstract TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor);

        public override string ToString()
        {
            return string.Join(string.Empty, ParsingNodes.SelectMany(x => x.ToString()));
        }

        public string ToString(bool debug)
        {
            return debug ? ToString().WhiteSpaceEscape() : ToString();
        }

    }
}
