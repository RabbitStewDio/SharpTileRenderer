using JetBrains.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Yaml.TexturePack.Model
{
    /// <summary>
    ///   Represents a single texture. Each texture can  be subdivided into various
    ///   (possibly overlapping) grids. Each grid can have its own cell size to allow
    ///   tile contents that exceed the on-screen grid cell size. 
    /// </summary>
    public class TextureFileModel
    {
        readonly List<TileGridModel> grids;

        public TextureFileModel()
        {
            grids = new List<TileGridModel>();
        }

        [UsedImplicitly]
        public DimensionModel? DefaultCellSize { get; set; }
        [UsedImplicitly]
        public string? Name { get; set; }

        [Required]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
        public List<TileGridModel> Grids
        {
            get
            {
                return grids;
            }
            set
            {
                grids.Clear();
                
                if (value != null)
                {
                    grids.Clear();
                    grids.AddRange(value);
                }
            }
        }
    }
}