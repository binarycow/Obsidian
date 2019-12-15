using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Lexing;

namespace Obsidian.Parsing
{
    internal static class Parser
    {
        internal static IEnumerable<ParsingNode> Parse(IEnumerable<Token> source)
        {
            using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(source, 10);
            var pendingOutput = new Queue<Token>();
            var canBeLineStatement = true; // Line statements must have only whitespace prior to it.
            while (enumerator.MoveNext())
            {
                ParsingNode pendingNode;
                switch (enumerator.Current.TokenType)
                {
                    case TokenType.LineStatement:
                        pendingNode = LineStatementOrUnknown(canBeLineStatement, enumerator);
                        canBeLineStatement = false;
                        break;
                    case TokenType.LineComment:
                        pendingNode = new ParsingNode(ParsingNodeType.Comment, ReadWhile(enumerator, tokenType => tokenType != TokenType.NewLine));
                        break;
                    case TokenType.StatementStart:
                        canBeLineStatement = false;
                        pendingNode = new ParsingNode(ParsingNodeType.Statement, ReadUntil(enumerator, TokenType.StatementEnd));
                        break;
                    case TokenType.CommentStart:
                        canBeLineStatement = false;
                        pendingNode = new ParsingNode(ParsingNodeType.Comment, ReadUntil(enumerator, TokenType.CommentEnd));
                        break;
                    case TokenType.ExpressionStart:
                        canBeLineStatement = false;
                        pendingNode = new ParsingNode(ParsingNodeType.Expression, ReadUntil(enumerator, TokenType.ExpressionEnd));
                        break;
                    case TokenType.NewLine:
                        pendingNode = new ParsingNode(ParsingNodeType.NewLine, ReadOne(enumerator));
                        canBeLineStatement = true;
                        break;
                    case TokenType.WhiteSpace:
                        var whiteSpaceNode = new ParsingNode(ParsingNodeType.WhiteSpace, ReadWhile(enumerator, tokenType => tokenType == TokenType.WhiteSpace));
                        if (!(canBeLineStatement && enumerator.TryGetNext(out var nextToken) && (nextToken.TokenType == TokenType.LineStatement || nextToken.TokenType == TokenType.LineComment)))
                        {
                            pendingNode = whiteSpaceNode;
                            break;
                        }
                        continue;
                    default:
                        canBeLineStatement = false;
                        pendingOutput.Enqueue(enumerator.Current);
                        continue;
                }
                if(pendingOutput.Count > 0)
                {
                    yield return new ParsingNode(ParsingNodeType.Output, pendingOutput);
                    pendingOutput.Clear();
                }
                yield return pendingNode;
            }
            if (pendingOutput.Count > 0)
            {
                yield return new ParsingNode(ParsingNodeType.Output, pendingOutput);
            }
        }
        private static ParsingNode LineStatementOrUnknown(bool canBeLineStatement, ILookaroundEnumerator<Token> enumerator)
        {
            return (canBeLineStatement && TryParseLineStatement(enumerator, out var lineStatementNode)) ?
                lineStatementNode :
                new ParsingNode(ParsingNodeType.Output, ReadOne(enumerator));
        }
        private static IEnumerable<Token> ReadOne(ILookaroundEnumerator<Token> enumerator)
        {
            yield return enumerator.Current;
        }
        private static IEnumerable<Token> ReadUntil(ILookaroundEnumerator<Token> enumerator, params TokenType[] stopTokens)
        {
            do
            {
                yield return enumerator.Current;
                if (stopTokens.Contains(enumerator.Current.TokenType))
                {
                    yield break;
                }
            } while (enumerator.MoveNext());
        }
        private static IEnumerable<Token> ReadWhile(ILookaroundEnumerator<Token> enumerator, Func<TokenType, bool> predicate)
        {
            yield return enumerator.Current;
            while (enumerator.TryGetNext(out var nextToken) && predicate(nextToken.TokenType))
            {
                var moveNext = enumerator.MoveNext();
                yield return enumerator.Current;
                if (moveNext == false)
                {
                    yield break;
                }
            }
        }

        internal static bool TryParseLineStatement(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ParsingNode? parsedNode)
        {
            parsedNode = default;
            if (enumerator.Current.TokenType != TokenType.LineStatement)
            {
                return false;
            }
            var continueLoop = true;
            var queue = new Queue<Token>();
            var nestingStack = new Stack<TokenType>();
            while (continueLoop)
            {
                if (enumerator.Current.TokenType == TokenType.NewLine && nestingStack.Count > 0)
                {
                    enumerator.MoveNext();
                    continue;
                }
                else if (enumerator.Current.TokenType.IsOpeningBrace())
                {
                    nestingStack.Push(enumerator.Current.TokenType);
                }
                else if (enumerator.Current.TokenType.IsClosingBrace())
                {
                    if (nestingStack.Peek().IsMatchingBrace(enumerator.Current.TokenType))
                    {
                        nestingStack.Pop();
                    }
                    else
                    {
                        throw new NotImplementedException(); //Unbalanced stack
                    }
                }

                if (enumerator.TryGetNext(out var nextToken) == false)
                {
                    continueLoop = false;
                }
                else if (nextToken.TokenType == TokenType.NewLine && nestingStack.Count == 0)
                {
                    continueLoop = false;
                }

                queue.Enqueue(enumerator.Current);
                enumerator.MoveNext();
            }
            if (nestingStack.Count != 0)
            {
                throw new NotImplementedException();
            }
            parsedNode = new ParsingNode(ParsingNodeType.Statement, queue);
            return true;
        }
    }

}
