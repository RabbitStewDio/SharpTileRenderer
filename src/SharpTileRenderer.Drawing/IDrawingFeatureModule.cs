using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Drawing
{
    public interface IDrawingFeatureModule : IFeatureModule
    {
        bool CreateRendererForData<TEntity, TClassification>(RenderLayerProducerData<TClassification> layerProducer,
                                                             ITileDataSetProducer<TEntity> dataSet,
                                                             [MaybeNullWhen(false)] out IRenderLayerProducer<TClassification> c)
            where TClassification : struct, IEntityClassification<TClassification>;
    }
}