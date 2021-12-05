using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpTileRenderer.TexturePack.Yaml.Model
{
    /// <summary>
    ///   A collection of textures and their respective descriptive metadata files. 
    /// </summary>
    public class TexturePackModel
    {
        [Required]
        public string Name { get; set; }
        public List<TextureFileModel> TextureFiles { get; set; }
        [Required]
        public DimensionModel TileSize { get; set; }
        public TextureType TextureType { get; set; }
        public List<string> Includes { get; set; }
    }
}