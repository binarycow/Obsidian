using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.Lexing;
using Obsidian.WhiteSpaceControl;
using static Obsidian.AST.NodeParsers.ForParser.ForState;
using static Obsidian.Lexing.TokenType;
using System.Globalization;

namespace Obsidian.AST.NodeParsers
{
    internal static class ForParser
    {
        internal enum ForState
        {
            StartJinja,
            WhiteSpaceOrKeyword,
            Keyword,
            VariableNames,
            Expression,
            Filter,
            WhiteSpaceOrEndJinja,
            EndJinja,
            Done,
        }

        internal static readonly Lazy<StateMachine<ForState>> _StartBlockParser = new Lazy<StateMachine<ForState>>(() => CreateStartParser());
        internal static StateMachine<ForState> StartBlock => _StartBlockParser.Value;
        internal static readonly Lazy<StateMachine<ForState>> _ElseBlockParser = new Lazy<StateMachine<ForState>>(() => CreateElseEndForParser(TokenType.Keyword_Else));
        internal static StateMachine<ForState> ElseBlock => _ElseBlockParser.Value;
        internal static readonly Lazy<StateMachine<ForState>> _EndBlockParser = new Lazy<StateMachine<ForState>>(() => CreateElseEndForParser(TokenType.Keyword_EndFor));
        internal static StateMachine<ForState> EndBlock => _EndBlockParser.Value;

        private static StateMachine<ForState> CreateStartParser()
        {
            var parser = new StateMachine<ForState>(StartJinja, Done);
            parser.Set("recursive", false);
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
                .Expect(Keyword_For)
                    .MoveTo(VariableNames)
                .Else()
                    .Return(false);
            parser.State(VariableNames)
                .Expect(Keyword_In)
                    .MoveTo(Expression)
                .Else()
                    .Accumulate(seperator: Comma);
            parser.State(Expression)
                .Expect(Minus).AndNext(StatementEnd)
                    .SetWhiteSpaceMode(WhiteSpacePosition.End, WhiteSpaceMode.Trim)
                    .MoveTo(EndJinja)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Expect(Keyword_If)
                    .MoveTo(Filter)
                .Expect(Keyword_Recursive)
                    .Set("recursive", true)
                    .MoveTo(EndJinja)
                .Else()
                    .Accumulate();
            parser.State(Filter)
                .Expect(Minus).AndNext(StatementEnd)
                    .SetWhiteSpaceMode(WhiteSpacePosition.End, WhiteSpaceMode.Trim)
                    .MoveTo(EndJinja)
                .Expect(Keyword_Recursive)
                    .Set("recursive", true)
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
        private static StateMachine<ForState> CreateElseEndForParser(TokenType keyword)
        {
            var parser = new StateMachine<ForState>(StartJinja, Done);
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
                    .MoveTo(WhiteSpaceOrEndJinja)
                .Else()
                    .Return(false);
            parser.State(WhiteSpaceOrEndJinja)
                .Ignore(WhiteSpace)
                .Expect(Minus).AndNext(StatementEnd)
                    .SetWhiteSpaceMode(WhiteSpacePosition.End, WhiteSpaceMode.Trim)
                    .MoveTo(EndJinja)
                .Expect(StatementEnd)
                    .MoveTo(Done);
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
