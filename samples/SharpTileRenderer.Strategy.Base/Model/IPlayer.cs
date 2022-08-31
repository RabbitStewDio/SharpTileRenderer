namespace SharpTileRenderer.Strategy.Base.Model
{
    public interface IPlayer
    {
        string Name { get; }
        PlayerColor PlayerColor { get; }
        Culture Culture { get; }
    }
}