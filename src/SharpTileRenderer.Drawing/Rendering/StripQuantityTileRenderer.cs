using SharpTileRenderer.Drawing.ViewPorts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Rendering
{
    public class StripQuantityTileRenderer<TEntity, TQuantity> : ITileRenderer<(TEntity, TQuantity)>
    {
        readonly ITileRenderer<TEntity> baseline;
        readonly List<ScreenRenderInstruction<TEntity>> buffer;

        public StripQuantityTileRenderer(ITileRenderer<TEntity> baseline)
        {
            this.baseline = baseline ?? throw new ArgumentNullException(nameof(baseline));
            this.buffer = new List<ScreenRenderInstruction<TEntity>>();
        }

        public async ValueTask RenderBatchAsync(IViewPort vp, 
                                                List<ScreenRenderInstruction<(TEntity, TQuantity)>> renderInstructionBuffer,
                                                CancellationToken cancellationToken)
        {
            buffer.Clear();
            foreach (var b in renderInstructionBuffer)
            {
                var renderInstruction = b.RenderInstruction;
                var ri = new RenderInstruction<TEntity>(renderInstruction.Context.Item1, renderInstruction.Tag, renderInstruction.SpritePosition, renderInstruction.MapPosition);
                buffer.Add(new ScreenRenderInstruction<TEntity>(ri, b.RenderPosition, b.RenderOrder));
            }

            await baseline.RenderBatchAsync(vp, buffer, cancellationToken).ConfigureAwait(true);
            buffer.Clear();
        }

        public void RenderBatch(IViewPort vp, List<ScreenRenderInstruction<(TEntity, TQuantity)>> renderInstructionBuffer)
        {
            buffer.Clear();
            foreach (var b in renderInstructionBuffer)
            {
                var renderInstruction = b.RenderInstruction;
                var ri = new RenderInstruction<TEntity>(renderInstruction.Context.Item1, renderInstruction.Tag, renderInstruction.SpritePosition, renderInstruction.MapPosition);
                buffer.Add(new ScreenRenderInstruction<TEntity>(ri, b.RenderPosition, b.RenderOrder));
            }

            baseline.RenderBatch(vp, buffer);
            buffer.Clear();
        }
    }
}