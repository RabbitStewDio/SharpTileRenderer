using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpTileRenderer.TexturePack.Yaml.Model
{
    /// <summary>
    ///   Represents a single texture. Each texture can  be subdivided into various
    ///   (possibly overlapping) grids. Each grid can have its own cell size to allow
    ///   tile contents that exceed the on-screen grid cell size. 
    /// </summary>
    public class TextureFileModel
    {
        public DimensionModel DefaultCellSize { get; set; }
        public string Name { get; set; }
        [Required]
        public List<TileGridModel> Grids { get; set; }
        
    }
}