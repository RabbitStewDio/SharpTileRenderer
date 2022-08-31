namespace SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters
{
    /// <summary>
    ///    Converts screen coordinates to map coordinates. Assumes that there are two
    ///    related coordinate systems converging at the same origin point. 
    /// </summary>
    public interface IScreenToMapConverter
    {
        /// <summary>
        ///    Converts a screen position to a virtual map position relative to the
        ///    view-port's focal point. This operation ignores all wrapping or clamping
        ///    and the resulting map coordinate needs to be normalized before it can
        ///    be used against a real map.
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        VirtualMapCoordinate ScreenToMap(IViewPort vp, ScreenPosition p);
    }
}