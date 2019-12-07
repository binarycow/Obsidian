using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian
{
    internal class DynamicRootContext : DynamicContext
    {
        private DynamicRootContext(StringBuilderTransformer transformer, string? name) : base(name)
        {
            Transformer = transformer;
        }



        public static DynamicRootContext CreateNew(string? name, StringBuilderTransformer transformer, IDictionary<string, object?> variables)
        {
            var scope = new DynamicRootContext(transformer, name);
            foreach (var key in variables.Keys)
            {
                scope.DefineAndSetVariable(key, variables[key]);
            }
            return scope;
        }

        public StringBuilderTransformer Transformer { get; }
        public string? CurrentBlockName { get; set; }
        public int? CurrentBlockIndex { get; set; }


        private readonly Dictionary<string, List<ContainerNode>> _Blocks = new Dictionary<string, List<ContainerNode>>();
        public void AddBlock(string name, ContainerNode containerNode)
        {
            if (_Blocks.TryGetValue(name, out var blockList) == false)
            {
                blockList = new List<ContainerNode>();
                _Blocks.Add(name, blockList);
            }
            if(blockList.Contains(containerNode) == false)
            {
                blockList.Add(containerNode);
            }
            else
            {
                ;
            }
        }
        public ContainerNode? GetBlock(string name, int index = 0)
        {
            if (_Blocks.TryGetValue(name, out var blockList) == false)
            {
                return default;
            }
            if(blockList.Count < index)
            {
                return default;
            }
            return blockList[index];
        }
    }
}
