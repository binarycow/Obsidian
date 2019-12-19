using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.NodeParsers
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class StateMachine<TState> where TState : struct, Enum
    {
        private string DebuggerDisplay
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{nameof(StateMachine<TState>)}<{typeof(TState).Name}>");
                sb.AppendLine();
                foreach (var expect in _States.Keys)
                {
                    sb.AppendLine($".State({expect})");
                    foreach(var line in _States[expect].DebuggerDisplay.Split(Environment.NewLine).Select(line => $"     {line}"))
                    {
                        sb.AppendLine(line);
                    }
                    
                }
                return sb.ToString();
            }
        }



        internal StateMachine(TState initialState) : this(initialState, null)
        {
        }
        internal StateMachine(TState initialState, TState? doneState)
        {
            InitialState = initialState;
            CurrentState = InitialState;
            DoneState = doneState;
        }

        internal TState InitialState { get; }
        internal TState? DoneState { get; }
        internal TState CurrentState { get; private set; }

        private readonly Dictionary<TState, State<TState>> _States = new Dictionary<TState, State<TState>>();
        private State<TState>? _ElseState = default;

        private readonly Lazy<Dictionary<TState, Stack<Queue<Token>>>> _Accumulations = new Lazy<Dictionary<TState, Stack<Queue<Token>>>>();
        private Dictionary<TState, Stack<Queue<Token>>> Accumulations => _Accumulations.Value;

        private readonly Lazy<Dictionary<string, (Type Type, object? Value)>> _Variables = new Lazy<Dictionary<string, (Type Type, object? Value)>>();


        private Dictionary<string, (Type Type, object? Value)> Variables => _Variables.Value;


        internal Queue<(WhiteSpacePosition position, WhiteSpaceMode mode)> WhiteSpace { get; } = new Queue<(WhiteSpacePosition position, WhiteSpaceMode mode)>();

        internal bool TryGetVariable<TType>(string name, out TType returnedObject)
        {
            returnedObject = default!;
            if (Variables.TryGetValue(name, out var tuple) == false)
            {
                return false;
            }
            if (tuple.Type != typeof(TType)) throw new NotImplementedException();
            returnedObject = (TType)tuple.Value!;
            return true;
        }
        internal TType GetVariable<TType>(string name, TType defaultValue)
        {
            if(TryGetVariable<TType>(name, out var returnedValue) == false)
            {
                return defaultValue;
            }
            return returnedValue;
        }

        internal void Set<TVarType>(string name, TVarType value)
        {
            Variables.Upsert(name, (Type: typeof(TVarType), Value: value));
        }

        internal bool TryGetAccumulation(TState? state, int index, out string accumulation)
        {
            accumulation = string.Empty;
            if (TryGetAccumulations(state, out var allAccumulations) == false) return false;
            if (index >= allAccumulations.Length) return false;
            accumulation = allAccumulations[index];
            return true;
        }
        internal bool TryGetAccumulations(TState? state, out string[] accumulations)
        {
            accumulations = Array.Empty<string>();
            foreach (var stateKey in Accumulations.Keys)
            {
                if (stateKey.Equals(state) == false)
                {
                    continue;
                }
                accumulations = Accumulations[stateKey]
                    .Reverse()
                    .Select(queue => string.Join(string.Empty, queue.Select(tok => tok.Value)).Trim())
                    .Where(str => !string.IsNullOrEmpty(str))
                    .ToArray();
                return true;
            }
            return false;
        }

        internal State<TState> State(TState currentState)
        {
            var newState = new State<TState>(this, currentState);
            _States.Add(currentState, newState);
            return newState;
        }
        internal State<TState> Else()
        {
            if (_ElseState != default) throw new NotImplementedException();
            _ElseState = new State<TState>(this, null);
            return _ElseState;
        }

        internal bool TryParse(Lexer lexer, string text)
        {
            var tokens = lexer.Tokenize(text);
            using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(tokens);
            return TryParse(enumerator, out _, out _);
        }

        internal bool TryParse(ParsingNode node)
        {
            return TryParse(node, out _, out _);
        }

        internal bool TryParse(ParsingNode node, out WhiteSpaceMode startingWhiteSpace, out WhiteSpaceMode endingWhiteSpace)
        {
            using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(node.Tokens);
            return TryParse(enumerator, out startingWhiteSpace, out endingWhiteSpace);
        }
        internal bool TryParse(ILookaroundEnumerator<Token> enumerator, out WhiteSpaceMode startingWhiteSpace, out WhiteSpaceMode endingWhiteSpace)
        {
            startingWhiteSpace = WhiteSpaceMode.Default;
            endingWhiteSpace = WhiteSpaceMode.Default;
            if (_Accumulations.IsValueCreated) _Accumulations.Value.Clear();
            WhiteSpace.Clear();
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
                    PerformAction(action, enumerator.Current, out var returnResult);
                    if (returnResult.HasValue) return returnResult.Value;
                }
            } while (enumerator.MoveNext());
            if(DoneState.HasValue && CurrentState.Equals(DoneState.Value) == false)
            {
                throw new NotImplementedException();
            }
            startingWhiteSpace = WhiteSpace.FirstOrDefaultValueType(ws => ws.position == WhiteSpacePosition.Start)?.mode ?? WhiteSpaceMode.Default;
            endingWhiteSpace = WhiteSpace.FirstOrDefaultValueType(ws => ws.position == WhiteSpacePosition.End)?.mode ?? WhiteSpaceMode.Default;
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

        private static bool TryGetAction(ILookaroundEnumerator<Token> enumerator, State<TState> state, [NotNullWhen(true)]out StateAction<TState>? action)
        {
            _ = state.TryParse(enumerator, out action);
            return action != default;
        }

        private bool PerformAction(AbstractAction<TState> action, Token currentToken, out bool? returnResult)
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
                    return PerformAction(CurrentState, accumulateAction, currentToken);
                case IgnoreAction<TState> _:
                    return true;
                case SetWhiteSpaceAction<TState> whiteSpaceAction:
                    return PerformAction(whiteSpaceAction);
                case ReturnStateAction<TState> returnAction:
                    returnResult = returnAction.Result;
                    return true;
                case SetAction<TState> setAction:
                    Variables.Upsert(setAction.Name, (setAction.Type, setAction.Value));
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

        private bool PerformAction(TState currentState, AccumulateAction<TState> action, Token currentToken)
        {
            if(Accumulations.TryGetValue(currentState, out var outsideQueue) == false)
            {
                outsideQueue = new Stack<Queue<Token>>();
                outsideQueue.Push(new Queue<Token>());
                Accumulations.Add(currentState, outsideQueue);
            }
            if(currentToken.TokenType == action.Seperator)
            {
                outsideQueue.Push(new Queue<Token>());
                return true;
            }
            outsideQueue.Peek().Enqueue(currentToken);
            return true;
        }
    }
}
