using SharpTileRenderer.Navigation;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.DataSets
{
    /// <summary>
    ///   A sparse data set performs a bulk query over a given map region. The data returned is
    ///   denormalized - an entity with multiple matching tags with contribute multiple times to the
    ///   result set, once for each tag/class.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface ITileDataSet<TData, TEntity>: ITileDataSet
    {
        /// <summary>
        ///   Queries a given sparsely populated map area. this query is intended to be handled by some
        ///   form of spatial sweep or bulk lookup like a box cast.  
        /// </summary>
        /// <param name="area"></param>
        /// <param name="z"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        List<SparseTagQueryResult<TData, TEntity>> QuerySparse(in ContinuousMapArea area,
                                                               int z,
                                                               List<SparseTagQueryResult<TData, TEntity>>? result = null);

        /// <summary>
        ///   Queries the area of a single map tile. If the data layer contains freely positionable elements, this query must return
        ///   all elements in the 1 by 1 square containing the given map location. 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="z"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        List<SparseTagQueryResult<TData, TEntity>> QueryPoint(in MapCoordinate location,
                                                              int z,
                                                              List<SparseTagQueryResult<TData, TEntity>>? result = null);
    }

    /// <summary>
    ///   A sparse data set performs a bulk query over a given map region. The data returned is
    ///   denormalized - an entity with multiple matching tags with contribute multiple times to the
    ///   result set, once for each tag/class.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface IContextTileDataSet<TData, TEntity>: ITileDataSet
    {
        /// <summary>
        ///   Queries the area of a single map tile. If the data layer contains freely positionable elements, this query must return
        ///   all elements in the 1 by 1 square containing the given map location. 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="z"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        List<SparseTagQueryResult<TData, TEntity>> QueryPoint(in MapCoordinate location,
                                                              int z,
                                                              List<SparseTagQueryResult<TData, TEntity>>? result = null);
    }

    public interface ITileDataSet
    {
        ITileDataSetMetaData MetaData { get; }
    }
}