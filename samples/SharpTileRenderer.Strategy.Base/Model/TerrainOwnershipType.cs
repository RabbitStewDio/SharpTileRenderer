using System;

namespace SharpTileRenderer.Strategy.Base.Model
{
    [Flags]
    public enum TerrainOwnershipType
    {
        None = 0,
        Farmed = 1,
        Influence = 2
    }
}