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

namespace Obsidian.ExpressionParserExt
{
    public class JinjaParser : Parser
    {
        public JinjaParser(JinjaLanguageDefinition languageDefinition) : base(languageDefinition, 1, 1)
        {

        }

        public override TryParseDelegate[] CustomParseDelegates => new TryParseDelegate[]
        {
            TryParseList,
            TryParseTuple,
            TryParseDictionary,
        };

        private bool TryParseCommaSeperatedSet(ILookaroundEnumerator<Token> enumerator, TokenType startTokenType, string? startTokenText, TokenType endTokenType, [NotNullWhen(true)]out IEnumerable<ASTNode>? parsedNodes, int minimumItems, AssignmentOperatorBehavior assignmentOperatorBehavior)
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
                if (enumerator.Current.TokenType != TokenType.Comma)
                {
                    break;
                }
                if (enumerator.MoveNext() == false)
                {
                    throw new ParseException($"Unterminated collection literal");
                }
            }
            if(queue.Count < minimumItems)
            {
                throw new NotImplementedException();
            }

            if (enumerator.Current.TokenType != endTokenType)
            {
                throw new NotImplementedException();
            }
            parsedNodes = queue;
            return true;
        }


        public bool TryParseList(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            if (TryParseCommaSeperatedSet(enumerator, TokenType.Operator, OPERATOR_SQUARE_BRACE_OPEN, SquareBrace_Close, out var parsedListItems, minimumItems: 1, assignmentOperatorBehavior))
            {
                parsedNode = new ListNode(parsedListItems);
                return true;
            }
            parsedNode = default;
            return false;
        }



        public bool TryParseTuple(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            if (TryParseCommaSeperatedSet(enumerator, TokenType.Operator, OPERATOR_PAREN_OPEN, Paren_Close, out var parsedListItems, minimumItems: 2, assignmentOperatorBehavior))
            {
                parsedNode = new TupleNode(parsedListItems);
                return true;
            }
            parsedNode = default;
            return false;
        }



        public bool TryParseDictionary(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            parsedNode = default;
            if (enumerator.Current.TokenType != TokenType.CurlyBrace_Open)
            {
                return false;
            }

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

            if(enumerator.State == EnumeratorState.Complete || enumerator.Current.TokenType != TokenType.CurlyBrace_Close)
            {
                throw new NotImplementedException();
            }
            parsedNode = new DictionaryNode(dictionaryItems);
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
