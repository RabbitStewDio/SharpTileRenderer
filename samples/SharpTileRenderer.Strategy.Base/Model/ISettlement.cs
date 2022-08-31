using SharpTileRenderer.Strategy.Base.Map;

namespace SharpTileRenderer.Strategy.Base.Model
{
    public interface ISettlement
    {
        string Name { get; }
        Point Location { get; }
        PlayerId Owner { get; }
        long Population { get; }
        bool Walled { get; }
    }
}