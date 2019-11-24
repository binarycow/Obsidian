using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Operators;
using ExpressionParser.Parsing;

namespace ExpressionParser.Transforming.Nodes
{
    public interface INodeTransformVisitor<TOutput>
    {
        TOutput Transform(BinaryASTNode item);
        TOutput Transform(UnaryASTNode item);
        TOutput Transform(LiteralNode item);
        TOutput Transform(IdentifierNode item);
        TOutput Transform(DictionaryItemNode item);
        TOutput Transform(DictionaryNode item);
        TOutput Transform(TupleNode item);
        TOutput Transform(ListNode item);
    }
}
