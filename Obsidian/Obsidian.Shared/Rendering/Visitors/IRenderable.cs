using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Rendering.Visitors
{
    public interface IRenderable
    {
        TOutput Render<TOutput>(IRenderVisitor<TOutput> visitor);
    }
}
