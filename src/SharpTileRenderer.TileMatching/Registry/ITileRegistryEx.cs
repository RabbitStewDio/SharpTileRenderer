namespace SharpTileRenderer.TileMatching.Registry
{
    public interface ITileRegistryEx<in TSelector, TTile>
    {
        // TTile Find(string tag, TSelector selector);

        bool TryFind(string tag, TSelector selector, out TTile tile);
    }
}
