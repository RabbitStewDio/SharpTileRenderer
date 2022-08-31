using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.Drawing.Queries;
using SharpTileRenderer.Drawing.ViewPorts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Rendering
{
    public class ViewportRendering
    {
        readonly SemaphoreSlim syncRoot;
        readonly IViewPort vp;
        readonly List<QueryPlan> queryPlanBuffer;
        readonly List<ConfiguredTaskAwaitable> taskBuffer;
        readonly List<ConfiguredValueTaskAwaitable> valueTaskBuffer;
        bool first;

        public ViewportRendering(IViewPort vp)
        {
            this.syncRoot = new SemaphoreSlim(1, 1);
            this.vp = vp;
            this.taskBuffer = new List<ConfiguredTaskAwaitable>();
            this.valueTaskBuffer = new List<ConfiguredValueTaskAwaitable>();
            this.queryPlanBuffer = new List<QueryPlan>();
        }

        public void Render(params ILayer[] layer)
        {
            var queryPlaner = QueryPlaner.FromTileType(vp.GridType);
            queryPlanBuffer.Clear();
            queryPlaner.Plan(vp, queryPlanBuffer);

            if (!first)
            {
                foreach (var p in queryPlanBuffer)
                {
                    Console.WriteLine("QueryPlan: " + p);
                }

                first = true;
            }

            foreach (var l in layer)
            {
                l.PrepareRenderLayer(vp, queryPlanBuffer);
            }

            foreach (var l in layer)
            {
                l.RenderLayer(vp);
            }
        }

        public Task RenderAsync(params ILayer[] layer) => RenderAsync(layer, CancellationToken.None);

        public async Task RenderAsync(ILayer[] layer, CancellationToken cancelToken)
        {
            await syncRoot.WaitAsync(cancelToken);
            try
            {
                var queryPlaner = QueryPlaner.FromTileType(vp.GridType);
                queryPlanBuffer.Clear();
                queryPlaner.Plan(vp, queryPlanBuffer);

                taskBuffer.Clear();
                foreach (var l in layer)
                {
                    if (l.ThreadSafePreparation)
                    {
                        taskBuffer.Add(Task.Run(() => PrepareRenderLayerAsync(l, cancelToken), cancelToken).ConfigureAwait(true));
                    }
                    else
                    {
                        taskBuffer.Add(PrepareRenderLayerAsync(l, cancelToken).ConfigureAwait(true));
                    }
                }

                foreach (var t in taskBuffer)
                {
                    await t;
                }

                valueTaskBuffer.Clear();
                foreach (var l in layer)
                {
                    valueTaskBuffer.Add(l.RenderLayerAsync(vp, cancelToken).ConfigureAwait(true));
                }

                foreach (var t in valueTaskBuffer)
                {
                    await t;
                }
            }
            finally
            {
                syncRoot.Release();
            }
        }

        Task PrepareRenderLayerAsync(ILayer l, CancellationToken tk)
        {
            return l.PrepareRenderLayerAsync(vp, queryPlanBuffer, tk).AsTask();
        }
    }
}