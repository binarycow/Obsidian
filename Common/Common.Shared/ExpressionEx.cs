using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Common.ExpressionCreators;
using Object = Common.ExpressionCreators.Object;
#if DEBUG
using Console = Common.ExpressionCreators.Console;
#endif

namespace Common
{
    internal class ExpressionEx
    {
#if DEBUG
        internal static Lazy<Console> _ConsoleWriter = new Lazy<Console>();
        internal static Console Console => _ConsoleWriter.Value;
#endif
        internal static Lazy<StringBuilder> _StringBuilder = new Lazy<StringBuilder>();
        internal static StringBuilder StringBuilder => _StringBuilder.Value;
        //internal static Lazy<Enumerable> _Enumerable = new Lazy<Enumerable>();
        //internal static Enumerable Enumerable => _Enumerable.Value;

        internal static Lazy<Object> _Object = new Lazy<Object>();
        internal static Object Object => _Object.Value;

        internal static Expression New_Generic(Type openGenericType, Type[] typeArguments, params Expression[] constructorArguments)
        {
            if (MethodLookups.TryGet_Constructor_GenericType(out var constructorInfo,
                openGenericType, typeArguments, constructorArguments.Select(exp => exp.Type).ToArray()) == false)
            {
                throw new NotImplementedException();
            }
            return Expression.New(constructorInfo, constructorArguments);
        }

        internal static bool ToArray(Expression collection, [NotNullWhen(true)]out Expression? arrayExpression, [NotNullWhen(true)]out Type? elementType)
        {
            arrayExpression = default;
            elementType = default;
            if (collection.Type.IsArray)
            {
                arrayExpression = collection;
                elementType = collection.Type.GetElementType();
                return true;
            }
            if(collection.Type.IsAssignableToGenericType(typeof(IEnumerable<>), out var genericTypeArguments))
            {
                MethodLookups.TryGet_Enumerable_ToArray(genericTypeArguments[0], out var methodInfo);
                arrayExpression = Expression.Call(null, methodInfo, collection);
                elementType = genericTypeArguments[0];
                return true;
            }
            throw new NotImplementedException();
        }



        internal static Expression CreateDictionary<TKey, TValue>(ParameterExpression[] items)
        {
            var type = typeof(Dictionary<TKey, TValue>);
            var addMethod = type.GetMethod(nameof(Dictionary<TKey, TValue>.Add));
            var itemExpressions = items.Select(item => Expression.ElementInit(addMethod, Expression.Constant(item.Name), Expression.Convert(item, typeof(TValue)))).ToArray();
            return Expression.ListInit(Expression.New(type), itemExpressions);
        }
    }
}
