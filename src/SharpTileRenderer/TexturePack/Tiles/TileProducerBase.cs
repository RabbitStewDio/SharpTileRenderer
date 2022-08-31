using JetBrains.Annotations;
using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TileMatching;
using System;

namespace SharpTileRenderer.TexturePack.Tiles
{
    /// <summary>
    ///    This is an extension point. Implement this for your specific engine.
    /// </summary>
    /// <typeparam name="TTile"></typeparam>
    /// <typeparam name="TTexture"></typeparam>
    [UsedImplicitly]
    public abstract class TileProducerBase<TTile, TTexture> : ITileProducer<TTile, TTexture>,
                                                              IDerivedTileProducer<TTile, TTexture>
        where TTile : ITexturedTile<TTexture>
        where TTexture : ITexture<TTexture>
    {
        readonly ITextureOperations<TTexture> textureOperations;
        readonly ITextureAtlasBuilder<TTexture> textureAtlas;

        protected TileProducerBase(ITextureOperations<TTexture> textureOperations,
                                   ITextureAtlasBuilder<TTexture>? atlasBuilder = null)
        {
            this.textureOperations = textureOperations ?? throw new ArgumentNullException(nameof(textureOperations));
            this.textureAtlas = atlasBuilder ?? new NoOpTextureAtlasBuilder<TTexture>();
        }

        protected abstract TTile CreateTile(SpriteTag tag, TTexture texture, IntDimension tileSize, IntPoint anchor);

        public TTile Produce(TTexture texture, IntDimension tileSize, IntRect gridBounds, IntPoint anchor, SpriteTag tag)
        {
            var subTextureName = tag + "@" + texture.Name;
            var subTextureBounds = textureOperations.ToNormalized(texture.Bounds.Size, texture.Bounds)
                                                    .Clip(new TextureCoordinateRect(gridBounds.X, gridBounds.Y, gridBounds.Width, gridBounds.Height));
            var nativeBounds = textureOperations.ToNative(texture.Bounds.Size, subTextureBounds);
            var nativeTexture = texture.CreateSubTexture(subTextureName, nativeBounds);
            var atlasTexture = textureAtlas.Add(nativeTexture);
            return CreateTile(tag, atlasTexture, tileSize, anchor);
        }

        public TTile Produce(TTexture texture, IntDimension tileSize, IntPoint anchor, SpriteTag tag)
        {
            var atlasTexture = textureAtlas.Add(texture);
            return CreateTile(tag, atlasTexture, tileSize, anchor);
        }
    }
}