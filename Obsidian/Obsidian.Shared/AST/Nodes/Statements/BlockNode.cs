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
            }


            public static bool TryParse(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
            {
                parsedNode = default;
                if(TryParseStart(enumerator.Current) == false)
                {
                    return false;
                }
                throw new NotImplementedException();
            }

            private static bool TryParseStart(ParsingNode current)
            {
                using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(current.Tokens);
                return TryParseStart(enumerator);
            }
            private static bool TryParseStart(ILookaroundEnumerator<Token> enumerator, out WhiteSpaceControlMode startWhiteSpace, out WhiteSpaceControlMode endWhiteSpace)
            {
                var state = States.StartJinja;
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
                                case TokenTypes.Keyword_If:
                                    state = States.Expression;
                                    continue;
                                default:
                                    return false;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }
                throw new NotImplementedException();
            }
        }
    }
}
