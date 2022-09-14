using SharpTileRenderer.Drawing.Queries;
using SharpTileRenderer.Drawing.ViewPorts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Layers
{
    public interface ILayer
    {
        string Name { get; }
        bool ThreadSafePreparation { get; }
        void PrepareRenderLayer(IViewPort v, List<QueryPlan> p);
        ValueTask PrepareRenderLayerAsync(IViewPort v, List<QueryPlan> queryPlans, CancellationToken tk);
        void RenderLayer(IViewPort v);
        ValueTask RenderLayerAsync(IViewPort v, CancellationToken tk);
    }

    public interface ILayer<TEntity>: ILayer
    {
        IReadOnlyList<ScreenRenderInstruction<TEntity>> RenderBuffer { get; }
    }
}