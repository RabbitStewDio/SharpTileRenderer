namespace SharpTileRenderer.Navigation
{
    /// <summary>
    /// Defines the map rendering type, which influences the internal organisation of the map data and how map coordinates are translated into screen coordinates.
    /// </summary>
    /// <para>
    /// Hex modes lack support in the navigation and matching code.
    /// </para>
    public enum GridType
    {
        Grid,
        IsoStaggered,
        IsoDiamond,

        Hex,
        HexDiamond,
    }
}
