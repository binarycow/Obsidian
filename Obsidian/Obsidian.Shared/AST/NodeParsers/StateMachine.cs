using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.NodeParsers
{
    internal class StateMachine<TState> where TState : struct, Enum
    {
        public StateMachine(TState initialState, TState doneState)
        {
            InitialState = initialState;
            CurrentState = InitialState;
            DoneState = doneState;
        }

        public TState InitialState { get; }
        public TState DoneState { get; }
        public TState CurrentState { get; private set; }

        private Dictionary<TState, State<TState>> _States = new Dictionary<TState, State<TState>>();
        private State<TState>? _ElseState = default;

        private Lazy<Dictionary<State<TState>, Queue<Queue<Token>>>> _Accumulations = new Lazy<Dictionary<State<TState>, Queue<Queue<Token>>>>();
        private Dictionary<State<TState>, Queue<Queue<Token>>> Accumulations => _Accumulations.Value;

        public Queue<(WhiteSpacePosition position, WhiteSpaceMode mode)> WhiteSpace { get; } = new Queue<(WhiteSpacePosition position, WhiteSpaceMode mode)>();



        public State<TState> State(TState currentState)
        {
            var newState = new State<TState>(this, currentState);
            _States.Add(currentState, newState);
            return newState;
        }
        public State<TState> Else()
        {
            if (_ElseState != default) throw new NotImplementedException();
            _ElseState = new State<TState>(this, null);
            return _ElseState;
        }



        public bool TryParse(ParsingNode node)
        {
            return TryParse(node, out _);
        }
        public bool TryParse(ParsingNode node, out Dictionary<TState?, string[]> accumulations)
        {
            using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(node.Tokens);
            return TryParse(enumerator, out accumulations);
        }
        public bool TryParse(ILookaroundEnumerator<Token> enumerator, out Dictionary<TState?, string[]> accumulations)
        {
            if (_Accumulations.IsValueCreated) _Accumulations.Value.Clear();
            accumulations = new Dictionary<TState?, string[]>();
            CurrentState = InitialState;
            if(enumerator.MoveNext() == false)
            {
                return false;
            }
            do
            {
                if (TryGetState(out var state) == false || state == null)
                {
                    return false;
                }
                if(TryGetAction(enumerator, state, out var stateAction) == false || stateAction == null)
                {
                    return false;
                }
                foreach (var action in stateAction.Actions)
                {
                    PerformAction(state, action, enumerator.Current, out var returnResult);
                    if (returnResult.HasValue) return returnResult.Value;
                }
            } while (enumerator.MoveNext());
            if(CurrentState.Equals(DoneState) == false)
            {
                throw new NotImplementedException();
            }

            foreach(var key in Accumulations.Keys)
            {
                accumulations.Add(key.StateEnum, Accumulations[key].Select(queue => string.Join(string.Empty, queue.Select(token => token.Value))).ToArray());
            }
            return true;
        }


        private bool TryGetState([NotNullWhen(true)]out State<TState>? state)
        {
            if (_States.TryGetValue(CurrentState, out state) == false)
            {
                if (_ElseState == default)
                {
                    return false;
                }
                state = _ElseState;
            }
            return state != default;
        }

        private bool TryGetAction(ILookaroundEnumerator<Token> enumerator, State<TState> state, [NotNullWhen(true)]out StateAction<TState>? action)
        {
            _ = state.TryParse(enumerator, out action);
            return action != default;
        }

        private bool PerformAction(State<TState> currentState, AbstractAction<TState> action, Token currentToken, out bool? returnResult)
        {
            returnResult = null;
            switch (action)
            {
                case MoveToStateAction<TState> moveTo:
                    CurrentState = moveTo.NextState;
                    return true;
                case ThrowAction<TState> throwAction:
                    throw throwAction.Exception;
                case AccumulateAction<TState> accumulateAction:
                    return PerformAction(currentState, accumulateAction, currentToken);
                case IgnoreAction<TState> _:
                    return true;
                case SetWhiteSpaceAction<TState> whiteSpaceAction:
                    return PerformAction(whiteSpaceAction);
                case ReturnStateAction<TState> returnAction:
                    returnResult = returnAction.Result;
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }

        private bool PerformAction(SetWhiteSpaceAction<TState> action)
        {
            WhiteSpace.Enqueue((position: action.Position, mode: action.Mode));
            return true;
        }

        private bool PerformAction(State<TState> currentState, AccumulateAction<TState> action, Token currentToken)
        {
            if(Accumulations.TryGetValue(currentState, out var outsideQueue) == false)
            {
                outsideQueue = new Queue<Queue<Token>>();
                outsideQueue.Enqueue(new Queue<Token>());
                Accumulations.Add(currentState, outsideQueue);
            }
            if(currentToken.TokenType == action.Seperator)
            {
                outsideQueue.Enqueue(new Queue<Token>());
            }
            outsideQueue.Peek().Enqueue(currentToken);
            return true;
        }
    }
}
