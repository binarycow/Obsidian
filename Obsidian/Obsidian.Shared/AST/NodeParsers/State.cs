﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Lexing;

namespace Obsidian.AST.NodeParsers
{
    internal class State<TState> where TState : struct, Enum
    {
        private class ConditionalAction
        {
            public ConditionalAction(StateAction<TState> action, ConditionalDelegate? predicate = null)
            {
                Action = action;
                Predicate = predicate ?? (enumerator => true);
            }
            public ConditionalDelegate Predicate { get; set; }
            public StateAction<TState> Action { get; }
        }

        public State(StateMachine<TState> parent, TState? stateEnum)
        {
            Parent = parent;
            StateEnum = stateEnum;
        }

        internal delegate bool ConditionalDelegate(ILookaroundEnumerator<Token> enumerator);

        public TState? StateEnum { get; }
        public StateMachine<TState> Parent { get; }
        private Dictionary<TokenTypes, List<ConditionalAction>> _Tokens = new Dictionary<TokenTypes, List<ConditionalAction>>();
        private StateAction<TState>? _ElseAction = default;
        private StateAction<TState>? _ThrowAction = default;

        internal void SetPredicate(StateAction<TState> action, ConditionalDelegate predicate)
        {
            var foundAction = _Tokens.Values.SelectMany(cond => cond).FirstOrDefault(listAction => listAction.Action == action);
            if (foundAction == default) throw new NotImplementedException();
            foundAction.Predicate = predicate;
        }

        public StateAction<TState> Expect(TokenTypes tokenType)
        {
            var action = new StateAction<TState>(this);
            _Tokens.Add(tokenType, new List<ConditionalAction>
            {
                new ConditionalAction(action)
            });
            return action;
        }
        public StateAction<TState> Ignore(TokenTypes tokenType)
        {
            var action = new StateAction<TState>(this);
            _Tokens.Add(tokenType, new List<ConditionalAction>
            {
                new ConditionalAction(action)
            });
            action.Ignore();
            return action;
        }

        public StateAction<TState> Else()
        {
            if (_ElseAction != default) throw new NotImplementedException();
            _ElseAction = new StateAction<TState>(this);
            return _ElseAction;
        }

        public StateAction<TState> Throw(Exception? exception = null)
        {
            if (_ThrowAction != default) throw new NotImplementedException();
            _ThrowAction = new StateAction<TState>(this);
            _ThrowAction.Throw(exception);
            return _ThrowAction;
        }

        public bool TryParse(ILookaroundEnumerator<Token> enumerator, [NotNullWhen(true)]out StateAction<TState>? action)
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
