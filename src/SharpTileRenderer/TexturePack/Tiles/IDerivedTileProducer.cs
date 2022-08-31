using SharpTileRenderer.TileMatching;

namespace SharpTileRenderer.TexturePack.Tiles
{
    public interface IDerivedTileProducer<TTile, TTexture>
        where TTile : ITexturedTile<TTexture>
        where TTexture : ITexture<TTexture>
    {
        TTile Produce(TTexture texture,
                      IntDimension tileSize,
                      IntPoint anchor,
                      SpriteTag tag);
    }
}