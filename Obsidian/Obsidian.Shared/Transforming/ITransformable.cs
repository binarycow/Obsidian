using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Transforming
{
    public interface ITransformable
    {
        TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor);
    }
}
