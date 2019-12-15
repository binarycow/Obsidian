using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Transforming
{
    internal interface ITransformable
    {
        TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor);
        void Transform(ITransformVisitor visitor);
    }
}
