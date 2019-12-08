using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian
{
    internal class CompiledSelf
    {
        internal RenderMode RenderMode { get; set; } = RenderMode.Direct;
        private Dictionary<string, List<Block>> Blocks { get; } = new Dictionary<string, List<Block>>();
        internal Queue<Expression> TemplateQueue { get; } = new Queue<Expression>();

        internal Block this[string name]
        {
            get
            {
                return Blocks[name][0];
            }
        }

        internal void EnqueueTemplate(Expression template)
        {
            TemplateQueue.Enqueue(template);
        }
        internal Expression DequeueTemplate()
        {
            return TemplateQueue.Dequeue();
        }
        internal bool HasQueuedTemplates => TemplateQueue.Count > 0;
        internal void AddBlock(string blockName, Expression blockExpression)
        {
            if (Blocks.TryGetValue(blockName, out var blockList) == false)
            {
                blockList = new List<Block>();
                Blocks.Add(blockName, blockList);
            }
            var block = new Block(blockName, blockList.Count, blockExpression);
            blockList.Add(block);
        }
        internal Block? GetBlock(string blockName)
        {
            if (Blocks.TryGetValue(blockName, out var blockList) == false) return default;
            return blockList.Last();
        }

        internal int TemplateQueueCount => TemplateQueue.Count;
    }
}
