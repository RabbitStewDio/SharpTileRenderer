using System;

namespace SharpTileRenderer.Navigation.Navigators
{
    public class GridNavigator : IMapNavigator<GridDirection>
    {
        public NavigatorMetaData MetaData => NavigatorMetaData.FromGridType(GridType.Grid);
        
        public bool NavigateTo(GridDirection direction, in MapCoordinate origin, out MapCoordinate result, int steps)
        {
            switch (direction)
            {
                case GridDirection.None:
                    result = origin;
                    break;
                case GridDirection.North:
                    result = new MapCoordinate(origin.X, origin.Y - steps);
                    break;
                case GridDirection.NorthEast:
                    result = new MapCoordinate(origin.X + steps, origin.Y - steps);
                    break;
                case GridDirection.East:
                    result = new MapCoordinate(origin.X + steps, origin.Y);
                    break;
                case GridDirection.SouthEast:
                    result = new MapCoordinate(origin.X + steps, origin.Y + steps);
                    break;
                case GridDirection.South:
                    result = new MapCoordinate(origin.X, origin.Y + steps);
                    break;
                case GridDirection.SouthWest:
                    result = new MapCoordinate(origin.X - steps, origin.Y + steps);
                    break;
                case GridDirection.West:
                    result = new MapCoordinate(origin.X - steps, origin.Y);
                    break;
                case GridDirection.NorthWest:
                    result = new MapCoordinate(origin.X - steps, origin.Y - steps);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return true;
        }

        public bool Navigate(GridDirection direction, in MapCoordinate origin, out MapCoordinate result, out NavigationInfo info, int steps = 1)
        {
            info = new NavigationInfo();
            return NavigateTo(direction, origin, out result, steps);
        }
    }
}
