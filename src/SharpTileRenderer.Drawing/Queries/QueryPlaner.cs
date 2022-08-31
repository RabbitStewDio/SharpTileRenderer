using SharpTileRenderer.Navigation;
using System;

namespace SharpTileRenderer.Drawing.Queries
{
    public static class QueryPlaner
    {
        static readonly IQueryPlaner grid = new CachingQueryPlaner(new GridQueryPlaner());
        static readonly IQueryPlaner iso = new CachingQueryPlaner(new IsometricQueryPlaner());
        static readonly IQueryPlaner isoStaggered = new CachingQueryPlaner(new IsometricQueryPlaner());

        public static IQueryPlaner FromTileType(GridType gt)
        {
            var queryPlaner = gt switch
            {
                GridType.Grid => grid,
                GridType.IsoDiamond => iso,
                GridType.IsoStaggered => isoStaggered,
                GridType.Hex => throw new InvalidOperationException("hex tile sets are not yet supported"),
                GridType.HexDiamond => throw new InvalidOperationException("hex tile sets are not yet supported"),
                _ => throw new ArgumentException()
            };
            return queryPlaner;
        }
    }
}