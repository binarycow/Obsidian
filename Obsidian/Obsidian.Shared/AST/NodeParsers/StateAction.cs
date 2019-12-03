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

        public StateAction(State<TState> parent)
        {
            Parent = parent;
        }

        public State<TState> Parent { get; }
        private List<AbstractAction<TState>> _Actions { get; } = new List<AbstractAction<TState>>();

        public IEnumerable<AbstractAction<TState>> Actions => _Actions.ToArray();

        public StateAction<TState> MoveTo(TState nextState)
        {
            _Actions.Add(new MoveToStateAction<TState>(this, nextState));
            return this;
        }
        public StateAction<TState> SetWhiteSpaceMode(WhiteSpacePosition position, WhiteSpaceMode mode)
        {
            _Actions.Add(new SetWhiteSpaceAction<TState>(this, position, mode));
            return this;
        }
        public StateAction<TState> AndNext(TokenTypes tokenType)
        {
            Parent.SetPredicate(this, enumerator => enumerator.TryGetNext(out var token) && token.TokenType == tokenType, $".AndNext({tokenType})");
            return this;
        }
        public StateAction<TState> Return(bool result)
        {
            _Actions.Add(new ReturnStateAction<TState>(this, result));
            return this;
        }
        public StateAction<TState> Throw(Exception? exception = null)
        {
            _Actions.Add(new ThrowAction<TState>(this, exception));
            return this;
        }

        public StateAction<TState> Ignore()
        {
            _Actions.Add(new IgnoreAction<TState>(this));
            return this;
        }

        public StateAction<TState> Accumulate(TokenTypes? seperator = default)
        {
            _Actions.Add(new AccumulateAction<TState>(this));
            return this;
        }

        public StateAction<TState> Expect(TokenTypes tokenType)
        {
            return Parent.Expect(tokenType);
        }
        public StateAction<TState> Else()
        {
            return Parent.Else();
        }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class AbstractAction<TState> where TState : struct, Enum
    {
        public virtual string DebuggerDisplay => ToString();
        public AbstractAction(StateAction<TState> parent)
        {
            Parent = parent;
        }
        public StateAction<TState> Parent { get; }
    }

    internal class MoveToStateAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        public override string DebuggerDisplay => $".MoveTo({NextState})";
        public MoveToStateAction(StateAction<TState> parent, TState nextState) : base(parent)
        {
            NextState = nextState;
        }

        public TState NextState { get; }
    }
    internal class ReturnStateAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        public override string DebuggerDisplay => $".Return({Result})";
        public ReturnStateAction(StateAction<TState> parent, bool result) : base(parent)
        {
            Result = result;
        }

        public bool Result { get; }
    }
    internal class ThrowAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        public override string DebuggerDisplay => $".Throw({Exception})";
        public ThrowAction(StateAction<TState> parent, Exception? exception = null) : base(parent)
        {
            Exception = exception ?? new NotImplementedException();
        }
        public Exception Exception { get; }
    }
    internal class IgnoreAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        public override string DebuggerDisplay => $".Ignore()";
        public IgnoreAction(StateAction<TState> parent) : base(parent)
        {
        }
    }
    internal class AccumulateAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        public override string DebuggerDisplay => $".Accumulate({Seperator})";
        public AccumulateAction(StateAction<TState> parent, TokenTypes? seperator = default) : base(parent)
        {
            Seperator = seperator;
        }

        public TokenTypes? Seperator { get; }
    }
    internal class SetWhiteSpaceAction<TState> : AbstractAction<TState> where TState : struct, Enum
    {
        public override string DebuggerDisplay => $".SetWhiteSpaceMode({nameof(WhiteSpacePosition)}.{Position}, {nameof(WhiteSpaceMode)}.{Mode})";
        public SetWhiteSpaceAction(StateAction<TState> parent, WhiteSpacePosition position, WhiteSpaceMode mode) : base(parent)
        {
            Position = position;
            Mode = mode;
        }

        public WhiteSpacePosition Position { get;  }
        public WhiteSpaceMode Mode { get; }
    }

}
