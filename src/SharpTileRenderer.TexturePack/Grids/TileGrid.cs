using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TexturePack.Grids
{
    /// <summary>
    ///   A grid of tiles defined for a given texture.
    /// </summary>
    public readonly struct TileGrid
    {
        public TileGrid(int cellWidth,
                        int cellHeight,
                        int offsetX,
                        int offsetY,
                        int anchorX,
                        int anchorY,
                        params GridTileDefinition[] tiles) :
            this(cellWidth, cellHeight, offsetX, offsetY, anchorX, anchorY, 0, 0, tiles)
        { }

        public TileGrid(int cellWidth,
                        int cellHeight,
                        int offsetX,
                        int offsetY,
                        int anchorX,
                        int anchorY,
                        int cellPaddingX,
                        int cellPaddingY,
                        params GridTileDefinition[] tiles)
        {
            Tiles = new List<GridTileDefinition>(tiles);
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            OffsetX = offsetX;
            OffsetY = offsetY;
            AnchorX = anchorX;
            AnchorY = anchorY;
            CellPaddingX = cellPaddingX;
            CellPaddingY = cellPaddingY;
        }

        public ReadOnlyListWrapper<GridTileDefinition> Tiles { get; }
        public int CellWidth { get; }
        public int CellHeight { get; }
        public int OffsetX { get; }
        public int OffsetY { get; }
        public int AnchorX { get; }
        public int AnchorY { get; }
        public int CellPaddingX { get; }
        public int CellPaddingY { get; }
    }
}
