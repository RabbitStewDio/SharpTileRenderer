namespace SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters
{
    class IsoDiamondScreenToMapConverter : IScreenToMapConverter
    {
        VirtualMapCoordinate GridMap(IViewPort vp, ScreenPosition p)
        {
            var centre = vp.PixelBounds.Center;

            var x = p.X - centre.X;
            var y = p.Y - centre.Y;
            var tileSize = vp.TileSize;
            return new VirtualMapCoordinate((x / tileSize.Width), (y / tileSize.Height));
        }

        public VirtualMapCoordinate ScreenToMap(IViewPort vp, ScreenPosition p)
        {
            // get the normalized grid coordinate as integer.
            // this roughly tells us the cell we are in.
            // Note: Grid coordinates are 1-unit based, 
            //       as opposed to the 4-unit based view coordinates 
            var grid = GridMap(vp, p);
            var mouseGridX = (grid.X - grid.Y);
            var mouseGridY = (grid.X + grid.Y);
            return new VirtualMapCoordinate(mouseGridX, mouseGridY) + vp.Focus;
        }
    }
}