using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian
{
    internal static class OutputCombiner
    {
        internal static IEnumerable<ParsingNode> CombineOutput(IEnumerable<ParsingNode> source)
        {
            var array = source.ToArray();
            var nonWhiteSpaceEncounteredOnLine = false;
            var pendingOutput = new Queue<ParsingNode>();
            var pendingWhiteSpace = new Queue<ParsingNode>();
            var output = new Queue<ParsingNode>();
            foreach (var item in source)
            {
                switch(item.NodeType)
                {
                    case ParsingNodeType.Empty:
                        continue;
                    case ParsingNodeType.WhiteSpace:
                        pendingWhiteSpace.Enqueue(item);
                        break;
                    case ParsingNodeType.Output:
                        if(nonWhiteSpaceEncounteredOnLine == false) output.Enqueue(MakeNode(pendingWhiteSpace, ParsingNodeType.WhiteSpace));
                        MoveWhiteSpaceToOutput();
                        pendingOutput.Enqueue(item);
                        nonWhiteSpaceEncounteredOnLine = true;
                        continue;
                    case ParsingNodeType.NewLine:
                        if (pendingOutput.Count > 0) output.Enqueue(MakeNode(pendingOutput, ParsingNodeType.Output));
                        if (pendingWhiteSpace.Count > 0) output.Enqueue(MakeNode(pendingWhiteSpace, ParsingNodeType.WhiteSpace));
                        output.Enqueue(item);
                        nonWhiteSpaceEncounteredOnLine = false;
                        continue;
                    case ParsingNodeType.Comment:
                    case ParsingNodeType.Expression:
                    case ParsingNodeType.Statement:
                        if (nonWhiteSpaceEncounteredOnLine == false && pendingWhiteSpace.Count > 0) output.Enqueue(MakeNode(pendingWhiteSpace, ParsingNodeType.WhiteSpace));
                        MoveWhiteSpaceToOutput();
                        if (pendingOutput.Count > 0) output.Enqueue(MakeNode(pendingOutput, ParsingNodeType.Output));
                        output.Enqueue(item);
                        continue;
                }
            }
            if (pendingOutput.Count > 0) output.Enqueue(MakeNode(pendingOutput, ParsingNodeType.Output));
            return output;

            static ParsingNode MakeNode(Queue<ParsingNode> queue, ParsingNodeType nodeType)
            {
                var allTokens = queue.SelectMany(node => node.Tokens);
                var node = new ParsingNode(nodeType, allTokens);
                queue.Clear();
                return node;
            }
            void MoveWhiteSpaceToOutput()
            {
                pendingOutput.Enqueue(pendingWhiteSpace);
                pendingWhiteSpace.Clear();
            }
        }
    }
}
