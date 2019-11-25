using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian
{
    public static class ObsidianExpressionEx
    {
        public static Lazy<ExpressionCreators.Self> _Self = new Lazy<ExpressionCreators.Self>();
        public static ExpressionCreators.Self Self => _Self.Value;
    }
}
