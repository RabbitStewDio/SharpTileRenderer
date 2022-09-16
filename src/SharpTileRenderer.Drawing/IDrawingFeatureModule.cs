using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.Drawing
{
    public interface IDrawingFeatureModule : IFeatureModule
    {
        Optional<ITileRenderer<TEntity>> CreateRendererForData<TEntity, TClassification>(IRenderLayerProducerConfig<TClassification> layerProducer,
                                                                                         TileMatcherModel model,
                                                                                         RenderLayerModel layer)
            where TClassification : struct, IEntityClassification<TClassification>;
    }
}