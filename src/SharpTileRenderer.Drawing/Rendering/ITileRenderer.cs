using SharpTileRenderer.Drawing.ViewPorts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Rendering
{
    public interface ITileRenderer<TEntity>
    {
        ValueTask RenderBatchAsync(IViewPort vp,
                                   List<ScreenRenderInstruction<TEntity>> renderInstructionBuffer,
                                   CancellationToken cancellationToken);

        void RenderBatch(IViewPort vp, List<ScreenRenderInstruction<TEntity>> renderInstructionBuffer);
    }
}