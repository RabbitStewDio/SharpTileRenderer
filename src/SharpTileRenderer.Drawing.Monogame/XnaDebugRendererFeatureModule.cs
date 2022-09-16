using Microsoft.Xna.Framework.Graphics;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.Drawing.Monogame
{
    public class XnaDebugRendererFeatureModule : IDrawingFeatureModule
    {
        readonly SpriteFont font;

        public XnaDebugRendererFeatureModule(SpriteFont font)
        {
            this.font = font;
        }

        public int PreferenceWeight => 1000;

        public void Initialize<TClassification>(IFeatureInitializer<TClassification> initializer) where TClassification : struct, IEntityClassification<TClassification>
        {
        }

        public Optional<ITileRenderer<TEntity>> CreateRendererForData<TEntity, TClassification>(IRenderLayerProducerConfig<TClassification> layerProducer,
                                                                                                TileMatcherModel model,
                                                                                                RenderLayerModel layer)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (!layerProducer.TryGetFeature<XnaRendererFeatureModule>(out var xnaModule))
            {
                return default;
            }

            if (!xnaModule.TileSet.TryGetValue(out var tileSet))
            {
                return default;
            }

            var graphics = xnaModule.GraphicsDevice;
            if (xnaModule.CreateRendererForData<TEntity, TClassification>(layerProducer, model, layer).TryGetValue(out var parent))
            {
                return new DebugSpriteBatchTileRenderer<TEntity, TexturedTile<XnaTexture>>(parent, graphics, tileSet, font);
            }

            return new DebugSpriteBatchTileRenderer<TEntity, TexturedTile<XnaTexture>>(null, graphics, tileSet, font);
        }
    }
}