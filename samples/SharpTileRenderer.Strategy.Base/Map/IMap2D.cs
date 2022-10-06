using System;

namespace SharpTileRenderer.Strategy.Base.Map
{
    public interface IMap2D<out TEntity>
    {
        int Width { get; }
        int Height { get; }
        TEntity this[int x, int y] { get; }
        event EventHandler<MapDataChangedEventArgs> MapDataChanged;
    }
}
