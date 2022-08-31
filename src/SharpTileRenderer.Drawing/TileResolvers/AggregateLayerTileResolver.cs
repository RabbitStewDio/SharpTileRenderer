using SharpTileRenderer.TileMatching.DataSets;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.TileResolvers
{
    public class AggregateLayerTileResolver<TQueryResult, TEntity> : ILayerTileResolver<TQueryResult, TEntity>
    {
        readonly List<ILayerTileResolver<TQueryResult, TEntity>> renderers;

        public AggregateLayerTileResolver()
        {
            this.renderers = new List<ILayerTileResolver<TQueryResult, TEntity>>();
            this.IsThreadSafe = true;
        }

        public bool IsThreadSafe { get; private set; }

        public void Add(ILayerTileResolver<TQueryResult, TEntity> r)
        {
            this.renderers.Add(r);
            this.IsThreadSafe &= r.IsThreadSafe;
        }

        public List<RenderInstruction<TEntity>> ResolveTiles(int z,
                                                             List<SparseTagQueryResult<TQueryResult, TEntity>> entities,
                                                             List<RenderInstruction<TEntity>> result)
        {
            foreach (var r in renderers)
            {
                r.ResolveTiles(z, entities, result);
            }

            return result;
        }
    }
}