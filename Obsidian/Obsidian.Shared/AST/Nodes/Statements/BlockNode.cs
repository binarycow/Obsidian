using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Common.Collections;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.Statements
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class BlockNode : AbstractContainerNode
    {

        public BlockNode(IEnumerable<ASTNode> children, WhiteSpaceControlMode startWhiteSpace, WhiteSpaceControlMode endWhiteSpace)
            : base(children, startWhiteSpace, endWhiteSpace)
        {
        }

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            throw new NotImplementedException();
        }

        public static bool TryParseBlock(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            return BlockNodeParser.TryParse(enumerator, out parsedNode);
        }


        private static class BlockNodeParser
        {
            public enum States
            {
                StartJinja,
                WhiteSpaceOrKeyword,
                Keyword,
                Name,
                EndJinja,
                Done,
            }


            public static bool TryParse(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
            {
                parsedNode = default;
                if(TryParseStartOrEnd(enumerator.Current,) == false)
                {
                    return false;
                }
                throw new NotImplementedException();
            }

            private static bool TryParseStart(ParsingNode current, out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace)
            {
                using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(current.Tokens);
                return TryParseStartOrEnd(enumerator, TokenTypes.Keyword_Block, out startWhiteSpace, out endWhiteSpace);
            }
            private static bool TryParseStartOrEnd(ILookaroundEnumerator<Token> enumerator, TokenTypes keywordType, out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace)
            {
                startWhiteSpace = WhiteSpaceControlMode.Default;
                endWhiteSpace = WhiteSpaceControlMode.Default;
                var state = States.StartJinja;
                var nameQueue = new Queue<Token>();
                while(enumerator.MoveNext())
                {
                    var token = enumerator.Current;
                    switch(state)
                    {
                        case States.StartJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.StatementStart:
                                    state = States.WhiteSpaceOrKeyword;
                                    continue;
                                default:
                                    return false;
                            }
                        case States.WhiteSpaceOrKeyword:
                            switch (token.TokenType)
                            {
                                case TokenTypes.WhiteSpace:
                                    state = States.Keyword;
                                    continue;
                                case TokenTypes.Minus:
                                case TokenTypes.Plus:
                                    startWhiteSpace = token.TokenType == TokenTypes.Minus ? WhiteSpaceControlMode.Trim : WhiteSpaceControlMode.Keep;
                                    state = States.Keyword;
                                    continue;
                                default:
                                    if (token.TokenType == keywordType)
                                    {
                                        state = States.Name;
                                        continue;
                                    }
                                    return false;
                            }
                        case States.Keyword:
                            if (token.TokenType == TokenTypes.WhiteSpace)
                            {
                                continue;
                            }
                            if (token.TokenType == keywordType)
                            {
                                state = States.Name;
                                continue;
                            }
                            return false;
                        case States.Name:
                            switch (token.TokenType)
                            {
                                case TokenTypes.Minus:
                                    if (enumerator.TryGetNext(out var nextItem) && nextItem.TokenType == TokenTypes.StatementEnd)
                                    {
                                        endWhiteSpace = WhiteSpaceControlMode.Trim;
                                        state = States.EndJinja;
                                        continue;
                                    }
                                    nameQueue.Enqueue(token);
                                    continue;
                                case TokenTypes.StatementEnd:
                                    state = States.Done;
                                    continue;
                                default:
                                    nameQueue.Enqueue(token);
                                    continue;
                            }
                        case States.EndJinja:
                            switch (token.TokenType)
                            {
                                case TokenTypes.StatementEnd:
                                    state = States.Done;
                                    continue;
                                default:
                                    throw new NotImplementedException();
                            }
                        case States.Done:
                            throw new NotImplementedException();
                        default:
                            throw new NotImplementedException();
                    }
                }
                throw new NotImplementedException();
            }
        }
    }
}
