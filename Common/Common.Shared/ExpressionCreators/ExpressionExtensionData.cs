using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Common.ExpressionCreators
{

    public class ExpressionExtensionData<TData> : ExpressionExtensionData
    {
        public ExpressionExtensionData(ParameterExpression expression) : base(expression, typeof(TData))
        {
            ParameterExpression = expression;
            Type = expression.Type;
        }

        public static implicit operator Expression(ExpressionExtensionData<TData> expr)
        {
            return expr.ParameterExpression;
        }


        public static bool TryCreate(ParameterExpression expression, [NotNullWhen(true)]out ExpressionExtensionData<TData>? expressionData)
        {
            expressionData = default;
            if(expression.Type == typeof(TData))
            {
                expressionData = new ExpressionExtensionData<TData>(expression);
            }
            return expressionData != null;
        }

        public static bool TryCreate_Generic(ParameterExpression variableExpression,
            [NotNullWhen(true)]out ExpressionExtensionData<TData>? expressionData,
            [NotNullWhen(true)]out Expression? newExpression,
            Type[] typeArguments,
            params Expression[] constructorArguments)
        {
            newExpression = default;
            expressionData = default;
            var constructorArgumentTypes = constructorArguments.Select(arg => arg.Type).ToArray();
            if(MethodLookups.TryGet_Constructor_GenericType(out var constructorInfo, typeof(TData), typeArguments, constructorArgumentTypes) == false)
            {
                return false;
            }
            newExpression = Expression.New(constructorInfo, constructorArguments);
            if (variableExpression.Type != newExpression.Type) throw new NotImplementedException();
            if (variableExpression.Type != typeof(TData)) throw new NotImplementedException();
            expressionData = new ExpressionExtensionData<TData>(variableExpression);
            return true;
        }
        public static bool TryCreate(ParameterExpression variableExpression,
            [NotNullWhen(true)]out ExpressionExtensionData<TData>? expressionData,
            [NotNullWhen(true)]out Expression? newExpression,
            params Expression[] constructorArguments)
        {
            return TryCreate_Generic(variableExpression, out expressionData, out newExpression, Type.EmptyTypes, constructorArguments);
        }
    }

    public class ExpressionExtensionData
    {
        public ExpressionExtensionData(ParameterExpression expression, Type type)
        {
            ParameterExpression = expression;
            Type = type;
        }
        public ParameterExpression ParameterExpression { get; set; }
        public Type Type { get; set; }


        public static implicit operator ExpressionExtensionData(ParameterExpression expr)
        {
            return new ExpressionExtensionData(expr, expr.Type);
        }
        public static implicit operator ParameterExpression(ExpressionExtensionData expr)
        {
            return expr.ParameterExpression;
        }

        public static bool TryCreate<TData>(ParameterExpression expression, [NotNullWhen(true)]out ExpressionExtensionData<TData>? expressionData)
        {
            return ExpressionExtensionData<TData>.TryCreate(expression, out expressionData);
        }
        public static bool TryCreate_Generic<TData>(ParameterExpression variableExpression,
            [NotNullWhen(true)]out ExpressionExtensionData<TData>? expressionData,
            [NotNullWhen(true)]out Expression? newExpression,
            Type[] typeArguments,
            params Expression[] constructorArguments)
        {
            return ExpressionExtensionData<TData>.TryCreate_Generic(variableExpression, out expressionData, out newExpression, typeArguments, constructorArguments);
        }
        public static bool TryCreate<TData>(ParameterExpression variableExpression,
            [NotNullWhen(true)]out ExpressionExtensionData<TData>? expressionData,
            [NotNullWhen(true)]out Expression? newExpression,
            params Expression[] constructorArguments)
        {
            return ExpressionExtensionData<TData>.TryCreate_Generic(variableExpression, out expressionData, out newExpression, Type.EmptyTypes, constructorArguments);
        }
    }
}
