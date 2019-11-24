using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Lexing;

namespace Obsidian.Parsing
{
    public static class Parser
    {
        public static IEnumerable<ParsingNode> Parse(IEnumerable<Token> source)
        {
            var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(source, 10);
            return Parse(enumerator);
        }
        private static IEnumerable<ParsingNode> Parse(ILookaroundEnumerator<Token> enumerator)
        {
            var canBeLineStatement = true; // Line statements must have only whitespace prior to it.
            while (enumerator.MoveNext())
            {
                switch (enumerator.Current.TokenType)
                {
                    case TokenTypes.LineStatement:
                        yield return LineStatementOrUnknown(canBeLineStatement, enumerator);
                        canBeLineStatement = false;
                        continue;
                    case TokenTypes.LineComment:
                        yield return new ParsingNode(ParsingNodeType.Comment, ReadWhile(enumerator, tokenType => tokenType != TokenTypes.NewLine));
                        continue;
                    case TokenTypes.StatementStart:
                        canBeLineStatement = false;
                        yield return new ParsingNode(ParsingNodeType.Statement, ReadUntil(enumerator, TokenTypes.StatementEnd));
                        continue;
                    case TokenTypes.CommentStart:
                        canBeLineStatement = false;
                        yield return new ParsingNode(ParsingNodeType.Comment, ReadUntil(enumerator, TokenTypes.CommentEnd));
                        continue;
                    case TokenTypes.ExpressionStart:
                        canBeLineStatement = false;
                        yield return new ParsingNode(ParsingNodeType.Expression, ReadUntil(enumerator, TokenTypes.ExpressionEnd));
                        continue;
                    case TokenTypes.NewLine:
                        yield return new ParsingNode(ParsingNodeType.NewLine, ReadOne(enumerator));
                        canBeLineStatement = true;
                        continue;
                    case TokenTypes.WhiteSpace:
                        var whiteSpaceNode = new ParsingNode(ParsingNodeType.WhiteSpace, ReadWhile(enumerator, tokenType => tokenType == TokenTypes.WhiteSpace));
                        if (!(canBeLineStatement && enumerator.TryGetNext(out var nextToken) && (nextToken.TokenType == TokenTypes.LineStatement || nextToken.TokenType == TokenTypes.LineComment)))
                        {
                            yield return whiteSpaceNode;
                        }
                        continue;
                    default:
                        canBeLineStatement = false;
                        yield return new ParsingNode(ParsingNodeType.Output, ReadOne(enumerator));
                        continue;
                }
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
        private static IEnumerable<Token> ReadUntil(ILookaroundEnumerator<Token> enumerator, params TokenTypes[] stopTokens)
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
        private static IEnumerable<Token> ReadWhile(ILookaroundEnumerator<Token> enumerator, Func<TokenTypes, bool> predicate)
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

        public static bool TryParseLineStatement(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ParsingNode? parsedNode)
        {
            parsedNode = default;
            if (enumerator.Current.TokenType != TokenTypes.LineStatement)
            {
                return false;
            }
            var continueLoop = true;
            var queue = new Queue<Token>();
            var nestingStack = new Stack<TokenTypes>();
            while (continueLoop)
            {
                if (enumerator.Current.TokenType == TokenTypes.NewLine && nestingStack.Count > 0)
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
                else if (nextToken.TokenType == TokenTypes.NewLine && nestingStack.Count == 0)
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
