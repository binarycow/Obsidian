using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Common.ExpressionCreators;
using ExpressionParser;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian
{
    public static class SelfExtensions
    {



        public static Expression SetRenderMode(this ExpressionExtensionData<Self> self, RenderMode renderMode)
        {
            return ObsidianExpressionEx.Self.SetRenderMode(self, renderMode);
        }
        public static Expression GetRenderMode(this ExpressionExtensionData<Self> self)
        {
            return ObsidianExpressionEx.Self.RenderMode(self);
        }
        public static Expression EnqueueIntoTemplateQueue(this ExpressionExtensionData<Self> self, Expression template)
        {
            return ObsidianExpressionEx.Self.EnqueueIntoTemplateQueue(self, template);
        }
        public static Expression AddBlock(this ExpressionExtensionData<Self> self, string blockName, Expression blockExpression)
        {
            return ObsidianExpressionEx.Self.AddBlock(self, blockName, blockExpression);
        }
        public static Expression HasQueuedTemplates(this ExpressionExtensionData<Self> self)
        {
            return ObsidianExpressionEx.Self.HasQueuedTemplates(self);
        }
        public static Expression DequeueTemplate(this ExpressionExtensionData<Self> self)
        {
            return ObsidianExpressionEx.Self.DequeueTemplate(self);
        }
        public static Expression TemplateQueueCount(this ExpressionExtensionData<Self> self)
        {
            return ObsidianExpressionEx.Self.TemplateQueueCount(self);
        }
    }
}
