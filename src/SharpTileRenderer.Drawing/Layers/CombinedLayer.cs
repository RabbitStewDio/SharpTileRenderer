using SharpTileRenderer.Drawing.Queries;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Drawing.Utils;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Layers
{
    public class CombinedLayer<TEntity> : ILayer<TEntity>
    {
        readonly ITileRenderer<TEntity> renderer;
        readonly List<ScreenRenderInstruction<TEntity>> renderInstructionBuffer;
        readonly List<ILayer<TEntity>> threadSafeLayers;
        readonly List<ILayer<TEntity>> nonThreadSafeLayers;
        readonly List<ILayer<TEntity>> allLayers;
        readonly BinaryHeap<(int, ScreenRenderInstruction<TEntity>)> sortHeap;
        readonly List<ConfiguredValueTaskAwaitable> taskBuffer;
        readonly SortComparer sortComparer;

        public CombinedLayer(string name,
                             RenderingSortOrder renderSortOrder,
                             ITileRenderer<TEntity> renderer,
                             params ILayer<TEntity>[] layers)
        {
            this.threadSafeLayers = new List<ILayer<TEntity>>();
            this.nonThreadSafeLayers = new List<ILayer<TEntity>>();
            this.allLayers = new List<ILayer<TEntity>>();
            this.renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            this.renderInstructionBuffer = new List<ScreenRenderInstruction<TEntity>>();
            this.taskBuffer = new List<ConfiguredValueTaskAwaitable>();
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.ThreadSafePreparation = ComputeThreadSafe(layers);

            this.sortComparer = new SortComparer(renderSortOrder.AsComparer<TEntity>());
            this.sortHeap = new BinaryHeap<(int, ScreenRenderInstruction<TEntity>)>(layers.Length, sortComparer);
        }

        bool ComputeThreadSafe(ILayer<TEntity>[] layers)
        {
            var threadSafe = true;
            foreach (var l in layers)
            {
                allLayers.Add(l);
                if (l.ThreadSafePreparation)
                {
                    threadSafeLayers.Add(l);
                }
                else
                {
                    nonThreadSafeLayers.Add(l);
                    threadSafe = false;
                }
            }

            return threadSafe;
        }

        public string Name { get; }
        public bool ThreadSafePreparation { get; }
        public IReadOnlyList<ScreenRenderInstruction<TEntity>> RenderBuffer => renderInstructionBuffer;

        public void PrepareRenderLayer(IViewPort v, List<QueryPlan> p)
        {
            taskBuffer.Clear();
            
            void PrepareRenderLocalBody(ILayer obj)
            {
                obj.PrepareRenderLayer(v, p);
            }

            if (threadSafeLayers.Count > 0)
            {
                var r = Parallel.ForEach(threadSafeLayers, PrepareRenderLocalBody);
                if (!r.IsCompleted) throw new Exception();
            }

            foreach (var l in nonThreadSafeLayers)
            {
                l.PrepareRenderLayer(v, p);
            }

            MergeSorted(allLayers, renderInstructionBuffer);
        }
        
        public async ValueTask PrepareRenderLayerAsync(IViewPort v, List<QueryPlan> queryPlans, CancellationToken tk)
        {
            taskBuffer.Clear();
            foreach (var l in nonThreadSafeLayers)
            {
                var f = l.PrepareRenderLayerAsync(v, queryPlans, tk).ConfigureAwait(ThreadSafePreparation);
                taskBuffer.Add(f);
            }

            foreach (var task in taskBuffer)
            {
                await task;
            }

            MergeSorted(allLayers, renderInstructionBuffer);
        }

        public virtual ValueTask RenderLayerAsync(IViewPort vp,
                                                  CancellationToken cancellationToken)
        {
            return renderer.RenderBatchAsync(vp, renderInstructionBuffer, cancellationToken);
        }

        public virtual void RenderLayer(IViewPort vp)
        {
            renderer.RenderBatch(vp, renderInstructionBuffer);
        }

        void MergeSorted(IReadOnlyList<ILayer<TEntity>> lists, List<ScreenRenderInstruction<TEntity>> resultBuffer)
        {
            var totalElements = 0;
            for (var index = 0; index < lists.Count; index++)
            {
                var l = lists[index];
                totalElements += l.RenderBuffer.Count;
            }

            resultBuffer.Clear();
            resultBuffer.Capacity = Math.Max(resultBuffer.Capacity, totalElements + 1);

            if (lists.Count == 0)
            {
                return;
            }


            sortHeap.Clear();
            var listOffsets = ArrayPool<int>.Shared.Rent(lists.Count);
            for (int c = 0; c < lists.Count; c += 1)
            {
                var singleList = lists[c].RenderBuffer;
                if (singleList.Count > 0)
                {
                    sortHeap.Add((c, singleList[0]));
                }

                listOffsets[c] = 0;
            }

            while (sortHeap.Size > 0)
            {
                var (listIdx, min) = sortHeap.Remove();
                resultBuffer.Add(min);
                var list = lists[listIdx];
                listOffsets[listIdx] += 1;
                var offset = listOffsets[listIdx];
                if (offset < list.RenderBuffer.Count)
                {
                    sortHeap.Add((listIdx, list.RenderBuffer[offset]));
                }
            }
        }

        class SortComparer : IComparer<(int, ScreenRenderInstruction<TEntity>)>
        {
            readonly IComparer<ScreenRenderInstruction<TEntity>> parent;

            public SortComparer(IComparer<ScreenRenderInstruction<TEntity>> parent)
            {
                this.parent = parent;
            }

            public int Compare((int, ScreenRenderInstruction<TEntity>) x, (int, ScreenRenderInstruction<TEntity>) y)
            {
                return parent.Compare(x.Item2, y.Item2) switch
                {
                    < 0 => -1,
                    > 0 => +1,
                    0 => x.Item1.CompareTo(y.Item1)
                };
            }
        }
    }
}