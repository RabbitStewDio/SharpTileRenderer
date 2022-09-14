using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    public enum MapNavigationType
    {
        Map = 0,
        Screen = 1
    }
    
    public interface IMapNavigation
    {
        IMapNavigator<GridDirection> this[MapNavigationType type] { get; }
    }
    
    public interface IViewPort
    {
        /// <summary>
        ///    Where on the screen will the data be rendered. Coordinates are in pixels, screen starts
        ///    at upper left corner. 
        /// </summary>
        ScreenBounds PixelBounds { get; }

        /// <summary>
        ///    Pixel bounds translated into tile coordinates.
        /// </summary>
        TileBounds TileBounds { get; }

        IntDimension TileSize { get; }

        ScreenInsets PixelOverdraw { get; }
        
        /// <summary>
        ///    The map coordinate that correlates to the center of the screen bounds. Note that this map
        ///    coordinate might not be valid on the map. This coordinate is a map coordinate, so the x/y axes
        ///    might not align with the screen's x/y axes.
        /// </summary>
        VirtualMapCoordinate Focus { get; }

        IScreenSpaceNavigator ScreenSpaceNavigator { get; }

        IMapNavigation Navigation { get; }

        int ZLayer { get; }
        int Rotation { get; }

        /// <summary>
        ///    Returns the data organisation type of the underlying maps.
        /// </summary>
        GridType GridType { get; }
        
        TileShape TileShape { get; }
    }
}