using System;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    [Flags]
    public enum CardinalFlags
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8
    }
}