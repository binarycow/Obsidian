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
    public class LiteralRenderObj : RenderObj
    {
        public LiteralRenderObj(object? value) : base(value)
        {
        }

        private string DebuggerDisplay => $"{nameof(LiteralRenderObj)} \"{ToString(debug: true)}\"";
        public override TOutput Render<TOutput>(IRenderVisitor<TOutput> visitor)
        {
            return visitor.Render(this);
        }

    }
}
