namespace SharpTileRenderer.RPG.Base.Map
{
    public readonly struct MapDataChangedEventArgs
    {
        public Point Coordinate { get; }
        public int Range { get; }

        public MapDataChangedEventArgs(int x, int y, int range = 1)
        {
            Coordinate = new Point(x, y);
            Range = range;
        }
    }
}
