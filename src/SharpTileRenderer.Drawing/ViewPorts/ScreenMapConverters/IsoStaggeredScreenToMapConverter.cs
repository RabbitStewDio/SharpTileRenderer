using SharpTileRenderer.Navigation;
using SharpTileRenderer.Navigation.Navigators;
using System;

namespace SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters
{
    class IsoStaggeredScreenToMapConverter : IScreenToMapConverter
    {
        readonly GridScreenToMapConverter gridMapper;
        readonly IsoStaggeredGridNavigator staggeredNavigator;

        public IsoStaggeredScreenToMapConverter()
        {
            gridMapper = new GridScreenToMapConverter();
            staggeredNavigator = new IsoStaggeredGridNavigator();
        }

        public VirtualMapCoordinate ScreenToMap(IViewPort vp, ScreenPosition p)
        {
            // get the normalized grid coordinate as integer.
            // this roughly tells us the cell we are in.
            // Note: Grid coordinates are 1-unit based, 
            //       as opposed to the 4-unit based view coordinates 
            // 
            // For Iso-Staggered maps the grid mapper already returns the (almost) correct 
            // coordinates for the central tile.
            // We still have to check whether the coordinate given is 
            // in one of the corner regions.
            var cgrid = gridMapper.ScreenToMap(vp, p);
            var ngrid = cgrid.Normalize();

            // offsets within the tile are not scaled.
            var dx = cgrid.X - ngrid.X;
            var dy = cgrid.Y - ngrid.Y;

            // In Staggered maps, the underlying map is compressed on the y-axis
            // compared to normal grids. On the y-axis one map-grid-cell of 1 
            // moves the position by 2.
            ngrid = ngrid.WithY(ngrid.Y * 2);

            if (Math.Abs(dx) > (0.5 - Math.Abs(dy)))
            {
                // This is one of the outer corner regions.
                //
                //
                //          NW /\  NE
                //            /  \
                //           /    \
                //           \    /
                //            \  /
                //          SW \/  SE
                //
                //  Those are possible non-continuous jumps and thus
                //  we have to tread carefully here. We must navigate
                //  to the next cell, and then apply the delta position
                //  (how far from the centre of the new tile)

                var direction = ComputeQuadrant(dx, dy);
                staggeredNavigator.NavigateTo(direction, ngrid, out MapCoordinate result);
                var d = (0.5f - Math.Abs(dx));
                var rdx = -Math.Sign(dx) * d;
                var rdy = -Math.Sign(dy) * (0.5f - Math.Abs(dy));
                return new VirtualMapCoordinate(result.X + rdx, result.Y + rdy);
            }

            return new VirtualMapCoordinate(ngrid.X + dx, ngrid.Y + dy);
        }

        static GridDirection ComputeQuadrant(double dx, double dy)
        {
            var xNegative = (dx < 0);
            var yNegative = (dy < 0);
            return (xpos: xNegative, ypos: yNegative) switch
            {
                (true, true) => GridDirection.NorthEast,
                (false, true) => GridDirection.NorthWest,
                (true, false) => GridDirection.SouthEast,
                (false, false) => GridDirection.SouthWest,
            };
        }
    }
}