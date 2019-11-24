﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Operators;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    public class BinaryASTNode : ASTNode
    {
        public BinaryASTNode(ASTNode left, Operator @operator, ASTNode right) : base(left.Tokens.Concat(@operator.Token).Concat(right.Tokens))
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }
        public ASTNode Left { get; }
        public Operator Operator { get;  }
        public ASTNode Right { get; }

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}