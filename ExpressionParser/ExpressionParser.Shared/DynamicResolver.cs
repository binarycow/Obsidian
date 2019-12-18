using Common;
using ExpressionParser.Configuration;
using ExpressionParser.References;
using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExpressionParser
{
    internal static class DynamicResolver
    {
        [Flags]
        internal enum MemberTypes
        {
            None = 0,
            Field = 1,
            Property = 2,
            StringIndexer = 4,
        }

        private static bool FastHasFlag(MemberTypes left, MemberTypes right)
        {
            return ((left & right) != 0);
        }

        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "<Pending>")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        internal static object? CallMethod<TScope, TRootScope>(ILanguageDefinition languageDefinition, ScopeStack<TScope, TRootScope> scopeStack, object? left, object?[] args)
            where TScope : class, IScope
            where TRootScope : class, TScope
        {
            switch(left)
            {
                case FunctionMethodGroup methodGroup:
                    return methodGroup.FunctionDefinition.Invoke(languageDefinition, args);
                case UserDefinedFunction userDefinedFunction:
                    return userDefinedFunction.Invoke(languageDefinition, args);
                case PipelineMethodGroup pipelineGroup:
                    return pipelineGroup.FunctionDefinition.Invoke(languageDefinition, pipelineGroup.ReferredObject, args);
                case ScopedFunctionMethodGroup scopedMethodGroup:
                    return scopedMethodGroup.FunctionDefinition.Invoke(languageDefinition, scopeStack.Current, args);
                case DynamicObject dynamicObject:
                    throw new NotImplementedException();
                default:
                    var callable = ReflectionHelpers.GetCallable(left);
                    if(callable != null)
                    {
                        return callable.Invoke(left, new object[] { args });
                    }
                    throw new NotImplementedException();
            }

#pragma warning disable CS8321 // Local function is declared but never used
            static object? FuncMethodGroup(ScopeStack<TScope, TRootScope> scopeStack, FunctionMethodGroup left, object?[] args)
#pragma warning restore CS8321 // Local function is declared but never used
            {
                throw new NotImplementedException();
                //if (left.FunctionDefinition.OverloadDefinitions.Length != 1) throw new NotImplementedException();
                //var overload = left.FunctionDefinition.OverloadDefinitions[0];
                //switch (overload)
                //{
                //    case SingleTypeOverloadDefinition singleOverload:
                //        if (args.Length < singleOverload.MinimumArguments) throw new NotImplementedException();
                //        if (args.Length > singleOverload.MaximumArguments) throw new NotImplementedException();

                //        var invalidTypes = args.Any(arg => TypeCoercion.CanCast(arg?.GetType() ?? typeof(object), singleOverload.ArgumentType));
                //        if (invalidTypes) throw new NotImplementedException();

                //        var result = singleOverload.Invoke(scopeStack.Current, args);
                //        if ((result?.GetType() ?? typeof(object)) != singleOverload.ReturnType) throw new NotImplementedException();
                //        return result;
                //    default:
                //        throw new NotImplementedException();
                //}
            }
        }

        internal static bool TryIndex(object? @object, object?[] args, out object? result)
        {
            result = default;
            if (@object == null) throw new NotImplementedException();
            var types = args.Select(arg => arg?.GetType() ?? typeof(object)).ToArray();
            var objType = @object.GetType();

            if(objType.IsArray)
            {
                throw new NotImplementedException();
            }

            var indexer = objType.GetProperty("Item", types);
            if (indexer == null) return false;

            result = indexer.GetValue(@object, args);
            return true;
        }

        internal static bool TryGetMember(object? @object, string memberName, MemberTypes allowedTypes, out object? result)
        {
            result = default;
            if (@object == null) throw new NotImplementedException();
            var type = @object.GetType();
            if (FastHasFlag(allowedTypes, MemberTypes.Property))
            {
                var propertyInfo = type.GetProperty(memberName);
                if (propertyInfo != null)
                {
                    result = propertyInfo.GetValue(@object);
                    return true;
                }
            }
            if (FastHasFlag(allowedTypes, MemberTypes.Field))
            {
                var fieldInfo = type.GetField(memberName);
                if (fieldInfo != null)
                {
                    result = fieldInfo.GetValue(@object);
                    return true;
                }
            }
            if (FastHasFlag(allowedTypes, MemberTypes.StringIndexer))
            {
                var indexInfo = type.GetProperty("Item");
                if (indexInfo != null)
                {
                    try
                    {
                        result = indexInfo.GetValue(@object, new object[] { memberName });
                        return true;
                    }
                    catch(TargetInvocationException inv) when (inv.InnerException is KeyNotFoundException)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
