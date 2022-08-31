using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TileMatching;

namespace SharpTileRenderer.TexturePack.Tiles
{
    public class TileProducer<TTexture> : TileProducerBase<TexturedTile<TTexture>, TTexture>
        where TTexture : ITexture<TTexture>
    {
        public TileProducer(ITextureOperations<TTexture> textureOperations, 
                            ITextureAtlasBuilder<TTexture>? atlasBuilder = null) :
            base(textureOperations, atlasBuilder)
        {
        }

        protected override TexturedTile<TTexture> CreateTile(SpriteTag tag, TTexture texture, IntDimension tileSize, IntPoint anchor)
        {
            return new TexturedTile<TTexture>(tag, texture, anchor);
        }
    }
}