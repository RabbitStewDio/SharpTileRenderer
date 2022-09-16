using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.Drawing
{
    interface IRenderLayerTypeLift
    {
        Optional<ILayer> Apply<TEntity>(TileMatcherModel model, RenderLayerModel layer, DataContextHolder<TEntity> ctx);
    }


    interface IDataContextHolder
    {
        bool CanHandleFully(RenderLayerModel model);
        Optional<ILayer> Apply(TileMatcherModel model, RenderLayerModel layer, IRenderLayerTypeLift l);
    }
}