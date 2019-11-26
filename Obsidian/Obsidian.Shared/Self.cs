using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian
{
    public class Self
    {
        public RenderMode RenderMode { get; set; } = RenderMode.Direct;
        private Dictionary<string, List<Block>> Blocks { get; } = new Dictionary<string, List<Block>>();
        public Queue<Template> TemplateQueue { get; } = new Queue<Template>();

        public Block this[string name]
        {
            get
            {
                return Blocks[name][0];
            }
        }

        public void EnqueueTemplate(Template template)
        {
            TemplateQueue.Enqueue(template);
        }
        public Template DequeueTemplate()
        {
            return TemplateQueue.Dequeue();
        }
        public bool HasQueuedTemplates => TemplateQueue.Count > 0;
        public void AddBlock(string blockName, ExpressionData blockExpression)
        {
            if (Blocks.TryGetValue(blockName, out var blockList) == false)
            {
                blockList = new List<Block>();
                Blocks.Add(blockName, blockList);
            }
            var block = new Block(blockName, blockList.Count, blockExpression);
            blockList.Add(block);
        }
        public Block? GetBlock(string blockName)
        {
            if (Blocks.TryGetValue(blockName, out var blockList) == false) return default;
            return blockList.Last();
        }

        public int TemplateQueueCount => TemplateQueue.Count;
    }
}
