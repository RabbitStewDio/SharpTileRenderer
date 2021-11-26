namespace SharpTileRenderer.TexturePack
{
    public interface IContentLoader<TRawTexture>
    {
        TRawTexture LoadTexture(string name);
    }
}
