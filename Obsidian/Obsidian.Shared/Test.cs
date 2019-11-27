using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ExpressionToString;

namespace Obsidian
{
    public static class Test
    {
        public static Expression Something(Expression expression)
        {
            var debug = expression.ToString("C#");
            return expression;
        }
    }
}
