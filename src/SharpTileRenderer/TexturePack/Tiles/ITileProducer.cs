using SharpTileRenderer.TileMatching;

namespace SharpTileRenderer.TexturePack.Tiles
{
    public interface ITileProducer<TTile, TTexture>
        where TTile : ITexturedTile<TTexture>
        where TTexture : ITexture<TTexture>
    {
        TTile Produce(TTexture texture,
                      IntDimension tileSize,
                      IntRect gridBounds,
                      IntPoint anchor,
                      SpriteTag tag);
    }
}