using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Rendering.RenderObjects;

namespace Obsidian.Rendering.Visitors
{
    public interface IRenderVisitor<TOutput>
    {
        TOutput Render(LiteralRenderObj item);
        TOutput Render(WhiteSpaceControlRenderObj item);
        TOutput Render(WhiteSpaceRenderObj item);
        TOutput Render(NewLineRenderObj item);
    }
}
