namespace SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters
{
    class IsoDiamondScreenToMapConverter : IScreenToMapConverter
    {
        readonly GridScreenToMapConverter gridMapper;

        public IsoDiamondScreenToMapConverter()
        {
            gridMapper = new GridScreenToMapConverter();
        }

        public VirtualMapCoordinate ScreenToMap(IViewPort vp, ScreenPosition p)
        {
            // get the normalized grid coordinate as integer.
            // this roughly tells us the cell we are in.
            // Note: Grid coordinates are 1-unit based, 
            //       as opposed to the 4-unit based view coordinates 
            var grid = gridMapper.ScreenToMap(vp, p);
            var mouseGridX = (grid.Y + grid.X);
            var mouseGridY = (grid.Y - grid.X);
            return new VirtualMapCoordinate(mouseGridX, mouseGridY);
        }
    }
}