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
        public Queue<Expression> TemplateQueue { get; } = new Queue<Expression>();

        public Block this[string name]
        {
            get
            {
                return Blocks[name][0];
            }
        }

        public void EnqueueTemplate(Expression template)
        {
            TemplateQueue.Enqueue(template);
        }
        public Expression DequeueTemplate()
        {
            return TemplateQueue.Dequeue();
        }
        public bool HasQueuedTemplates => TemplateQueue.Count > 0;
        public void AddBlock(string blockName, Expression blockExpression)
        {
            if(Blocks.TryGetValue(blockName, out var blockList) == false)
            {
                blockList = new List<Block>();
                Blocks.Add(blockName, blockList);
            }
            var block = new Block(blockName, blockList.Count, blockExpression);
            blockList.Add(block);
        }
    }
}
