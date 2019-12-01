using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.Lexing;
using Obsidian.WhiteSpaceControl;
using static Obsidian.AST.NodeParsers.ForParser.ForState;
using static Obsidian.Lexing.TokenTypes;

namespace Obsidian.AST.NodeParsers
{
    internal static class ForParser
    {
        public enum ForState
        {
            StartJinja,
            WhiteSpaceOrKeyword,
            Keyword,
            VariableNames,
            Expression,
            WhiteSpaceOrEndJinja,
            EndJinja,
            Done,
        }

        public static readonly Lazy<StateMachine<ForState>> _StartBlockParser = new Lazy<StateMachine<ForState>>(() => CreateStartParser());
        public static StateMachine<ForState> StartBlock => _StartBlockParser.Value;
        public static readonly Lazy<StateMachine<ForState>> _ElseBlockParser = new Lazy<StateMachine<ForState>>(() => CreateElseEndForParser(TokenTypes.Keyword_Else));
        public static StateMachine<ForState> ElseBlock => _ElseBlockParser.Value;
        public static readonly Lazy<StateMachine<ForState>> _EndBlockParser = new Lazy<StateMachine<ForState>>(() => CreateElseEndForParser(TokenTypes.Keyword_EndFor));
        public static StateMachine<ForState> EndBlock => _EndBlockParser.Value;

        private static StateMachine<ForState> CreateStartParser()
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
                .Else()
                    .Accumulate();
            parser.State(EndJinja)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Else()
                    .Accumulate();
            parser.Else()
                .Throw(new Exception("Unhandled State?"));
            return parser;
        }
        private static StateMachine<ForState> CreateElseEndForParser(TokenTypes keyword)
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
                .Throw(new Exception("Unhandled State?"));
            return parser;
        }




    }
}
