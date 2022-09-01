using JetBrains.Annotations;
using SharpTileRenderer.TexturePack;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Yaml.TexturePack.Model
{
    /// <summary>
    ///   A collection of textures and their respective descriptive metadata files. 
    /// </summary>
    public class TexturePackModel
    {
        readonly List<string> includes;
        readonly List<TextureFileModel> textureFiles;

        public TexturePackModel()
        {
            includes = new List<string>();
            textureFiles = new List<TextureFileModel>();
        }

        [Required]
        [UsedImplicitly]
        public string? Name { get; set; }

        [UsedImplicitly]
        public string? Author { get; set; }

        [Required]
        [UsedImplicitly]
        public DimensionModel? TileSize { get; set; }

        [UsedImplicitly]
        public TileShape TileShape { get; set; }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
        public List<string> Includes
        {
            get
            {
                return includes;
            }
            set
            {
                includes.Clear();
                if (value != null)
                {
                    includes.Clear();
                    includes.AddRange(value);
                }
            }
        }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
        public List<TextureFileModel> TextureFiles
        {
            get
            {
                return textureFiles;
            }
            set
            {
                textureFiles.Clear();
                
                if (value != null)
                {
                    textureFiles.Clear();
                    textureFiles.AddRange(value);
                }
            }
        }
    }
}