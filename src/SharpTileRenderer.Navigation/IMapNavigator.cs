namespace SharpTileRenderer.Navigation
{
    /// <summary>
    ///  A map navigator knows how to progress from one tile to the next given a abstract
    ///  direction. 
    /// </summary>
    public interface IMapNavigator<in TDirection>
        where TDirection : struct
    {
        NavigatorMetaData MetaData { get; }
        bool NavigateTo(TDirection direction, in MapCoordinate origin, out MapCoordinate result, int steps = 1);

        bool Navigate(TDirection direction,
                      in MapCoordinate origin,
                      out MapCoordinate result,
                      out NavigationInfo info,
                      int steps = 1);
    }

    public readonly struct NavigationInfo
    {
        /// <summary>
        ///   Indicates the number and direction of horizontal wrap-arounds.
        /// </summary>
        public readonly int WrapXIndicator;
        /// <summary>
        ///   Indicates the number and direction of vertical wrap-arounds.
        /// </summary>
        public readonly int WrapYIndicator;
        /// <summary>
        ///   Indicates that the map has been limited horizontally.
        /// </summary>
        public readonly bool LimitedX;
        /// <summary>
        ///   Indicates that the map has been limited vertically.
        /// </summary>
        public readonly bool LimitedY;

        public NavigationInfo(int wrapXIndicator, int wrapYIndicator, bool limitedX, bool limitedY)
        {
            WrapXIndicator = wrapXIndicator;
            WrapYIndicator = wrapYIndicator;
            LimitedX = limitedX;
            LimitedY = limitedY;
        }
    }
}
