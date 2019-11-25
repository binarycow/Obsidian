using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser;

namespace Obsidian
{
    public class Self
    {
        public RenderMode RenderMode { get; set; } = RenderMode.Direct;
        private Dictionary<string, List<Block>> Blocks { get; } = new Dictionary<string, List<Block>>();
        public Queue<ExpressionData> TemplateQueue { get; } = new Queue<ExpressionData>();

        public Block this[string name]
        {
            get
            {
                return Blocks[name][0];
            }
        }

        public void EnqueueTemplate(ExpressionData template)
        {
            TemplateQueue.Enqueue(template);
        }
        public ExpressionData DequeueTemplate()
        {
            return TemplateQueue.Dequeue();
        }
        public bool HasQueuedTemplates => TemplateQueue.Count > 0;
        public void AddBlock(string name, Block block)
        {
            if(Blocks.TryGetValue(name, out var blockList) == false)
            {
                blockList = new List<Block>();
                Blocks.Add(name, blockList);
            }
            blockList.Add(block);
        }
    }
}
