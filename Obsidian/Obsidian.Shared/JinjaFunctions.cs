using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian
{
    public static class JinjaFunctions
    {

        public static object? Super(IScope scope)
        {
            // TODO: This only works with string render tranformer

            var rootContext = GetRootContext(scope);
            if (rootContext.CurrentBlockName == null) throw new NotImplementedException();
            var nextBlock = rootContext.GetBlock(rootContext.CurrentBlockName, (rootContext.CurrentBlockIndex ?? 0) + 1);
            if (nextBlock == null) return null;
            nextBlock.Transform(rootContext.Transformer);
            return string.Empty;
            DynamicRootContext GetRootContext(IScope scope)
            {
                switch (scope)
                {
                    case DynamicRootContext rootContext:
                        return rootContext;
                    case DynamicContext dynamicContext:
                        var root = dynamicContext.FindRootScope();
                        if (root is DynamicRootContext dynRoot) return dynRoot;
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }

            }
        }
        public static object? Escape(object?[] args)
        {
            if (args.Length != 1) throw new NotImplementedException();
            return args[0]?.ToString()?.HTMLEscape() ?? string.Empty;
        }
        public static object? Upper(object?[] args)
        {
            if (args.Length != 1) throw new NotImplementedException();
            return args[0].ToString().ToUpper();
        }
    }
}
