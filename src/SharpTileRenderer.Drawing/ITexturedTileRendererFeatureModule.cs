using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.Drawing
{
    public interface ITexturedTileRendererFeatureModule<TTexture> : IFeatureModule 
        where TTexture : ITexture<TTexture>
    {
        public Optional<ITileResolver<SpriteTag, TexturedTile<TTexture>>> TileSet { get; }
        
        Optional<ITileRenderer<TEntity>> CreateRendererForData<TEntity, TClassification>(IRenderLayerProducerConfig<TClassification> layerProducer,
                                                                                         TileMatcherModel model,
                                                                                         RenderLayerModel layer,
                                                                                         ITileResolver<SpriteTag, TexturedTile<TTexture>> tileSet)
            where TClassification : struct, IEntityClassification<TClassification>;
    }
}