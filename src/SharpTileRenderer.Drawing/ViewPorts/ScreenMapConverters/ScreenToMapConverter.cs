using SharpTileRenderer.Navigation;
using System;

namespace SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters
{
    public static class ScreenToMapConverter
    {
        public static IScreenToMapConverter Create(GridType t)
        {
            return t switch
            {
                GridType.Grid => new GridScreenToMapConverter(),
                GridType.IsoStaggered => new IsoStaggeredScreenToMapConverter(),
                GridType.IsoDiamond => new IsoDiamondScreenToMapConverter(),
                _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
            };
        }
    }
}