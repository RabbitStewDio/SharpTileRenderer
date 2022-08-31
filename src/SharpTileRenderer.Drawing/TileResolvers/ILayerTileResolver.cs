using SharpTileRenderer.TileMatching.DataSets;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.TileResolvers
{
    public interface ILayerTileResolver<TQueryData, TEntity>
    {
        bool IsThreadSafe { get; }

        List<RenderInstruction<TEntity>> ResolveTiles(int z,
                                                      List<SparseTagQueryResult<TQueryData, TEntity>> entities,
                                                      List<RenderInstruction<TEntity>> result);
    }
}