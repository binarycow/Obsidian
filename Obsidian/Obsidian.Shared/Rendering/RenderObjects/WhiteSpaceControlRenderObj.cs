using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Obsidian.Rendering.Visitors;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.Rendering.RenderObjects
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class WhiteSpaceControlRenderObj : RenderObj
    {
        public WhiteSpaceControlRenderObj(WhiteSpaceControlMode controlMode, WhiteSpaceControlPosition position, string? debugValue = null) : base(controlMode)
        {
            Position = position;
            DebugValue = debugValue;
        }

        public string? DebugValue { get; }

        public WhiteSpaceControlPosition Position { get; }
        public WhiteSpaceControlMode WhiteSpaceControlMode => (WhiteSpaceControlMode)Value!;
        public override TOutput Render<TOutput>(IRenderVisitor<TOutput> visitor)
        {
            return visitor.Render(this);
        }
        private string DebuggerDisplay => $"{nameof(WhiteSpaceControlRenderObj)} \"{DebugValue ?? ToString(debug: true)}\" Position: {Position} Control: {WhiteSpaceControlMode}";



    }
}
