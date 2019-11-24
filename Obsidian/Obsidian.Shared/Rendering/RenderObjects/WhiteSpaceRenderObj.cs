using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Rendering.Visitors;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.Rendering.RenderObjects
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class WhiteSpaceRenderObj : RenderObj
    {
        public WhiteSpaceRenderObj(string value, WhiteSpaceControlMode controlMode) : base(value)
        {
            ControlMode = controlMode;
        }
        public override TOutput Render<TOutput>(IRenderVisitor<TOutput> visitor)
        {
            return visitor.Render(this);
        }
        public WhiteSpaceControlMode ControlMode { get; set; } = WhiteSpaceControlMode.Default;
        private string DebuggerDisplay => $"{nameof(WhiteSpaceRenderObj)} \"{ToString(debug: true)}\" Control: {ControlMode}";
    }
}
