using JetBrains.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Yaml.TexturePack.Model
{
    /// <summary>
    ///    A single cell grid. A grid area is implicitly defined by its various properties.
    ///    The GridOrigin defines the upper-left corner of the grid in the texture.
    ///    Each cell occupies a an area defined by CellSize. Padding space can be inserted
    ///    around cells to avoid texture bleeding during blending or mip-mapping. Note
    ///    that padding will applied around the grid as well.
    ///
    ///    Within each cell, an anchor point can be defined that will map to the center
    ///    of the screen grid cell.
    ///     
    /// </summary>
    public class TileGridModel
    {
        readonly List<TileDefinitionModel> tiles;

        public TileGridModel()
        {
            tiles = new List<TileDefinitionModel>();
        }

        [UsedImplicitly]
        public string? Id { get; set; }
        [UsedImplicitly]
        public DimensionModel? CellSize { get; set; }
        [UsedImplicitly]
        public DimensionModel? CellPadding { get; set; }
        [Required]
        [UsedImplicitly]
        public PointModel? GridOrigin { get; set; }
        [UsedImplicitly]
        public PointModel? CellAnchor { get; set; }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
        public List<TileDefinitionModel> Tiles
        {
            get
            {
                return tiles;
            }
            set
            {
                tiles.Clear();
                if (value != null)
                {
                    tiles.Clear();
                    tiles.AddRange(value);
                }
            }
        }
    }
}