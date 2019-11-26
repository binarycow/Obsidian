using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Common;
using Common.ExpressionCreators;
using ExpressionParser;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian.ExpressionCreators
{
    public static class SelfEx
    {
        private static Lazy<MethodInfo> _EnqueueTemplate = new Lazy<MethodInfo>(() =>
            MethodLookups.GetMethod(typeof(Obsidian.Self), nameof(Obsidian.Self.EnqueueTemplate), new[] { typeof(Expression) }));
        private static Lazy<MethodInfo> _DequeueTemplate = new Lazy<MethodInfo>(() =>
            MethodLookups.GetMethod(typeof(Obsidian.Self), nameof(Obsidian.Self.DequeueTemplate), Type.EmptyTypes));
        private static Lazy<MethodInfo> _AddBlock = new Lazy<MethodInfo>(() =>
            MethodLookups.GetMethod(typeof(Obsidian.Self), nameof(Obsidian.Self.AddBlock), new[] { typeof(string), typeof(Expression) }));
        private static Lazy<MethodInfo> _GetBlock = new Lazy<MethodInfo>(() =>
            MethodLookups.GetMethod(typeof(Obsidian.Self), nameof(Obsidian.Self.GetBlock), new[] { typeof(string) }));

        internal static Expression SetRenderMode(Expression self, RenderMode renderMode)
        {
            return Expression.Assign(
                Expression.Property(self, nameof(Obsidian.Self.RenderMode)),
                Expression.Constant(renderMode)
            );
        }
        internal static Expression RenderMode(Expression self)
        {
            return Expression.Property(self, nameof(Obsidian.Self.RenderMode));
        }
        internal static Expression HasQueuedTemplates(Expression self)
        {
            return Expression.Property(self, nameof(Obsidian.Self.HasQueuedTemplates));
        }

        internal static Expression TemplateQueueCount(Expression self)
        {
            return Expression.Property(self, nameof(Obsidian.Self.TemplateQueueCount));
        }

        internal static Expression DequeueTemplate(Expression self)
        {
            return Expression.Call(self, _DequeueTemplate.Value);
        }

        internal static Expression EnqueueIntoTemplateQueue(Expression self, Expression template)
        {
            return Expression.Call(self, _EnqueueTemplate.Value, new[]
            {
                template
            });
        }
        internal static Expression AddBlock(Expression self, string blockName, Expression blockExpression)
        {
            return Expression.Call(self, _AddBlock.Value, new[]
            {
                Expression.Constant(blockName),
                Expression.Constant(blockExpression)
            });
        }

        internal static Expression GetBlock(Expression self, string blockName)
        {
            return Expression.Call(self, _GetBlock.Value, new[]
            {
                Expression.Constant(blockName),
            });
        }
    }
}
