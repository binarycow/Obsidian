using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Parsing;

namespace Obsidian.AST.Nodes
{
    public abstract class AbstractContainerNode : ASTNode, IWithChildren
    {
        public AbstractContainerNode(ParsingNode? startingParsingNode, IEnumerable<ASTNode> children, ParsingNode? endingParsingNode)
            : base(startingParsingNode, children.SelectMany(child => child.ParsingNodes), endingParsingNode)
        {
            Children = children.ToArrayWithoutInstantiation();
        }

        public ASTNode[] Children { get; }
    }
}
