using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Rendering.RenderObjects;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.Rendering.Visitors
{
    public class StringRenderVisitor : IRenderVisitor<IEnumerable<string>>
    {
        public IEnumerable<string> Render(LiteralRenderObj item)
        {
            yield return item.Value?.ToString() ?? string.Empty;
        }

        public IEnumerable<string> Render(WhiteSpaceRenderObj item)
        {
            var toOutput = item.Value?.ToString();
            if (toOutput == null)
            {
                yield break;
            }
            switch (item.ControlMode)
            {
                case WhiteSpaceControlMode.Default:
                case WhiteSpaceControlMode.Keep:
                    yield return toOutput;
                    yield break;
                case WhiteSpaceControlMode.Trim:
                    yield break;
            }
        }

        public IEnumerable<string> Render(WhiteSpaceControlRenderObj item)
        {
            yield break;
        }

        public IEnumerable<string> Render(NewLineRenderObj item)
        {
            var toOutput = item.Value?.ToString();
            if (toOutput == null)
            {
                yield break;
            }
            switch (item.ControlMode)
            {
                case WhiteSpaceControlMode.Default:
                case WhiteSpaceControlMode.Keep:
                    yield return toOutput;
                    yield break;
                case WhiteSpaceControlMode.Trim:
                    yield break;
            }
        }
    }
}
