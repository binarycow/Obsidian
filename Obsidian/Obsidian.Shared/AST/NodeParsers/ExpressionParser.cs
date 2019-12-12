using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Obsidian.WhiteSpaceControl;
using static Obsidian.AST.NodeParsers.ExpressionParser.ExpressionState;
using static Obsidian.Lexing.TokenType;

namespace Obsidian.AST.NodeParsers
{
    internal static class ExpressionParser
    {
        internal enum ExpressionState
        {
            StartJinja,
            WhiteSpaceOrExpression,
            Expression,
            WhiteSpaceOrEndJinja,
            EndJinja,
            Done,
        }
        internal static readonly Lazy<StateMachine<ExpressionState>> _StartBlockParser = new Lazy<StateMachine<ExpressionState>>(() => CreateParser());
        internal static StateMachine<ExpressionState> StartBlock => _StartBlockParser.Value;

        private static StateMachine<ExpressionState> CreateParser()
        {
            var parser = new StateMachine<ExpressionState>(StartJinja, Done);
            parser.State(StartJinja)
                .Expect(ExpressionStart)
                    .MoveTo(WhiteSpaceOrExpression)
                .Else()
                    .Return(false);
            parser.State(WhiteSpaceOrExpression)
                .Expect(Minus)
                    .SetWhiteSpaceMode(WhiteSpacePosition.Start, WhiteSpaceMode.Trim)
                    .MoveTo(Expression)
                .Expect(Plus)
                    .SetWhiteSpaceMode(WhiteSpacePosition.Start, WhiteSpaceMode.Keep)
                    .MoveTo(Expression)
                .Expect(WhiteSpace)
                    .MoveTo(Expression)
                .Else()
                    .Return(false);
            parser.State(Expression)
                .Expect(Minus).AndNext(StatementEnd)
                    .SetWhiteSpaceMode(WhiteSpacePosition.End, WhiteSpaceMode.Trim)
                    .MoveTo(EndJinja)
                .Expect(ExpressionEnd)
                    .MoveTo(Done)
                .Else()
                    .Accumulate();
            parser.State(EndJinja)
                .Expect(ExpressionEnd)
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
