using SharpTileRenderer.Navigation;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    public readonly struct WorldPosition
    {
        /// <summary>
        ///    The map coordinate pointing to the actual world position displayed at the screen.
        /// </summary>
        public readonly Optional<ContinuousMapCoordinate> Coordinate;
        /// <summary>
        ///    A non-normalized map coordinate. Rotation is applied, but limit and wrapping are ignored.
        /// </summary>
        public readonly VirtualMapCoordinate VirtualCoordinate;

        public readonly NavigationInfo Info;

        public WorldPosition(VirtualMapCoordinate virtualCoordinate, NavigationInfo info)
        {
            VirtualCoordinate = virtualCoordinate;
            Info = info;
            Coordinate = Optional.Empty();
        }

        public WorldPosition(ContinuousMapCoordinate coordinate, 
                             VirtualMapCoordinate virtualCoordinate, 
                             NavigationInfo info)
        {
            VirtualCoordinate = virtualCoordinate;
            Info = info;
            Coordinate = coordinate;
        }

        public override string ToString()
        {
            return $"{nameof(Coordinate)}: {Coordinate}, {nameof(VirtualCoordinate)}: {VirtualCoordinate}, {nameof(Info)}: {Info}";
        }
    }
}