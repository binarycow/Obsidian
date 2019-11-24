using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.WhiteSpaceControl;
using System.Diagnostics;
using Obsidian.Rendering.Visitors;

namespace Obsidian.Rendering.RenderObjects
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class NewLineRenderObj : RenderObj
    {
        public NewLineRenderObj(string value, WhiteSpaceControlMode controlMode) : base(value)
        {
            ControlMode = controlMode;
        }

        public WhiteSpaceControlMode ControlMode { get; set; }

        private string DebuggerDisplay => $"{nameof(NewLineRenderObj)} \"{ToString(debug: true)}\" Control: {ControlMode}";
        public override TOutput Render<TOutput>(IRenderVisitor<TOutput> visitor)
        {
            return visitor.Render(this);
        }
    }
}
