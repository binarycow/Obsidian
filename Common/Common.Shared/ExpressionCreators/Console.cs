using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Common.ExpressionCreators
{
#if DEBUG
    public class Console
    {
        private Type _StringBuilderType = typeof(System.Console);

        private Lazy<Dictionary<Type, MethodInfo>> _WriteMethods = new Lazy<Dictionary<Type, MethodInfo>>();
        private Dictionary<Type, MethodInfo> WriteMethods => _WriteMethods.Value;


        private Lazy<HashSet<Type>> _NoWriteMethods = new Lazy<HashSet<Type>>();
        private HashSet<Type> NoWriteMethods => _NoWriteMethods.Value;


        private Lazy<Dictionary<Type, MethodInfo>> _WriteLineMethods = new Lazy<Dictionary<Type, MethodInfo>>();
        private Dictionary<Type, MethodInfo> WriteLineMethods => _WriteLineMethods.Value;

        private Lazy<HashSet<Type>> _NoWriteLineMethods = new Lazy<HashSet<Type>>();
        private HashSet<Type> NoWriteLineMethods => _NoWriteLineMethods.Value;

        private MethodInfo GetWriteLineMethod(Type type)
        {
            if (WriteLineMethods.TryGetValue(type, out var methodInfo))
            {
                return methodInfo;
            }
            if (NoWriteLineMethods.Contains(type) == false)
            {
                var foundMethod = _StringBuilderType.GetMethod("WriteLine", new[] { type });
                if (foundMethod != null)
                {
                    WriteLineMethods.Add(type, foundMethod);
                    return foundMethod;
                }
                else
                {
                    NoWriteLineMethods.Add(type);
                }
            }
            return GetWriteLineMethod(typeof(object));
        }
        private MethodInfo GetWriteMethod(Type type)
        {
            if (WriteMethods.TryGetValue(type, out var methodInfo))
            {
                return methodInfo;
            }
            if (NoWriteMethods.Contains(type) == false)
            {
                var foundMethod = _StringBuilderType.GetMethod("Write", new[] { type });
                if (foundMethod != null)
                {
                    WriteMethods.Add(type, foundMethod);
                    return foundMethod;
                }
                else
                {
                    NoWriteMethods.Add(type);
                }
            }
            return GetWriteMethod(typeof(object));
        }

        public Expression WriteExpression(Expression item)
        {
            return Expression.Call(null, GetWriteMethod(item.Type), new[] { item });
        }
        public Expression WriteLineExpression(Expression item)
        {
            return Expression.Call(null, GetWriteLineMethod(item.Type), new[] { item });
        }
        public Expression Write(object? item)
        {
            if(!(item is Expression expr))
            {
                expr = Expression.Constant(item);
            }
            return WriteExpression(expr);
        }
        public Expression WriteLine(object? item)
        {
            if (!(item is Expression expr))
            {
                expr = ExpressionEx.ToString(Expression.Constant(item));
            }
            return WriteLineExpression(ExpressionEx.ToString(expr));
        }
    }
#endif
}
