using SharpTileRenderer.Navigation;
using System;

namespace SharpTileRenderer.TileMatching.Selectors
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

    public static class SpritePositionExtensions
    {
        public static ContinuousMapCoordinate Apply(this SpritePosition pos, ContinuousMapCoordinate mapPos, GridType t)
        {
            switch (t)
            {
                case GridType.Grid:
                {
                    return pos switch
                    {
                        SpritePosition.Whole => mapPos,
                        SpritePosition.Up => new ContinuousMapCoordinate(mapPos.X - 0.25f, mapPos.Y - 0.25f),
                        SpritePosition.Right => new ContinuousMapCoordinate(mapPos.X + 0.25f, mapPos.Y - 0.25f),
                        SpritePosition.Down => new ContinuousMapCoordinate(mapPos.X + 0.25f, mapPos.Y + 0.25f),
                        SpritePosition.Left => new ContinuousMapCoordinate(mapPos.X - 0.25f, mapPos.Y + 0.25f),
                        SpritePosition.CellMap => new ContinuousMapCoordinate(mapPos.X - 0.5f, mapPos.Y),
                        _ => throw new ArgumentOutOfRangeException(nameof(pos), pos, null)
                    };
                }
                case GridType.IsoStaggered:
                case GridType.IsoDiamond:
                {
                    return pos switch
                    {
                        SpritePosition.Whole => mapPos,
                        SpritePosition.Up => new ContinuousMapCoordinate(mapPos.X, mapPos.Y - 0.25f),
                        SpritePosition.Right => new ContinuousMapCoordinate(mapPos.X + 0.25f, mapPos.Y),
                        SpritePosition.Down => new ContinuousMapCoordinate(mapPos.X, mapPos.Y + 0.25f),
                        SpritePosition.Left => new ContinuousMapCoordinate(mapPos.X - 0.25f, mapPos.Y),
                        SpritePosition.CellMap => new ContinuousMapCoordinate(mapPos.X, mapPos.Y - 0.5f),
                        _ => throw new ArgumentOutOfRangeException(nameof(pos), pos, null)
                    };
                }
                case GridType.Hex:
                case GridType.HexDiamond:
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }
    }

}
