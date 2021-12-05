using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpTileRenderer.TexturePack.Grids
{
    /// <summary>
    ///   A texture file that contains one ore more regular grids. All tiles
    ///   inside each grid have a uniform size and anchor point. FreeCiv uses
    ///   this schema for its graphics packs.
    ///
    ///   This is purely a data file. 
    /// </summary>
    public class GridTileCollection : ITileCollection
    {
        public string TextureAssetName { get; }
        public List<TileGrid> Grids { get; }

        public GridTileCollection(string name,
                                  params TileGrid[] grids)
        {
            if (grids == null)
            {
                throw new ArgumentNullException(nameof(grids));
            }

            TextureAssetName = name ?? throw new ArgumentNullException(nameof(name));
            Grids = new List<TileGrid>(grids);
        }

        public IEnumerable<TexturedTileSpec> ProduceTiles()
        {
            return from grid in Grids
                   let tileWidth = grid.CellWidth
                   let tileHeight = grid.CellHeight
                   from tile in grid.Tiles
                   let tileX = grid.OffsetX + tile.GridX * (tileWidth + grid.CellPaddingX)
                   let tileY = grid.OffsetY + tile.GridY * (tileHeight + grid.CellPaddingY)
                   let tileBounds = new IntRect(tileX, tileY, tileWidth, tileHeight)
                   let anchor = new IntPoint(tile.AnchorX ?? grid.AnchorX, tile.AnchorY ?? grid.AnchorY)
                   select new TexturedTileSpec(TextureAssetName, tileBounds, anchor, tile.Tags);
        }
    }
}