using JetBrains.Annotations;

namespace SharpTileRenderer.Yaml.TexturePack.Model
{
    public class PointModel
    {
        [UsedImplicitly]
        public int X { get; set; }
        [UsedImplicitly]
        public int Y { get; set; }

        public PointModel()
        {
        }

        public PointModel(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}