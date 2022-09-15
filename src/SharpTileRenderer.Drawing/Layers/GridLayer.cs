using Serilog;
using Serilog.Events;
using SharpTileRenderer.Drawing.Queries;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Drawing.TileResolvers;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Layers
{
    public class GridLayer<TQueryData, TEntity> : LayerBase<TQueryData, TEntity>
    {
        static readonly ILogger logger = SLog.ForContext<GridLayer<TQueryData, TEntity>>();
        readonly List<RenderInstruction<TEntity>> tileBuffer;
        readonly Dictionary<TQueryData, bool> warnNoRenderer;

        public GridLayer(string name,
                         ILayerTileResolver<TQueryData, TEntity> tileResolver,
                         ITileDataSet<TQueryData, TEntity> primaryDataSet,
                         RenderingSortOrder renderSortOrder,
                         ITileRenderer<TEntity> renderer) : base(name, tileResolver, primaryDataSet, renderSortOrder, renderer)
        {
            tileBuffer = new List<RenderInstruction<TEntity>>();
            warnNoRenderer = new Dictionary<TQueryData, bool>();
        }

        /// <summary>
        ///   Todo: Account for isometric and other iteration strategies.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="p"></param>
        /// <param name="resultBuffer"></param>
        /// <param name="cancellationToken"></param>
        protected override ValueTask PrepareRenderingAsync(IViewPort v,
                                                           QueryPlan p,
                                                           List<ScreenRenderInstruction<TEntity>> resultBuffer,
                                                           CancellationToken cancellationToken)
        {
            var queryBuffer = QueryBufferPool.Get();
            try
            {
                var qp = p.ToGridArea();
                PrimaryDataSet.QuerySparse(qp, v.ZLayer, queryBuffer);
                TileResolver.ResolveTiles(v.ZLayer, queryBuffer, tileBuffer);
                return PostProcessTilesAsync(v, tileBuffer, resultBuffer, cancellationToken);
            }
            finally
            {
                QueryBufferPool.Return(queryBuffer);
            }
        }

        protected override void PrepareRendering(IViewPort v, QueryPlan queryPlan, List<ScreenRenderInstruction<TEntity>> resultBuffer)
        {
            var queryBuffer = QueryBufferPool.Get();
            try
            {
                tileBuffer.Clear();

                var qp = queryPlan.ToGridArea();

                if (logger.IsEnabled(LogEventLevel.Debug))
                {
                    for (var y = qp.MinExtentY; y <= qp.MaxExtentY; y += 1)
                    {
                        for (var x = qp.MinExtentX; x <= qp.MaxExtentX; x += 1)
                        {
                            var pt = new ContinuousMapCoordinate(x, y).Normalize();
                            queryBuffer.Clear();
                            PrimaryDataSet.QueryPoint(pt, v.ZLayer, queryBuffer);
                            var tileBufferCountOld = tileBuffer.Count;
                            TileResolver.ResolveTiles(v.ZLayer, queryBuffer, tileBuffer);

                            if (tileBuffer.Count == tileBufferCountOld && queryBuffer.Count != 0)
                            {
                                if (!warnNoRenderer.TryGetValue(queryBuffer[0].TagData, out _))
                                {
                                    logger.Debug("{LayerName}: Skipped rendering of {Coordinate} for query result {QueryResult}", Name, pt, queryBuffer[0]);
                                    warnNoRenderer[queryBuffer[0].TagData] = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    PrimaryDataSet.QuerySparse(qp, v.ZLayer, queryBuffer);
                    TileResolver.ResolveTiles(v.ZLayer, queryBuffer, tileBuffer);
                }

                PostProcessTiles(v, tileBuffer, resultBuffer);
            }
            finally
            {
                QueryBufferPool.Return(queryBuffer);
            }
        }
    }
}