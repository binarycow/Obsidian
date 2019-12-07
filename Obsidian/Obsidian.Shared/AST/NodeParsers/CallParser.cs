using Obsidian.Lexing;
using Obsidian.WhiteSpaceControl;
using System;
using System.Collections.Generic;
using System.Text;
using static Obsidian.AST.NodeParsers.CallParser.CallState;
using static Obsidian.Lexing.TokenType;
using System.Globalization;

namespace Obsidian.AST.NodeParsers
{
    internal static class CallParser
    {
        public enum CallState
        {
            StartJinja,
            WhiteSpaceOrKeyword,
            Keyword,
            CallDefinition,
            WhiteSpaceOrEndJinja,
            EndJinja,
            Done,
        }


        public static readonly Lazy<StateMachine<CallState>> _StartBlockParser = new Lazy<StateMachine<CallState>>(() => CreateParser(Keyword_Call));
        public static StateMachine<CallState> StartBlock => _StartBlockParser.Value;
        public static readonly Lazy<StateMachine<CallState>> _EndBlockParser = new Lazy<StateMachine<CallState>>(() => CreateParser(Keyword_EndCall));
        public static StateMachine<CallState> EndBlock => _EndBlockParser.Value;

        private static StateMachine<CallState> CreateParser(TokenType keyword)
        {
            var parser = new StateMachine<CallState>(StartJinja, Done);
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
                    .MoveTo(CallDefinition)
                .Else()
                    .Return(false);
            parser.State(CallDefinition)
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
