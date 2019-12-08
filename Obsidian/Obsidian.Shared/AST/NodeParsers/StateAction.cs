using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Lexing;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.NodeParsers
{

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class StateAction<TState> where TState : struct, Enum
    {
        internal string DebuggerDisplay
        {
            get
            {
                return string.Join(Environment.NewLine, _Actions.Select(act => act.DebuggerDisplay));
            }
        }

        internal StateAction(State<TState> parent)
        {
            Parent = parent;
        }

        internal State<TState> Parent { get; }
        private List<AbstractAction<TState>> _Actions { get; } = new List<AbstractAction<TState>>();

        internal IEnumerable<AbstractAction<TState>> Actions => _Actions.ToArray();

        internal StateAction<TState> MoveTo(TState nextState)
        {
            _Actions.Add(new MoveToStateAction<TState>(this, nextState));
            return this;
        }
        internal StateAction<TState> SetWhiteSpaceMode(WhiteSpacePosition position, WhiteSpaceMode mode)
        {
            _Actions.Add(new SetWhiteSpaceAction<TState>(this, position, mode));
            return this;
        }
        internal StateAction<TState> AndNext(TokenType tokenType)
        {
            Parent.SetPredicate(this, enumerator => enumerator.TryGetNext(out var token) && token.TokenType == tokenType, $".AndNext({tokenType})");
            return this;
        }
        internal StateAction<TState> Return(bool result)
        {
            _Actions.Add(new ReturnStateAction<TState>(this, result));
            return this;
        }
        internal StateAction<TState> Throw(Exception? exception = null)
        {
            _Actions.Add(new ThrowAction<TState>(this, exception));
            return this;
        }

        internal StateAction<TState> Ignore()
        {
            _Actions.Add(new IgnoreAction<TState>(this));
            return this;
        }

        internal StateAction<TState> Accumulate(TokenType? seperator = default)
        {
            _Actions.Add(new AccumulateAction<TState>(this, seperator));
            return this;
        }

        internal StateAction<TState> Expect(TokenType tokenType)
        {
            return Parent.Expect(tokenType);
        }
        internal StateAction<TState> Else()
        {
            return Parent.Else();
        }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class AbstractAction<TState> where TState : struct, Enum
    {
        internal virtual string DebuggerDisplay => ToString();
        internal AbstractAction(StateAction<TState> parent)
        {
            Parent = parent;
        }
        internal StateAction<TState> Parent { get; }
    }

    internal class MoveToStateAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        internal override string DebuggerDisplay => $".MoveTo({NextState})";
        internal MoveToStateAction(StateAction<TState> parent, TState nextState) : base(parent)
        {
            NextState = nextState;
        }

        internal TState NextState { get; }
    }
    internal class ReturnStateAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        internal override string DebuggerDisplay => $".Return({Result})";
        internal ReturnStateAction(StateAction<TState> parent, bool result) : base(parent)
        {
            Result = result;
        }

        internal bool Result { get; }
    }
    internal class ThrowAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        internal override string DebuggerDisplay => $".Throw({Exception})";
        internal ThrowAction(StateAction<TState> parent, Exception? exception = null) : base(parent)
        {
            Exception = exception ?? new NotImplementedException();
        }
        internal Exception Exception { get; }
    }
    internal class IgnoreAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        internal override string DebuggerDisplay => $".Ignore()";
        internal IgnoreAction(StateAction<TState> parent) : base(parent)
        {
        }
    }
    internal class AccumulateAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        internal override string DebuggerDisplay => $".Accumulate({Seperator})";
        internal AccumulateAction(StateAction<TState> parent, TokenType? seperator = default) : base(parent)
        {
            Seperator = seperator;
        }

        internal TokenType? Seperator { get; }
    }
    internal class SetWhiteSpaceAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        internal override string DebuggerDisplay => $".SetWhiteSpaceMode({nameof(WhiteSpacePosition)}.{Position}, {nameof(WhiteSpaceMode)}.{Mode})";
        internal SetWhiteSpaceAction(StateAction<TState> parent, WhiteSpacePosition position, WhiteSpaceMode mode) : base(parent)
        {
            Position = position;
            Mode = mode;
        }

        internal WhiteSpacePosition Position { get;  }
        internal WhiteSpaceMode Mode { get; }
    }

}
