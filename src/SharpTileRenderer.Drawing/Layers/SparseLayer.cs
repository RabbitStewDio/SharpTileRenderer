﻿using Microsoft.Extensions.ObjectPool;
using SharpTileRenderer.Drawing.Queries;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Drawing.TileResolvers;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.Util;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Layers
{
    public class SparseLayer<TQueryData, TEntity> : LayerBase<TQueryData, TEntity>
    {
        readonly ObjectPool<List<RenderInstruction<TEntity>>> tileBufferPool;

        public SparseLayer(string name,
                           ILayerTileResolver<TQueryData, TEntity> tileResolver,
                           ITileDataSet<TQueryData, TEntity> primaryDataSet,
                           RenderingSortOrder renderSortOrder,
                           ITileRenderer<TEntity> renderer) : base(name, tileResolver, primaryDataSet, renderSortOrder, renderer)
        {
            tileBufferPool = new DefaultObjectPool<List<RenderInstruction<TEntity>>>(new ListObjectPolicy<RenderInstruction<TEntity>>());
        }

        protected override void PrepareRendering(IViewPort v,
                                                 QueryPlan queryPlan,
                                                 List<ScreenRenderInstruction<TEntity>> resultBuffer)
        {
            var qp = queryPlan.ToGridArea();
            var queryBuffer = QueryBufferPool.Get();
            var tileBuffer = tileBufferPool.Get();
            try
            {
                tileBuffer.Clear();
                queryBuffer.Clear();
                this.PrimaryDataSet.QuerySparse(qp, v.ZLayer, queryBuffer);
                this.TileResolver.ResolveTiles(v.ZLayer, queryBuffer, tileBuffer);

                PostProcessTiles(v, tileBuffer, resultBuffer);
            }
            finally
            {
                tileBufferPool.Return(tileBuffer);
                QueryBufferPool.Return(queryBuffer);
            }
        }

        protected override ValueTask PrepareRenderingAsync(IViewPort v,
                                                           QueryPlan p,
                                                           List<ScreenRenderInstruction<TEntity>> resultBuffer,
                                                           CancellationToken cancellationToken)
        {
            var qp = p.ToGridArea();
            var queryBuffer = QueryBufferPool.Get();
            var tileBuffer = tileBufferPool.Get();

            try
            {
                tileBuffer.Clear();
                queryBuffer.Clear();
                this.PrimaryDataSet.QuerySparse(qp, v.ZLayer, queryBuffer);
                this.TileResolver.ResolveTiles(v.ZLayer, queryBuffer, tileBuffer);

                return PostProcessTilesAsync(v, tileBuffer, resultBuffer, cancellationToken);
            }
            finally
            {
                tileBufferPool.Return(tileBuffer);
                QueryBufferPool.Return(queryBuffer);
            }
        }
    }
}