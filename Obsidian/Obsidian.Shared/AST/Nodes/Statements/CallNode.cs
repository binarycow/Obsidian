using Common;
using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using static Obsidian.Lexing.TokenType;

namespace Obsidian.AST.Nodes.Statements
{
    internal class CallNode : ASTNode, IWhiteSpaceControlling
    {
        internal CallNode(ParsingNode? startParsingNode, ExpressionNode callerDefinition, ExpressionNode macroCall, ContainerNode contents, ParsingNode? endParsingNode, WhiteSpaceControlSet? whiteSpace = null)
            : base(startParsingNode, contents.ParsingNodes, endParsingNode)
        {
            Contents = contents;
            WhiteSpaceControl = whiteSpace ?? new WhiteSpaceControlSet();
            MacroCall = macroCall;
            CallerDefinition = callerDefinition;
        }

        internal ContainerNode Contents { get; }
        internal ExpressionNode MacroCall { get; }
        internal ExpressionNode CallerDefinition { get; }
        public WhiteSpaceControlSet WhiteSpaceControl { get; }

        public override void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false)
        {
            visitor.Transform(this, inner);
        }
        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }

        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }

        internal static bool TryParseCall(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;


            if (CallParser.StartBlock.TryParse(enumerator.Current, out var outsideStart, out var insideStart) == false)
            {
                return false;
            }
            if (CallParser.StartBlock.TryGetAccumulation(CallParser.CallState.CallDefinition, 0, out var callDefinition) == false)
            {
                throw new NotImplementedException();
            }
            var startParsingNode = enumerator.Current;
            enumerator.MoveNext();
            var contents = ASTGenerator.ParseUntilFailure(environment, lexer, enumerator).ToArray();
            if (CallParser.EndBlock.TryParse(enumerator.Current, out var insideEnd, out var outsideEnd) == false)
            {
                return false;
            }
            var endParsingNode = enumerator.Current;
            var contentsNode = new ContainerNode(null, contents, null,
                new WhiteSpaceControlSet(insideStart, insideEnd)
            );

            if (TryParseCallDefinition(lexer, callDefinition, out var callArgumentList, out var macroCall) == false) throw new NotImplementedException();


            parsedNode = new CallNode(startParsingNode, 
                ExpressionNode.FromString(environment, callArgumentList), 
                ExpressionNode.FromString(environment, macroCall), contentsNode, endParsingNode,
                new WhiteSpaceControlSet(outsideStart, outsideEnd)
            );
            return true;
        }



        private static bool TryParseCallDefinition(Lexer lexer, string callDefinition, out string argumentList, out string macroCall)
        {
            argumentList = string.Empty;
            macroCall = string.Empty;

            using var argListCheckout = StringBuilderPool.Instance.Checkout();
            using var macroCallCheckout = StringBuilderPool.Instance.Checkout();
            var argumentListstringBuilder = argListCheckout.CheckedOutObject;
            var macroCallStringBuilder = macroCallCheckout.CheckedOutObject;
            var activeStringBuilder = argumentListstringBuilder;

            var nestingStack = new Stack<Token>();

            using var enumerator = lexer.Tokenize(callDefinition).GetEnumerator();
            if (enumerator.MoveNext() == false) throw new NotImplementedException();
            if (enumerator.Current.TokenType != Paren_Open) throw new NotImplementedException();

            do
            {
                var token = enumerator.Current;
                switch (token.TokenType)
                {
                    case SquareBrace_Open:
                    case CurlyBrace_Open:
                    case Paren_Open:
                        nestingStack.Push(token);
                        activeStringBuilder.Append(token.Value);
                        continue;
                    case SquareBrace_Close:
                    case CurlyBrace_Close:
                    case Paren_Close:
                        if (nestingStack.Peek().TokenType.IsMatchingBrace(token.TokenType) == false) throw new NotImplementedException();
                        nestingStack.Pop();
                        activeStringBuilder.Append(token.Value);
                        if(nestingStack.Count == 0 && activeStringBuilder == argumentListstringBuilder)
                        {
                            activeStringBuilder = macroCallStringBuilder;
                        }
                        continue;
                    default:
                        activeStringBuilder.Append(token.Value);
                        continue;
                }
            } while (enumerator.MoveNext());

            if (nestingStack.Count != 0) throw new NotImplementedException();

            argumentList = $"caller{argumentListstringBuilder.ToString().Trim()}";
            macroCall = macroCallStringBuilder.ToString().Trim();
            return true;
        }

    }

}
