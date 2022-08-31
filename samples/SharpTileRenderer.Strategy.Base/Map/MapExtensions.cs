using SharpTileRenderer.Strategy.Base.Util;
using System;
using System.Text;

namespace SharpTileRenderer.Strategy.Base.Map
{
    public static class MapExtensions
    {
        public static string Print<T>(this IMap2D<T> map, Func<T, char> printer)
        {
            var b = new StringBuilder();
            for (int y = 0; y < map.Height; y += 1)
            {
                for (int x = 0; x < map.Width; x += 1)
                {
                    var c = printer(map[x, y]);
                    b.Append(c);
                }

                b.Append('\n');
            }

            return b.ToString();
        }
        
    }
}