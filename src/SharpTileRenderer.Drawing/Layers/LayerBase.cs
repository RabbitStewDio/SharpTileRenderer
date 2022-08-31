﻿using Microsoft.Extensions.ObjectPool;
using SharpTileRenderer.Drawing.Queries;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Drawing.TileResolvers;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.TileMatching.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Layers
{
    public abstract class LayerBase<TQueryData, TEntity> : ILayer
    {
        readonly ObjectPool<List<ScreenRenderInstruction<TEntity>>> pool;
        readonly List<ScreenRenderInstruction<TEntity>> renderInstructionBuffer;
        readonly List<(ConfiguredValueTaskAwaitable, List<ScreenRenderInstruction<TEntity>>)> taskBuffer;
        readonly List<ScreenPosition> mappingResult;
        readonly RenderingSortOrder renderSortOrder;
        readonly ITileRenderer<TEntity> renderer;
        protected readonly ObjectPool<List<SparseTagQueryResult<TQueryData, TEntity>>> QueryBufferPool;
        protected readonly ILayerTileResolver<TQueryData, TEntity> TileResolver;
        protected readonly ITileDataSet<TQueryData, TEntity> PrimaryDataSet;

        public LayerBase(string name, 
                         ILayerTileResolver<TQueryData, TEntity> tileResolver, 
                         ITileDataSet<TQueryData, TEntity> primaryDataSet,
                         RenderingSortOrder renderSortOrder,
                         ITileRenderer<TEntity> renderer)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.mappingResult = new List<ScreenPosition>();
            this.TileResolver = tileResolver ?? throw new ArgumentNullException(nameof(tileResolver));
            this.PrimaryDataSet = primaryDataSet ?? throw new ArgumentNullException(nameof(primaryDataSet));
            this.renderSortOrder = renderSortOrder;
            this.renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            this.ThreadSafePreparation = tileResolver.IsThreadSafe && primaryDataSet.MetaData.IsThreadSafe;
            
            this.QueryBufferPool = new DefaultObjectPool<List<SparseTagQueryResult<TQueryData, TEntity>>>(new ListObjectPolicy<SparseTagQueryResult<TQueryData, TEntity>>());
            this.renderInstructionBuffer = new List<ScreenRenderInstruction<TEntity>>();
            this.taskBuffer = new List<(ConfiguredValueTaskAwaitable, List<ScreenRenderInstruction<TEntity>>)>();
            this.pool = new DefaultObjectPool<List<ScreenRenderInstruction<TEntity>>>(new ListObjectPolicy<ScreenRenderInstruction<TEntity>>());
        }

        public string Name { get; }

        public async ValueTask PrepareRenderLayerAsync(IViewPort v, List<QueryPlan> queryPlans,  CancellationToken cancelToken)
        {
            renderInstructionBuffer.Clear();
            taskBuffer.Clear();
            foreach (var queryPlan in queryPlans)
            {
                var bufferList = pool.Get();
                var task = PrepareRenderingAsync(v, queryPlan, bufferList, cancelToken).ConfigureAwait(!ThreadSafePreparation);
                taskBuffer.Add((task, bufferList));
            }

            foreach (var (task, renderInstructions) in taskBuffer)
            {
                await task;
                foreach (var l in renderInstructions)
                {
                    renderInstructionBuffer.Add(l);
                }

                pool.Return(renderInstructions);
            }

            await Task.Factory.StartNew(ResolveRenderPosition,
                                        cancelToken, TaskCreationOptions.None,
                                        TaskScheduler.Default)
                      .ConfigureAwait(!ThreadSafePreparation);

            taskBuffer.Clear();
        }

        public void PrepareRenderLayer(IViewPort v, List<QueryPlan> p)
        {
            renderInstructionBuffer.Clear();
            foreach (var queryPlan in p)
            {
                var bufferList = pool.Get();
                PrepareRendering(v, queryPlan, bufferList);
                foreach (var l in bufferList)
                {
                    renderInstructionBuffer.Add(l);
                }

                pool.Return(bufferList);
            }

            ResolveRenderPosition();
        }

        void ResolveRenderPosition()
        {
            var comparer = renderSortOrder.AsComparer<TEntity>();
            renderInstructionBuffer.Sort(comparer);
        }

        protected virtual ValueTask PrepareRenderingAsync(IViewPort v,
                                                          QueryPlan p,
                                                          List<ScreenRenderInstruction<TEntity>> resultBuffer,
                                                          CancellationToken cancellationToken)
        {
            PrepareRendering(v, p, resultBuffer);
            return new ValueTask(Task.CompletedTask);
        }

        protected abstract void PrepareRendering(IViewPort v,
                                                 QueryPlan queryPlan,
                                                 List<ScreenRenderInstruction<TEntity>> resultBuffer);

        public bool ThreadSafePreparation { get; }

        protected virtual ValueTask PostProcessTilesAsync(IViewPort v,
                                                  List<RenderInstruction<TEntity>> tileBuffer,
                                                  List<ScreenRenderInstruction<TEntity>> resultBuffer,
                                                  CancellationToken cancellationToken)
        {
            return new ValueTask(Task.Factory.StartNew(() => PostProcessTiles(v, tileBuffer, resultBuffer), cancellationToken));
        }

        /// <summary>
        ///     Takes all resolved render instructions and computes the relevant screen position for the tile. 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="tileBuffer"></param>
        /// <param name="resultBuffer"></param>
        protected virtual void PostProcessTiles(IViewPort v, 
                                                List<RenderInstruction<TEntity>> tileBuffer, 
                                                List<ScreenRenderInstruction<TEntity>> resultBuffer)
        {
            var gridType = v.GridType;

            for (var index = 0; index < tileBuffer.Count; index++)
            {
                var t = tileBuffer[index];
                var mapPos = t.SpritePosition.Apply(t.MapPosition, gridType);

                // if the screen has wrapping enabled and a full wrap around occurs (ie the full map is visible),
                // any map tile can be rendered multiple times on screen at different positions. 
                mappingResult.Clear();
                v.ScreenSpaceNavigator.MapInverse(v, mapPos, mappingResult);
                for (var i = 0; i < mappingResult.Count; i++)
                {
                    var mr = mappingResult[i];
                    resultBuffer.Add(new ScreenRenderInstruction<TEntity>(t, mr, resultBuffer.Count));
                }
            }
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
    }
}