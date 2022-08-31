using SharpTileRenderer.Drawing.ViewPorts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Rendering
{
    public class EnrichQuantityTileRenderer<TEntity, TQuantity> : ITileRenderer<TEntity>
    {
        readonly ITileRenderer<(TEntity, TQuantity)> baseline;
        readonly TQuantity defaultQuantity;
        readonly List<ScreenRenderInstruction<(TEntity, TQuantity)>> buffer;

        public EnrichQuantityTileRenderer(ITileRenderer<(TEntity, TQuantity)> baseline, TQuantity defaultQuantity)
        {
            this.baseline = baseline ?? throw new ArgumentNullException(nameof(baseline));
            this.defaultQuantity = defaultQuantity;
            this.buffer = new List<ScreenRenderInstruction<(TEntity, TQuantity)>>();
        }

        public async ValueTask RenderBatchAsync(IViewPort vp,
                                                List<ScreenRenderInstruction<TEntity>> renderInstructionBuffer,
                                                CancellationToken cancellationToken)
        {
            buffer.Clear();
            foreach (var b in renderInstructionBuffer)
            {
                var riBase = b.RenderInstruction;
                var ri = new RenderInstruction<(TEntity, TQuantity)>((riBase.Context, defaultQuantity), riBase.Tag, riBase.SpritePosition, riBase.MapPosition);
                buffer.Add(new ScreenRenderInstruction<(TEntity, TQuantity)>(ri, b.RenderPosition, b.RenderOrder));
            }

            await baseline.RenderBatchAsync(vp, buffer, cancellationToken).ConfigureAwait(true);
            buffer.Clear();
        }

        public void RenderBatch(IViewPort vp, List<ScreenRenderInstruction<TEntity>> renderInstructionBuffer)
        {
            buffer.Clear();
            foreach (var b in renderInstructionBuffer)
            {
                var riBase = b.RenderInstruction;
                var ri = new RenderInstruction<(TEntity, TQuantity)>((riBase.Context, defaultQuantity), riBase.Tag, riBase.SpritePosition, riBase.MapPosition);
                buffer.Add(new ScreenRenderInstruction<(TEntity, TQuantity)>(ri, b.RenderPosition, b.RenderOrder));
            }

            baseline.RenderBatch(vp, buffer);
            buffer.Clear();
        }
    }
}