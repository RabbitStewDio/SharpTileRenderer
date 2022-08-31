using System;

namespace SharpTileRenderer.Navigation.Navigators
{
    public class FixedOffsetRotationGridNavigator : IMapNavigator<GridDirection>
    {
        readonly GridDirection[] directions;
        readonly IMapNavigator<GridDirection> parent;
        readonly int rotationSteps;

        public FixedOffsetRotationGridNavigator(IMapNavigator<GridDirection> parent, int rotationSteps)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            if (rotationSteps is < -7 or > 7) throw new ArgumentOutOfRangeException(nameof(rotationSteps));
            
            this.rotationSteps = rotationSteps;
            this.directions = new GridDirection[9];
            for (var i = 0; i < directions.Length; i += 1)
            {
                directions[i] = RotateBy((GridDirection)i, rotationSteps);
            }
        }

        public NavigatorMetaData MetaData => parent.MetaData.WithRotation(rotationSteps);

        public bool NavigateTo(GridDirection direction, in MapCoordinate source, out MapCoordinate result, int steps = 1)
        {
            var d = directions[(int)direction];
            return parent.NavigateTo(d, source, out result, steps);
        }

        public bool Navigate(GridDirection direction, in MapCoordinate origin, out MapCoordinate result, out NavigationInfo info, int steps = 1)
        {
            var d = directions[(int)direction];
            return parent.Navigate(d, origin, out result, out info, steps);
        }

        static GridDirection RotateBy(GridDirection direction, int steps)
        {
            if (direction == GridDirection.None)
            {
                return direction;
            }

            var directionAsInt = (int)direction - 1; // between 0 and 7
            directionAsInt += 8; // between 8 and 15
            directionAsInt += steps; // rotated, now anywhere between 1 and 22
            directionAsInt = directionAsInt % 8 + 1;
            return (GridDirection)directionAsInt;
        }
    }
}