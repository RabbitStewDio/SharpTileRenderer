namespace SharpTileRenderer.Navigation
{
    public static class GridTypeExtensions
    {
        public static bool IsStaggered(this GridType t)
        {
            return t == GridType.IsoStaggered || t == GridType.Hex;
        }

        public static bool IsIsometric(this GridType t)
        {
            return t == GridType.IsoStaggered || t == GridType.IsoDiamond;
        }
    }
}
