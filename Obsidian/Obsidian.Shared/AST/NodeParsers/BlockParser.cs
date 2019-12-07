using Obsidian.Lexing;
using Obsidian.WhiteSpaceControl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static Obsidian.AST.NodeParsers.BlockParser.BlockState;
using static Obsidian.Lexing.TokenType;

namespace Obsidian.AST.NodeParsers
{
    internal static class BlockParser
    {
        public enum BlockState
        {
            StartJinja,
            WhiteSpaceOrKeyword,
            Keyword,
            BlockName,
            WhiteSpaceOrEndJinja,
            EndJinja,
            Done,
        }


        public static readonly Lazy<StateMachine<BlockState>> _StartBlockParser = new Lazy<StateMachine<BlockState>>(() => CreateParser(Keyword_Block));
        public static StateMachine<BlockState> StartBlock => _StartBlockParser.Value;
        public static readonly Lazy<StateMachine<BlockState>> _EndBlockParser = new Lazy<StateMachine<BlockState>>(() => CreateParser(Keyword_EndBlock));
        public static StateMachine<BlockState> EndBlock => _EndBlockParser.Value;

        private static StateMachine<BlockState> CreateParser(TokenType keyword)
        {
            var parser = new StateMachine<BlockState>(StartJinja, Done);
            parser.State(StartJinja)
                .Expect(StatementStart)
                    .MoveTo(WhiteSpaceOrKeyword)
                .Else()
                    .Return(false);
            parser.State(WhiteSpaceOrKeyword)
                .Expect(Minus)
                    .SetWhiteSpaceMode(WhiteSpacePosition.Start, WhiteSpaceMode.Trim)
                    .MoveTo(Keyword)
                .Expect(Plus)
                    .SetWhiteSpaceMode(WhiteSpacePosition.Start, WhiteSpaceMode.Keep)
                    .MoveTo(Keyword)
                .Expect(WhiteSpace)
                    .MoveTo(Keyword)
                .Else()
                    .Return(false);
            parser.State(Keyword)
                .Ignore(WhiteSpace)
                .Expect(keyword)
                    .MoveTo(BlockName)
                .Else()
                    .Return(false);
            parser.State(BlockName)
                .Expect(Minus).AndNext(StatementEnd)
                    .SetWhiteSpaceMode(WhiteSpacePosition.End, WhiteSpaceMode.Trim)
                    .MoveTo(EndJinja)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Else()
                    .Accumulate();
            parser.State(EndJinja)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Else()
                    .Accumulate();
            parser.Else()
                .Throw(new Exception(
                    ObsidianStrings.ResourceManager.GetString("ASTStateMachineError_UnhandledState", 
                    CultureInfo.InvariantCulture)
                ));
            return parser;
        }


    }
}
