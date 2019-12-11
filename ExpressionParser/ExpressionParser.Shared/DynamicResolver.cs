using Common;
using ExpressionParser.Configuration;
using ExpressionParser.References;
using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            return left switch
            {
                FunctionMethodGroup methodGroup => methodGroup.FunctionDefinition.Invoke(languageDefinition, args),
                UserDefinedFunction userDefinedFunction => userDefinedFunction.Invoke(languageDefinition, args),
                PipelineMethodGroup pipelineGroup => pipelineGroup.FunctionDefinition.Invoke(languageDefinition, pipelineGroup.ReferredObject, args),
                ScopedFunctionMethodGroup scopedMethodGroup => scopedMethodGroup.FunctionDefinition.Invoke(languageDefinition, scopeStack.Current, args),
                _ => throw new NotImplementedException(),
            };



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
                    result = indexInfo.GetValue(@object, new object[] { memberName });
                    return true;
                }
            }
            return false;
        }
    }
}
