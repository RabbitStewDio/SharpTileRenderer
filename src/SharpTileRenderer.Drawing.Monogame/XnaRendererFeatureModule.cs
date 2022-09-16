using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.Drawing.Monogame
{
    public class XnaRendererFeatureModule : IDrawingFeatureModule, ITexturedTileRendererFeatureModule<XnaTexture>

    {
        internal readonly GraphicsDeviceManager GraphicsDevice;

        XnaRendererFeatureModule(GraphicsDeviceManager graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
        }

        public void Initialize<TClassification>(IFeatureInitializer<TClassification> initializer)
            where TClassification : struct, IEntityClassification<TClassification>
        {
        }

        public int PreferenceWeight => 0;

        public Optional<ITileRenderer<TEntity>> CreateRendererForData<TEntity, TClassification>(IRenderLayerProducerConfig<TClassification> layerProducer,
                                                                                                TileMatcherModel model,
                                                                                                RenderLayerModel layer)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (!TileSet.TryGetValue(out var tileSet))
            {
                return default;
            }

            return new SpriteBatchTileRenderer<TEntity, TexturedTile<XnaTexture>>(layer.Id ?? "", GraphicsDevice, tileSet, SpriteBatch);
        }

        public Optional<ITileRenderer<TEntity>> CreateRendererForData<TEntity, TClassification>(IRenderLayerProducerConfig<TClassification> layerProducer, 
                                                                                                TileMatcherModel model, 
                                                                                                RenderLayerModel layer, 
                                                                                                ITileResolver<SpriteTag, TexturedTile<XnaTexture>> tileSet) where TClassification : struct, IEntityClassification<TClassification>
        {
            return new SpriteBatchTileRenderer<TEntity, TexturedTile<XnaTexture>>(layer.Id ?? "", GraphicsDevice, tileSet, SpriteBatch);
        }

        public XnaRendererFeatureModule WithTileSet(ITileResolver<SpriteTag, TexturedTile<XnaTexture>> tileSet)
        {
            this.TileSet = Optional.OfNullable(tileSet);
            return this;
        }

        public XnaRendererFeatureModule WithSharedSpriteBatch(SpriteBatch batch)
        {
            this.SpriteBatch = batch;
            return this;
        }

        public static XnaRendererFeatureModule For(GraphicsDeviceManager mgr) => new XnaRendererFeatureModule(mgr);

        public Optional<ITileResolver<SpriteTag, TexturedTile<XnaTexture>>> TileSet { get; private set; }

        public SpriteBatch? SpriteBatch { get; private set; }
    }
}