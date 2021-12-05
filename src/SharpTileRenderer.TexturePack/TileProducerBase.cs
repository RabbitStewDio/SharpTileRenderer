using SharpTileRenderer.TexturePack.Operations;

namespace SharpTileRenderer.TexturePack
{
    public abstract class TileProducerBase<TTile, TTexture, TRawTexture> : ITileProducer<TTile, TTexture, TRawTexture>,
                                                                           IDerivedTileProducer<TTile, TTexture>
        where TTile : ITexturedTile<TTexture>
        where TTexture : ITexture
        where TRawTexture : IRawTexture<TTexture>
    {
        readonly ITextureOperations<TTexture> textureOperations;
        readonly ITextureAtlasBuilder<TTexture> textureAtlas;

        protected TileProducerBase(ITextureOperations<TTexture> textureOperations,
                                   ITextureAtlasBuilder<TTexture> atlasBuilder = null)
        {
            this.textureOperations = textureOperations;
            textureAtlas = atlasBuilder ?? new NoOpTextureAtlasBuilder<TTexture>();
        }

        protected abstract TTile CreateTile(string tag, TTexture texture, IntDimension tileSize, IntPoint anchor);

        public TTile Produce(TRawTexture texture, IntDimension tileSize, IntRect gridBounds, IntPoint anchor, string tag)
        {
            var subTextureName = tag + "@" + texture.Name;
            var subTextureBounds = texture.Bounds.Clip(gridBounds);
            var nativeBounds = textureOperations.ToNative(texture.Bounds.Size, subTextureBounds);
            var nativeTexture = texture.CreateSubTexture(subTextureName, nativeBounds);
            var atlasTexture = textureAtlas.Add(nativeTexture);
            return CreateTile(tag, atlasTexture, tileSize, anchor);
        }

        public TTile Produce(TTexture texture, IntDimension tileSize, IntPoint anchor, string tag)
        {
            var atlasTexture = textureAtlas.Add(texture);
            return CreateTile(tag, atlasTexture, tileSize, anchor);
        }
    }
}