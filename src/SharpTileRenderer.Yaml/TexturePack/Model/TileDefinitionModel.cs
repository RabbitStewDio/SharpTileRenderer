using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpTileRenderer.Yaml.TexturePack.Model
{
    public class TileDefinitionModel
    {
        public string? Tag { get; set;  }
        public PointModel? CellAnchor { get; set; }
        [Required]
        public PointModel? Position { get; set; }
        [Required]
        public List<string>? Aliases { get; set; }
    }
}