using System;

namespace SharpTileRenderer.RPG.Base.Map
{
    public interface IMap2D<out TEntity>
    {
        int Width { get; }
        int Height { get; }
        TEntity this[int x, int y] { get; }
        event EventHandler<MapDataChangedEventArgs> MapDataChanged;
    }

    public class DefaultMap<TEntity> : IMap2D<TEntity>
    {
        readonly TEntity[] data;
        
        public int Width { get; }
        public int Height { get; }

        public DefaultMap(int width, int height, TEntity defaultEntity)
        {
            Width = width;
            Height = height;
            data = new TEntity[width * height];
            Array.Fill(data, defaultEntity);
        }

        public TEntity this[int x, int y]
        {
            get { return data[x + y * Width]; }
            set
            {
                data[x + y * Width] = value;
                MapDataChanged?.Invoke(this, new MapDataChangedEventArgs(x, y));
            }
        }

        public event EventHandler<MapDataChangedEventArgs>? MapDataChanged;
    }
}
