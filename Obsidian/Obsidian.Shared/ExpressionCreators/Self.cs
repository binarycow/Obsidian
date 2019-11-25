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
    public class Self
    {
        private Lazy<MethodInfo> _EnqueueTemplate = new Lazy<MethodInfo>(() =>
            MethodLookups.GetMethod(typeof(Obsidian.Self), nameof(Obsidian.Self.EnqueueTemplate), new[] { typeof(ExpressionData) }));
        private Lazy<MethodInfo> _DequeueTemplate = new Lazy<MethodInfo>(() =>
            MethodLookups.GetMethod(typeof(Obsidian.Self), nameof(Obsidian.Self.DequeueTemplate), Type.EmptyTypes));
        private Lazy<MethodInfo> _AddBlock = new Lazy<MethodInfo>(() =>
            MethodLookups.GetMethod(typeof(Obsidian.Self), nameof(Obsidian.Self.AddBlock), new[] { typeof(string), typeof(Expression) }));

        internal Expression SetRenderMode(ExpressionExtensionData<Obsidian.Self> self, RenderMode renderMode)
        {
            return Expression.Assign(
                Expression.Property(self.ParameterExpression, nameof(Obsidian.Self.RenderMode)),
                Expression.Constant(renderMode)
            );
        }
        internal Expression RenderMode(ExpressionExtensionData<Obsidian.Self> self)
        {
            return Expression.Property(self.ParameterExpression, nameof(Obsidian.Self.RenderMode));
        }
        internal Expression HasQueuedTemplates(ExpressionExtensionData<Obsidian.Self> self)
        {
            return Expression.Property(self.ParameterExpression, nameof(Obsidian.Self.HasQueuedTemplates));
        }

        internal Expression DequeueTemplate(ExpressionExtensionData<Obsidian.Self> self)
        {
            return Expression.Call(self.ParameterExpression, _DequeueTemplate.Value);
        }

        internal Expression EnqueueIntoTemplateQueue(ExpressionExtensionData<Obsidian.Self> self, Expression template)
        {
            return Expression.Call(self.ParameterExpression, _EnqueueTemplate.Value, new[]
            {
                template
            });
        }
        internal Expression AddBlock(ExpressionExtensionData<Obsidian.Self> self, string blockName, Expression blockExpression)
        {
            return Expression.Call(self.ParameterExpression, _AddBlock.Value, new[]
            {
                Expression.Constant(blockName),
                Expression.Constant(blockExpression)
            });
        }
    }
}
