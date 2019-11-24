using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Obsidian.Rendering.Visitors;
using Obsidian.WhiteSpaceControl;
using System.Linq.Expressions;

namespace Obsidian.Rendering.RenderObjects
{
    public abstract class RenderObj
    {
        public RenderObj(object? value)
        {
            Value = value;
        }
        public object? Value { get; }
        public abstract TOutput Render<TOutput>(IRenderVisitor<TOutput> visitor);

        public override string ToString()
        {
            return Value?.ToString() ?? "{null}";
        }
        public string ToString(bool debug)
        {
            return debug ? ToString().WhiteSpaceEscape() : ToString();
        }
    }
}
