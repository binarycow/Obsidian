using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Obsidian.Lexing;
using static Obsidian.AST.NodeParsers.ImportParser.ImportState;
using static Obsidian.Lexing.TokenType;

namespace Obsidian.AST.NodeParsers
{
    internal static class ImportParser
    {

        internal static readonly Lazy<StateMachine<ImportState>> _Parser = new Lazy<StateMachine<ImportState>>(() => CreateParser());
        internal static StateMachine<ImportState> Parser => _Parser.Value;

        internal enum ImportState
        {
            StartJinja,
            Keyword,
            FromDefinition,
            ImportDefinition,
            AsDefinition,
            Done
        }
        private static StateMachine<ImportState> CreateParser()
        {
            var parser = new StateMachine<ImportState>(StartJinja, Done);
            parser.State(StartJinja)
                .Expect(StatementStart)
                    .MoveTo(Keyword)
                .Else()
                    .Return(false);
            parser.State(Keyword)
                .Ignore(WhiteSpace)
                .Expect(Keyword_From)
                    .MoveTo(FromDefinition)
                .Expect(Keyword_Import)
                    .MoveTo(ImportDefinition)
                .Else()
                    .Return(false);
            parser.State(FromDefinition)
                .Expect(Keyword_Import)
                    .MoveTo(ImportDefinition)
                .Else()
                    .Accumulate();
            parser.State(ImportDefinition)
                .Expect(Keyword_As)
                    .MoveTo(AsDefinition)
                .Else()
                    .Accumulate();
            parser.State(AsDefinition)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Else()
                    .Accumulate();
            parser.State(Done)
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
