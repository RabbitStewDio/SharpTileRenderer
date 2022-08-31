using System;

namespace SharpTileRenderer.Strategy.Base.Model
{
    [Flags]
    public enum RoadTypeId
    {
        None = 0,
        River = 1,
        Road = 2,
        Railroad = 4
    }
}