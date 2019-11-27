using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian
{
    public class DynamicSelf
    {
        private Dictionary<string, List<BlockNode>> _Blocks = new Dictionary<string, List<BlockNode>>();

        public void AddBlock(BlockNode blockNode)
        {
            if (_Blocks.TryGetValue(blockNode.Name, out var blockList) == false)
            {
                blockList = new List<BlockNode>();
                _Blocks.Add(blockNode.Name, blockList);
            }
            blockList.Add(blockNode);
        }
        public BlockNode? GetBlock(string name)
        {
            if (_Blocks.TryGetValue(name, out var blockList) == false)
            {
                return default;
            }
            return blockList.First();
        }
    }
}
