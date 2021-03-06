using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Lexing;

namespace Obsidian.AST.NodeParsers
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class State<TState> where TState : struct, Enum
    {
        internal string DebuggerDisplay
        {
            get
            {
                var sb = new StringBuilder();
                foreach(var expect in _Tokens.Keys)
                {
                    sb.AppendLine($".Expect({expect})");
                    foreach(var conditional in _Tokens[expect])
                    {
                        foreach (var line in conditional.DebuggerDisplay.Split(Environment.NewLine).Select(line => $"     {line}"))
                        {
                            sb.AppendLine(line);
                        }
                    }
                }
                return sb.ToString();
            }
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        private class ConditionalAction
        {

            internal string DebuggerDisplay
            {
                get
                {
                    if(PredicateDebuggerDisplay != null)
                    {
                        return $"{PredicateDebuggerDisplay}{Environment.NewLine}     {Action.DebuggerDisplay}";
                    }
                    else
                    {
                        return Action.DebuggerDisplay;
                    }
                }
            }

            internal ConditionalAction(StateAction<TState> action, ConditionalDelegate? predicate = null)
            {
                Action = action;
                Predicate = predicate ?? (enumerator => true);
            }
            internal ConditionalDelegate Predicate { get; set; }
            internal string? PredicateDebuggerDisplay { get; set; }
            internal StateAction<TState> Action { get; }
        }

        internal State(StateMachine<TState> parent, TState? stateEnum)
        {
            Parent = parent;
            StateEnum = stateEnum;
        }

        internal delegate bool ConditionalDelegate(ILookaroundEnumerator<Token> enumerator);

        internal TState? StateEnum { get; }
        internal StateMachine<TState> Parent { get; }
        private readonly Dictionary<TokenType, List<ConditionalAction>> _Tokens = new Dictionary<TokenType, List<ConditionalAction>>();
        private StateAction<TState>? _ElseAction = default;
        private StateAction<TState>? _ThrowAction = default;

        internal void SetPredicate(StateAction<TState> action, ConditionalDelegate predicate, string predicateDebuggerDisplay)
        {
            var foundAction = _Tokens.Values.SelectMany(cond => cond).FirstOrDefault(listAction => listAction.Action == action);
            if (foundAction == default) throw new NotImplementedException();
            foundAction.Predicate = predicate;
            foundAction.PredicateDebuggerDisplay = predicateDebuggerDisplay;
        }

        internal StateAction<TState> Expect(TokenType tokenType)
        {
            var action = new StateAction<TState>(this);
            _Tokens.Add(tokenType, new List<ConditionalAction>
            {
                new ConditionalAction(action)
            });
            return action;
        }
        internal StateAction<TState> Ignore(TokenType tokenType)
        {
            var action = new StateAction<TState>(this);
            _Tokens.Add(tokenType, new List<ConditionalAction>
            {
                new ConditionalAction(action)
            });
            action.Ignore();
            return action;
        }

        internal StateAction<TState> Else()
        {
            if (_ElseAction != default) throw new NotImplementedException();
            _ElseAction = new StateAction<TState>(this);
            return _ElseAction;
        }

        internal StateAction<TState> Throw(Exception? exception = null)
        {
            if (_ThrowAction != default) throw new NotImplementedException();
            _ThrowAction = new StateAction<TState>(this);
            _ThrowAction.Throw(exception);
            return _ThrowAction;
        }

        internal bool TryParse(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out StateAction<TState>? action)
        {
            action = default;
            if(_ThrowAction != default)
            {
                action = _ThrowAction;
                return true;
            }
            if(_Tokens.TryGetValue(enumerator.Current.TokenType, out var actionConditions))
            {
                foreach(var actionCondition in actionConditions)
                {
                    if(actionCondition.Predicate(enumerator))
                    {
                        action = actionCondition.Action;
                        return true;
                    }
                }
            }
            if(_ElseAction != default)
            {
                action = _ElseAction;
                return true;
            }
            return false;
        }
    }
}
