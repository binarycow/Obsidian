using System;
using Obsidian.Lexing;
using System.Collections.Generic;
using System.Text;
using static Obsidian.AST.NodeParsers.IncludeParser.IncludeState;
using static Obsidian.Lexing.TokenType;
using System.Globalization;

namespace Obsidian.AST.NodeParsers
{
    internal class IncludeParser
    {
        internal enum IncludeState
        {
            StartJinja,
            Keyword,
            TemplateNames,
            Missing,
            WithWithout,
            Context,
            EndJinja,
            Done,
        }
        internal static readonly Lazy<StateMachine<IncludeState>> _Parser = new Lazy<StateMachine<IncludeState>>(() => CreateParser());
        internal static StateMachine<IncludeState> Parser => _Parser.Value;

        private static StateMachine<IncludeState> CreateParser()
        {
            var parser = new StateMachine<IncludeState>(StartJinja, Done);
            parser.State(StartJinja)
                .Expect(StatementStart)
                    .MoveTo(Keyword)
                .Else()
                    .Return(false);
            parser.State(Keyword)
                .Ignore(WhiteSpace)
                .Expect(Keyword_Include)
                    .MoveTo(TemplateNames)
                .Else()
                    .Return(false);
            parser.State(TemplateNames)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Expect(Keyword_Ignore)
                    .MoveTo(Missing)
                .Expect(Keyword_With)
                    .MoveTo(Context)
                .Else()
                    .Accumulate();
            parser.State(Missing)
                .Ignore(WhiteSpace)
                .Expect(Keyword_Missing)
                    .Set("ignoreMissing", true)
                    .MoveTo(WithWithout)
                .Else()
                    .Throw();
            parser.State(WithWithout)
                .Ignore(WhiteSpace)
                .Expect(Keyword_With)
                    .Set<bool?>("context", true)
                    .MoveTo(Context)
                .Expect(Keyword_Without)
                    .Set<bool?>("context", false)
                    .MoveTo(Context)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Else()
                    .Throw();
            parser.State(Context)
                .Ignore(WhiteSpace)
                .Expect(Keyword_Context)
                    .MoveTo(EndJinja)
                .Else()
                    .Throw();
            parser.State(EndJinja)
                .Ignore(WhiteSpace)
                .Expect(StatementEnd)
                    .MoveTo(Done)
                .Else()
                    .Accumulate();
            parser.Else()
                .Throw(new Exception(
                    ObsidianStrings.ResourceManager.GetString("ASTStateMachineError_UnhandledState", CultureInfo.InvariantCulture)
                ));
            return parser;
        }

    }
}
