using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using static Obsidian.Lexing.TokenType;
using static Obsidian.AST.NodeParsers.FromParser.FromState;
using static Obsidian.AST.NodeParsers.FromParser.AsState;

namespace Obsidian.AST.NodeParsers
{
    internal static class FromParser
    {
        internal static readonly Lazy<StateMachine<FromState>> _Parser = new Lazy<StateMachine<FromState>>(() => CreateParser());
        internal static StateMachine<FromState> Parser => _Parser.Value;
        internal static readonly Lazy<StateMachine<AsState>> _AsParser = new Lazy<StateMachine<AsState>>(() => CreateAsParser());
        internal static StateMachine<AsState> AsParser => _AsParser.Value;

        internal enum FromState
        {
            StartJinja,
            Keyword,
            Template,
            Import,
            Done,
        }

        private static StateMachine<FromState> CreateParser()
        {
            var parser = new StateMachine<FromState>(StartJinja, Done);
            parser.State(StartJinja)
                .Expect(StatementStart)
                    .MoveTo(Keyword)
                .Else()
                    .Return(false);
            parser.State(Keyword)
                .Ignore(WhiteSpace)
                .Expect(Keyword_From)
                    .MoveTo(Template)
                .Else()
                    .Return(false);
            parser.State(Template)
                .Expect(Keyword_Import)
                    .MoveTo(Import)
                .Else()
                    .Accumulate();
            parser.State(Import)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Else()
                    .Accumulate(seperator: Comma);
            parser.State(Done)
                .Throw();
            parser.Else()
                .Throw(new Exception(
                    ObsidianStrings.ResourceManager.GetString("ASTStateMachineError_UnhandledState",
                    CultureInfo.InvariantCulture)
                ));
            return parser;
        }


        internal enum AsState
        {
            MacroName,
            AliasName,
        }


        private static StateMachine<AsState> CreateAsParser()
        {
            var parser = new StateMachine<AsState>(MacroName);
            parser.State(MacroName)
                .Expect(Keyword_As)
                    .MoveTo(AliasName)
                .Else()
                    .Accumulate();
            parser.State(AliasName)
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
