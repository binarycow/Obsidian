using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Parsing;

namespace Obsidian.WhiteSpaceControl
{
    public static class EnvironmentTrimming
    {
        public static IEnumerable<ParsingNode> EnvironmentTrim(IEnumerable<ParsingNode> source, EnvironmentSettings settings)
        {
            // TODO: Remove these .ToArray()
            var trimmedSource = (settings.TrimBlocks ? TrimBlocks(source) : source).ToArrayWithoutInstantiation();
            var strippedSource = (settings.LStripBlocks ? StripBlocks(trimmedSource) : trimmedSource).ToArrayWithoutInstantiation();
            var trimmedTrailingNewline = (settings.TrimTrailingNewline ? TrimTrailingNewline(strippedSource) : strippedSource).ToArrayWithoutInstantiation();
            return trimmedTrailingNewline;
        }
        private static IEnumerable<ParsingNode> TrimTrailingNewline(IEnumerable<ParsingNode> source)
        {
            var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(source, 1);
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.NodeType != ParsingNodeType.NewLine)
                {
                    yield return enumerator.Current;
                    continue;
                }
                if (enumerator.TryGetNext(out _) == false)
                {
                    enumerator.Current.WhiteSpaceControlMode = WhiteSpaceControlMode.Trim;
                    yield return enumerator.Current;
                    continue;
                }
                else
                {
                    yield return enumerator.Current;
                }
            }
        }

        private static IEnumerable<ParsingNode> StripBlocks(IEnumerable<ParsingNode> source)
        {
            // TODO: We need using statements around this....  But - not just this one, *ALL* of them.  But, for some reason, that causes issues...  scope issue?
            var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(source, 0);
            while (enumerator.MoveNext())
            {
                switch (enumerator.Current.NodeType)
                {
                    case ParsingNodeType.NewLine:
                        yield return enumerator.Current;
                        var queuedItems = new Queue<ParsingNode>();
                        // Find the next block
                        bool continueLoop = true;
                        bool onlyWhiteSpace = true;
                        while (continueLoop && enumerator.MoveNext())
                        {
                            switch (enumerator.Current.NodeType)
                            {
                                case ParsingNodeType.WhiteSpace:
                                    queuedItems.Enqueue(enumerator.Current);
                                    continue;
                                case ParsingNodeType.Expression:
                                case ParsingNodeType.Statement:
                                    continueLoop = false;
                                    continue;
                                default:
                                    onlyWhiteSpace = false;
                                    continueLoop = false;
                                    continue;
                            }
                        }
                        foreach (var item in queuedItems)
                        {
                            item.WhiteSpaceControlMode = onlyWhiteSpace ? WhiteSpaceControlMode.Trim : item.WhiteSpaceControlMode;
                            yield return item;
                        }
                        break;
                }
                yield return enumerator.Current;
            }
        }
        private static IEnumerable<ParsingNode> TrimBlocks(IEnumerable<ParsingNode> source)
        {
            var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(source, 1);
            while (enumerator.MoveNext())
            {
                switch (enumerator.Current.NodeType)
                {
                    case ParsingNodeType.Statement:
                        if (enumerator.TryGetNext(out var nextItem))
                        {
                            switch (nextItem?.NodeType)
                            {
                                case ParsingNodeType.NewLine:
                                    yield return enumerator.Current;
                                    enumerator.MoveNext();
                                    enumerator.Current.WhiteSpaceControlMode = WhiteSpaceControlMode.Trim;
                                    yield return enumerator.Current;
                                    continue;
                            }
                        }
                        break;
                }
                yield return enumerator.Current;
            }
        }
    }
}
