namespace SharpTileRenderer.Navigation
{
    /// <summary>
    ///  A map navigator knows how to progress from one tile to the next given an abstract direction.
    ///  A non-rotated navigator aligns so that moving north is equal to moving towards negative y. 
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
}