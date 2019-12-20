using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using ExpressionParser;
using ExpressionParser.Configuration;
using ExpressionParser.Exceptions;
using ExpressionParser.Lexing;
using ExpressionParser.Parsing;
using static Obsidian.JinjaLanguageDefinition;
using static ExpressionParser.Lexing.TokenType;
using System.Globalization;

namespace Obsidian.ExpressionParserExt
{
    internal class JinjaParser : Parser
    {
        internal JinjaParser(JinjaLanguageDefinition languageDefinition) : base(languageDefinition, 1, 1)
        {

        }

        internal override TryParseDelegate[] CustomParseDelegates => new TryParseDelegate[]
        {
            TryParseList,
            TryParseTuple,
            TryParseDictionary,
        };

        private bool TryParseCommaSeperatedSet(ILookaroundEnumerator<Token> enumerator, TokenType startTokenType, string? startTokenText, TokenType endTokenType, [NotNullWhen(true)]out IEnumerable<ASTNode>? parsedNodes, int minimumItems, AssignmentOperatorBehavior assignmentOperatorBehavior, bool requireDanglingCommaForOneItem = true)
        {
            parsedNodes = default;
            if (enumerator.Current.TokenType != startTokenType)
            {
                return false;
            }
            if(startTokenText != null && enumerator.Current.TextValue != startTokenText)
            {
                return false;
            }

            if (enumerator.TryGetPrevious(out var prevToken) == true)
            {
                if(prevToken.TokenType.IsTerminal())
                {
                    return false;
                }
            }
            enumerator.MoveNext();
            var queue = new Queue<ASTNode>();
            while (TryParse(enumerator, out var listItem, assignmentOperatorBehavior))
            {
                queue.Enqueue(listItem);
                if (enumerator.Current.TokenType == endTokenType)
                {
                    break;
                }
                if(enumerator.Current.TokenType != TokenType.Comma)
                {
                    throw new NotImplementedException();
                }
                if (enumerator.MoveNext() == false)
                {
                    throw new ParseException(ExpressionParserStrings.ResourceManager.GetString("ParsingError_UnterminatedCollectionLiteral", CultureInfo.InvariantCulture));
                }
            }

            if(queue.Count == 1 && minimumItems == 1)
            {
                if(enumerator.TryGetPrevious(out prevToken) == false)
                {
                    throw new NotImplementedException();
                }
                if(requireDanglingCommaForOneItem && prevToken.TokenType != TokenType.Comma)
                {
                    return false;
                }
            }
            if(queue.Count < minimumItems)
            {
                throw new NotImplementedException();
            }

            parsedNodes = queue;
            return true;
        }


        internal bool TryParseList(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            var backtrackID = enumerator.StartBacktrackSession();
            if (TryParseCommaSeperatedSet(enumerator, TokenType.Operator, _OPERATOR_SQUARE_BRACE_OPEN, SquareBraceClose, out var parsedListItems, minimumItems: 0, assignmentOperatorBehavior))
            {
                parsedNode = new ListNode(parsedListItems);
                enumerator.CommitBacktrackSession(backtrackID);
                return true;
            }
            enumerator.ResetBacktrackSession(backtrackID);
            parsedNode = default;
            return false;
        }



        internal bool TryParseTuple(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            var backtrackID = enumerator.StartBacktrackSession();
            if (TryParseCommaSeperatedSet(enumerator, TokenType.Operator, _OPERATOR_PAREN_OPEN, ParenClose, out var parsedListItems, minimumItems: 1, assignmentOperatorBehavior))
            {
                parsedNode = new TupleNode(parsedListItems);
                enumerator.CommitBacktrackSession(backtrackID);
                return true;
            }
            enumerator.ResetBacktrackSession(backtrackID);
            parsedNode = default;
            return false;
        }



        internal bool TryParseDictionary(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            parsedNode = default;
            if (enumerator.Current.TokenType != TokenType.CurlyBraceOpen)
            {
                return false;
            }

            var backtrackID = enumerator.StartBacktrackSession();
            var dictionaryItems = new Queue<DictionaryItemNode>();

            while(enumerator.MoveNext() && TryParseDictionaryItem(enumerator, out var dictionaryItem, assignmentOperatorBehavior))
            {
                dictionaryItems.Enqueue(dictionaryItem);
                if (enumerator.MoveNext() == false) throw new NotImplementedException();
                if(enumerator.Current.TokenType != TokenType.Comma)
                {
                    break;
                }
            }

            if(enumerator.State == EnumeratorState.Complete || enumerator.Current.TokenType != TokenType.CurlyBraceClose)
            {
                throw new NotImplementedException();
            }
            parsedNode = new DictionaryNode(dictionaryItems);
            enumerator.CommitBacktrackSession(backtrackID);
            return true;
        }

        private bool TryParseDictionaryItem(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out DictionaryItemNode? dictionaryItem, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            dictionaryItem = default;
            if(TryParse(enumerator, out var key, assignmentOperatorBehavior) == false || key == default)
            {
                return false;
            }
            if (enumerator.MoveNext() == false) throw new NotImplementedException();
            if (enumerator.Current.TokenType != TokenType.Colon) throw new NotImplementedException();
            if (enumerator.MoveNext() == false) throw new NotImplementedException();
            if (TryParse(enumerator, out var value, assignmentOperatorBehavior) == false || value == default) throw new NotImplementedException();
            dictionaryItem = new DictionaryItemNode(key, value);
            return true;
        }
    }
}
