using SharpTileRenderer.Strategy.Base.Map;
using SharpTileRenderer.Strategy.Base.Util;
using System;

namespace SharpTileRenderer.Strategy.Base.Model
{
    public class TerrainMap : IMap2D<TerrainData>
    {
        readonly TerrainData[,] data;

        public TerrainMap(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.data = new TerrainData[width, height];
        }

        public int Width { get; }
        public int Height { get; }
        public event EventHandler<MapDataChangedEventArgs>? MapDataChanged;

        public TerrainData this[int x, int y]
        {
            get { return data[x, y]; }
            set
            {
                data[x, y] = value; 
                MapDataChanged?.Invoke(this, new MapDataChangedEventArgs(x, y));
            }
        }
    }
}