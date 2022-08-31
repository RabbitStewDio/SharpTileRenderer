namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public static class CardinalSelectorKeyExtensions
    {
        public static CardinalSelectorKey AsCardinalKey(this CardinalFlags flag)
        {
            return CardinalSelectorKey.ValueOf(flag.HasFlagEx(CardinalFlags.North),
                                               flag.HasFlagEx(CardinalFlags.East),
                                               flag.HasFlagEx(CardinalFlags.South),
                                               flag.HasFlagEx(CardinalFlags.West)
            );
        }

        static bool HasFlagEx(this CardinalFlags flag, CardinalFlags value)
        {
            return (flag & value) == value;
        }

        public static int AsInt(this CardinalIndex idx) => (int)idx;
    }

    public static class NeighbourIndexExtensions
    {
        public static int AsInt(this NeighbourIndex idx) => (int)idx;
    }
}