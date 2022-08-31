using JetBrains.Annotations;
using SharpTileRenderer.TexturePack;

namespace SharpTileRenderer.Yaml.TexturePack.Model
{
    public class DimensionModel
    {
        [UsedImplicitly]
        public int Width { get; set; }
        [UsedImplicitly]
        public int Height { get; set; }

        public DimensionModel()
        {
        }

        public DimensionModel(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static implicit operator IntDimension(DimensionModel m)
        {
            return new IntDimension(m.Width, m.Height);
        }  

        public static implicit operator DimensionModel(IntDimension m)
        {
            return new DimensionModel(m.Width, m.Height);
        }  

    }
}