using System;
using System.Collections.Generic;
using System.Text;
using static Obsidian.AST.NodeParsers.RawParser.RawState;
using static Obsidian.Lexing.TokenType;
using Obsidian.Lexing;
using System.Globalization;

namespace Obsidian.AST.NodeParsers
{
    internal class RawParser
    {
        public enum RawState
        {
            StartJinja,
            Keyword,
            EndJinja,
            Done
        }

        public static readonly Lazy<StateMachine<RawState>> _StartBlockParser = new Lazy<StateMachine<RawState>>(() => CreateParser(Keyword_Raw));
        public static StateMachine<RawState> StartBlock => _StartBlockParser.Value;
        public static readonly Lazy<StateMachine<RawState>> _EndBlockParser = new Lazy<StateMachine<RawState>>(() => CreateParser(Keyword_EndRaw));
        public static StateMachine<RawState> EndBlock => _EndBlockParser.Value;

        private static StateMachine<RawState> CreateParser(TokenType keyword)
        {
            var parser = new StateMachine<RawState>(StartJinja, Done);
            parser.State(StartJinja)
                .Expect(StatementStart)
                    .MoveTo(Keyword)
                .Else()
                    .Return(false);
            parser.State(Keyword)
                .Ignore(WhiteSpace)
                .Expect(keyword)
                    .MoveTo(EndJinja)
                .Else()
                    .Return(false);
            parser.State(EndJinja)
                .Ignore(WhiteSpace)
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
