using System;
using System.Diagnostics;

namespace SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters
{
    class GridScreenToMapConverter : IScreenToMapConverter
    {
        public VirtualMapCoordinate ScreenToMap(IViewPort vp, ScreenPosition p)
        {
            var focus = vp.Focus;
            var centre = vp.PixelBounds.Center;

            // make screen coordinates relative to the centre, which also represents the focus point.
            var x = p.X - centre.X;
            var y = p.Y - centre.Y;
            var tileSize = vp.TileSize;

            // ignoring rotation for now.
            (x, y) = RotateCCW(x, y, vp.Rotation);
            return new VirtualMapCoordinate(focus.X + (x / tileSize.Width), focus.Y + (y / tileSize.Height));
        }

        (float x, float y) RotateCCW(float x, float y, int steps)
        {
            Debug.Assert(steps is >= 0 and < 4);
            switch (steps)
            {
                case 0: return (x, y);
                case 1: return (-y, x);
                case 2: return (-x, -y);
                case 3: return (y, -x);
                default: throw new ArgumentException();
            }
        }
    }
}