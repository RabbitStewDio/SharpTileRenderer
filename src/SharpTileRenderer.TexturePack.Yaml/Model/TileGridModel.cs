using System.Collections.Generic;

namespace SharpTileRenderer.TexturePack.Yaml.Model
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
        public List<TileDefinitionModel> Tiles { get; }
        public PointModel GridOrigin { get; set; }
        public DimensionModel CellSize { get; set; }
        public DimensionModel CellPadding { get; set; }
        public PointModel CellAnchor { get; set; }
    }
}