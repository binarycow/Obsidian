using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Operators;
using ExpressionParser.Parsing;
using ExpressionParser.References;

namespace ExpressionParser.Transforming.Nodes
{
    internal interface INodeTransformVisitor<TOutput>
    {
        TOutput Transform(BinaryASTNode item);
        TOutput Transform(UnaryASTNode item);
        TOutput Transform(LiteralNode item);
        TOutput Transform(IdentifierNode item);
        TOutput Transform(DictionaryItemNode item);
        TOutput Transform(DictionaryNode item);
        TOutput Transform(TupleNode item);
        TOutput Transform(ListNode item);

        TOutput Transform(PipelineMethodGroup item);
        TOutput Transform(ArgumentSetNode item);

    }
}
