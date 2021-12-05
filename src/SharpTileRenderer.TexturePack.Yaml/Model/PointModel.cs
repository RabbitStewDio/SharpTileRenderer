namespace SharpTileRenderer.TexturePack.Yaml.Model
{
    public class PointModel
    {
        public int X { get; set; }
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