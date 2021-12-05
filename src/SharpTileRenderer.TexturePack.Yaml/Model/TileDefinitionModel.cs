using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpTileRenderer.TexturePack.Yaml.Model
{
    public class TileDefinitionModel
    {
        public string Name { get; }
        public PointModel CellAnchor { get; set; }
        [Required]
        public PointModel Position { get; set; }
        [Required]
        public List<string> Tags { get; set; }
    }
}