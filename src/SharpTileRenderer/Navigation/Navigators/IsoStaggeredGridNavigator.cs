using System;

namespace SharpTileRenderer.Navigation.Navigators
{
    public class IsoStaggeredGridNavigator : IMapNavigator<GridDirection>
    {
        public NavigatorMetaData MetaData => NavigatorMetaData.FromGridType(GridType.IsoStaggered);
        
        public bool NavigateTo(GridDirection direction, in MapCoordinate origin, out MapCoordinate result, int steps = 1)
        {
            if (steps == 0 || direction == GridDirection.None)
            {
                result = origin;
                return true;
            }

            result = origin;

            switch (direction)
            {
                case GridDirection.North:
                    result = new MapCoordinate(result.X, result.Y - 2 * steps);
                    return true;
                case GridDirection.East:
                    result = new MapCoordinate(result.X + steps, result.Y);
                    return true;
                case GridDirection.South:
                    result = new MapCoordinate(result.X, result.Y + 2 * steps);
                    return true;
                case GridDirection.West:
                    result = new MapCoordinate(result.X - steps, result.Y);
                    return true;
            }

            for (var i = 0; i < steps; i += 1)
            {
                switch (direction)
                {
                    case GridDirection.NorthEast:
                        result = new MapCoordinate(result.X + (result.Y & 1), origin.Y - 1);
                        break;
                    case GridDirection.SouthEast:
                        result = new MapCoordinate(result.X + (result.Y & 1), origin.Y + 1);
                        break;
                    case GridDirection.SouthWest:
                        result = new MapCoordinate(result.X + (result.Y & 1) - 1, origin.Y + 1);
                        break;
                    case GridDirection.NorthWest:
                        result = new MapCoordinate(result.X + (result.Y & 1) - 1, result.Y - 1);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
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
