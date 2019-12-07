using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.Lexing;
using Obsidian.WhiteSpaceControl;
using static Obsidian.AST.NodeParsers.IfParser.IfState;
using static Obsidian.Lexing.TokenType;
using System.Globalization;

namespace Obsidian.AST.NodeParsers
{
    internal static class IfParser
    {
        public enum IfState
        {
            StartJinja,
            WhiteSpaceOrKeyword,
            Keyword,
            Expression,
            WhiteSpaceOrEndJinja,
            EndJinja,
            Done,
        }

        public static readonly Lazy<StateMachine<IfState>> _StartBlockParser = new Lazy<StateMachine<IfState>>(() => CreateStartElseIfParser(Keyword_If));
        public static StateMachine<IfState> StartBlock => _StartBlockParser.Value;
        public static readonly Lazy<StateMachine<IfState>> _ElseIfBlockParser = new Lazy<StateMachine<IfState>>(() => CreateStartElseIfParser(Keyword_Elif));
        public static StateMachine<IfState> ElseIfBlock => _ElseIfBlockParser.Value;
        public static readonly Lazy<StateMachine<IfState>> _ElseBlockParser = new Lazy<StateMachine<IfState>>(() => CreateElseEndForParser(Keyword_Else));
        public static StateMachine<IfState> ElseBlock => _ElseBlockParser.Value;
        public static readonly Lazy<StateMachine<IfState>> _EndBlockParser = new Lazy<StateMachine<IfState>>(() => CreateElseEndForParser(Keyword_Endif));
        public static StateMachine<IfState> EndBlock => _EndBlockParser.Value;

        private static StateMachine<IfState> CreateStartElseIfParser(TokenType keyword)
        {
            var parser = new StateMachine<IfState>(StartJinja, Done);
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
                    .MoveTo(Expression)
                .Else()
                    .Return(false);
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
                    .Throw();
            parser.Else()
                .Throw(new Exception(
                    ObsidianStrings.ResourceManager.GetString("ASTStateMachineError_UnhandledState",
                    CultureInfo.InvariantCulture)
                ));
            return parser;
        }

        private static StateMachine<IfState> CreateElseEndForParser(TokenType keyword)
        {
            var parser = new StateMachine<IfState>(StartJinja, Done);
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
                    .Throw();
            parser.Else()
                .Throw(new Exception(
                    ObsidianStrings.ResourceManager.GetString("ASTStateMachineError_UnhandledState",
                    CultureInfo.InvariantCulture)
                ));
            return parser;
        }
    }
}