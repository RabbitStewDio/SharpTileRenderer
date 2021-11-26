namespace SharpTileRenderer.TileMatching.Registry
{
    public enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    public static class IndexConversion
    {
        public static int AsInt(this CardinalIndex c)
        {
            return (int)c;
        }

        public static int AsInt(this NeighbourIndex c)
        {
            return (int)c;
        }
    }
}
