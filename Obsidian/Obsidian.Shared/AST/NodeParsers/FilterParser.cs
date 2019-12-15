using Obsidian.Lexing;
using Obsidian.WhiteSpaceControl;
using System;
using System.Collections.Generic;
using System.Text;
using static Obsidian.AST.NodeParsers.FilterParser.FilterState;
using static Obsidian.Lexing.TokenType;
using System.Globalization;

namespace Obsidian.AST.NodeParsers
{
    internal static class FilterParser
    {
        internal enum FilterState
        {
            StartJinja,
            WhiteSpaceOrKeyword,
            Keyword,
            FilterName,
            WhiteSpaceOrEndJinja,
            EndJinja,
            Done,
        }


        internal static readonly Lazy<StateMachine<FilterState>> _StartBlockParser = new Lazy<StateMachine<FilterState>>(() => CreateStartParser());
        internal static StateMachine<FilterState> StartBlock => _StartBlockParser.Value;
        internal static readonly Lazy<StateMachine<FilterState>> _EndBlockParser = new Lazy<StateMachine<FilterState>>(() => CreateEndParser());
        internal static StateMachine<FilterState> EndBlock => _EndBlockParser.Value;

        private static StateMachine<FilterState> CreateStartParser()
        {
            var parser = new StateMachine<FilterState>(StartJinja, Done);
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
                .Expect(Keyword_Filter)
                    .MoveTo(FilterName)
                .Else()
                    .Return(false);
            parser.State(FilterName)
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
        private static StateMachine<FilterState> CreateEndParser()
        {
            var parser = new StateMachine<FilterState>(StartJinja, Done);
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
                .Expect(Keyword_EndFilter)
                    .MoveTo(WhiteSpaceOrEndJinja)
                .Else()
                    .Return(false);
            parser.State(WhiteSpaceOrEndJinja)
                .Ignore(WhiteSpace)
                .Expect(Minus).AndNext(StatementEnd)
                    .SetWhiteSpaceMode(WhiteSpacePosition.End, WhiteSpaceMode.Trim)
                    .MoveTo(EndJinja)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Else()
                    .Throw();
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
