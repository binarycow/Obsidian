using System;
using System.Collections.Generic;
using System.Text;
using static Obsidian.AST.NodeParsers.MacroParser.MacroState;
using static Obsidian.Lexing.TokenType;
using Obsidian.Lexing;
using Obsidian.WhiteSpaceControl;
using System.Globalization;

namespace Obsidian.AST.NodeParsers
{
    internal class MacroParser
    {
        internal enum MacroState
        {
            StartJinja,
            WhiteSpaceOrKeyword,
            Keyword,
            MacroDefinition,
            WhiteSpaceOrEndJinja,
            EndJinja,
            Done
        }

        internal static readonly Lazy<StateMachine<MacroState>> _StartBlockParser = new Lazy<StateMachine<MacroState>>(() => CreateStartParser());
        internal static StateMachine<MacroState> StartBlock => _StartBlockParser.Value;
        internal static readonly Lazy<StateMachine<MacroState>> _EndBlockParser = new Lazy<StateMachine<MacroState>>(() => CreateEndParser());
        internal static StateMachine<MacroState> EndBlock => _EndBlockParser.Value;

        private static StateMachine<MacroState> CreateStartParser()
        {
            var parser = new StateMachine<MacroState>(StartJinja, Done);
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
                .Expect(Keyword_Macro)
                    .MoveTo(MacroDefinition)
                .Else()
                    .Return(false);
            parser.State(MacroDefinition)
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
        private static StateMachine<MacroState> CreateEndParser()
        {
            var parser = new StateMachine<MacroState>(StartJinja, Done);
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
                .Expect(Keyword_EndMacro)
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
