using System;

namespace SharpTileRenderer.Strategy.Base.Map
{
    [Flags]
    public enum FogState
    {
        /// <summary>
        ///  Havent seen the tile yet.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///  Have seen the tile, but no unit is in range.
        /// </summary>
        Explored = 1,

        /// <summary>
        ///  A unit is currently looking at the tile.
        /// </summary>
        Visible = 2
    }
}
