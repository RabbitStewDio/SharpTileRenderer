namespace SharpTileRenderer.TileMatching.Sprites
{
    /// <summary>
    /// Defines the relative sprite render position. This system assumes that a 
    /// tile has a size of 1x1 units and that fractional units are allowed. The
    /// position provides the anchor alignment point for the tile. 
    /// </summary>
    public enum SpritePosition
    {
        /// <summary>
        ///  Render at 0, 0. 
        ///  <para/>
        ///  This is the normal position without any offset correction.
        /// </summary>
        Whole = 0,

        /// <summary>
        ///  Iso: 0, -0.25.
        ///  Grid: -0.25, -0.25.
        ///  <para/>
        ///  Render the northern/ northwestern quadrant of the tile cell.
        /// </summary>
        Up = 1,

        /// <summary>
        ///  Iso: 0.25, 0.
        ///  Grid: 0.25, -0.25.
        ///  <para/>
        ///  Render the eastern/ northeastern quadrant of the tile cell.
        /// </summary>
        Right = 2,

        /// <summary>
        ///  Iso: 0, 0.25
        ///  Grid: 0.25, 0.25.
        ///  <para/>
        ///  Render the southern/ Southeast quadrant of the tile cell.
        /// </summary>
        Down = 3,

        /// <summary>
        ///  Iso: -0.25, 0
        ///  Grid: -0.25, +0.25.
        ///  <para/>
        ///  Render the west / south west quadrant of the tile cell.
        /// </summary>
        Left = 4,

        /// <summary>
        ///  Iso: Render at 0, -0.5. 
        ///  Grid: Render at -0.5, 0.5.
        ///  <para/>
        ///  This aligns the tile centre point with the point where the four northern 
        /// </summary>
        CellMap = 5,
    }

}
