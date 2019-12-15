using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Transforming
{
    internal interface IForceTransformable
    {
        TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force);
    }
}
