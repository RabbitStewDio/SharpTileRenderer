using SharpTileRenderer.Strategy.Base.Map;
using System;

namespace SharpTileRenderer.Strategy.Base.Util
{
    public interface IMap2D<out TEntity>
    {
        int Width { get; }
        int Height { get; }
        TEntity this[int x, int y] { get; }
        event EventHandler<MapDataChangedEventArgs> MapDataChanged;
    }
}
