using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;

namespace SharpTileRenderer.Util
{
    public class ListObjectPolicy<T>: DefaultPooledObjectPolicy<List<T>>
    {
        public override bool Return(List<T> obj)
        {
            obj.Clear();
            return base.Return(obj);
        }
    }
}