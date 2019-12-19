using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Obsidian.Lexing;
using static Obsidian.Lexing.TokenType;
using static Obsidian.AST.NodeParsers.ImportParser.ImportState;

namespace Obsidian.AST.NodeParsers
{
    internal static class ImportParser
    {
        internal static readonly Lazy<StateMachine<ImportState>> _ParserImport = new Lazy<StateMachine<ImportState>>(() => CreateImportParser());
        internal static StateMachine<ImportState> ParserImport => _ParserImport.Value;

        internal enum ImportState
        {
            StartJinja,
            Keyword,
            Template,
            As,
            Done,
        }

        private static StateMachine<ImportState> CreateImportParser()
        {
            var parser = new StateMachine<ImportState>(ImportState.StartJinja, ImportState.Done);
            parser.State(ImportState.StartJinja)
                .Expect(StatementStart)
                    .MoveTo(ImportState.Keyword)
                .Else()
                    .Return(false);
            parser.State(ImportState.Keyword)
                .Ignore(WhiteSpace)
                .Expect(Keyword_Import)
                    .MoveTo(ImportState.Template)
                .Else()
                    .Return(false);
            parser.State(ImportState.Template)
                .Expect(Keyword_As)
                    .MoveTo(ImportState.As)
                .Else()
                    .Accumulate();
            parser.State(ImportState.As)
                .Expect(StatementEnd)
                    .MoveTo(ImportState.Done)
                .Else()
                    .Accumulate();
            parser.State(ImportState.Done)
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
