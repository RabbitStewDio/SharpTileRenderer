using SharpTileRenderer.TileMatching.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpTileRenderer.TexturePack.Model
{
    /// <summary>
    ///   A texture file that contains one ore more regular grids. All tiles
    ///   inside each grid have a uniform size and anchor point. FreeCiv uses
    ///   this schema for its graphics packs.
    ///
    ///   This is purely a data file. 
    /// </summary>
    public class SpriteSheetTileCollection : ModelBase
    {
        string? textureAssetName;

        public SpriteSheetTileCollection()
        {
            Grids = new ObservableCollection<SpriteSheetTileGrid>();
            RegisterObservableList(nameof(Grids), Grids);
        }

        public SpriteSheetTileCollection(string textureName,
                                         params SpriteSheetTileGrid[] grids) : this()
        {
            if (grids == null)
            {
                throw new ArgumentNullException(nameof(grids));
            }

            TextureAssetName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            foreach (var g in grids)
            {
                Grids.Add(g);
            }
        }

        public string? TextureAssetName
        {
            get
            {
                return textureAssetName;
            }
            set
            {
                if (value == textureAssetName) return;
                textureAssetName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SpriteSheetTileGrid> Grids { get; }

        public IReadOnlyList<TexturedTileSpec> ProduceTiles()
        {
            if (string.IsNullOrEmpty(TextureAssetName))
            {
                return Array.Empty<TexturedTileSpec>();
            }

            var q = Grids.SelectMany(grid => grid.Tiles, CombineGridAndTile)
                         .Select(ComputePosition)
                         .Select(t => new TexturedTileSpec(TextureAssetName,
                                                           new IntRect(t.tileX, t.tileY, t.grid.CellWidth, t.grid.CellHeight),
                                                           new IntPoint(t.tile.AnchorX ?? t.grid.AnchorX, t.tile.AnchorY ?? t.grid.AnchorY),
                                                           t.tile.Tags));
            return q.ToList();
        }

        static (SpriteSheetTileGrid grid, SpriteSheetTileDefinition tile) CombineGridAndTile(SpriteSheetTileGrid grid, SpriteSheetTileDefinition tile)
        {
            return (grid, tile);
        }

        static (SpriteSheetTileGrid grid, SpriteSheetTileDefinition tile, int tileX, int tileY) ComputePosition((SpriteSheetTileGrid grid, SpriteSheetTileDefinition tile) gridWithTiles)
        {
            return (gridWithTiles.grid, gridWithTiles.tile,
                    gridWithTiles.grid.OffsetX + gridWithTiles.tile.GridX * (gridWithTiles.grid.CellWidth + gridWithTiles.grid.CellPaddingX),
                    gridWithTiles.grid.OffsetY + gridWithTiles.tile.GridY * (gridWithTiles.grid.CellHeight + gridWithTiles.grid.CellPaddingY)
                );
        }
    }
}