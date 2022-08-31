using Microsoft.Extensions.ObjectPool;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.DataSets
{
    public static class DowngradedQuantifiedTagTileDataSet
    {
        public static ITileDataSet<TData, TEntity> Downgrade<TData, TEntity, TQuantity>(this IQuantifiedTagTileDataSet<TData, TEntity, TQuantity> backend)
            where TQuantity : IComparable<TQuantity>
        {
            return new DowngradedQuantifiedTagTileDataSet<TData, TEntity, TQuantity>(backend);
        }
    }
    
    public class DowngradedQuantifiedTagTileDataSet<TData, TEntity, TQuantity> : ITileDataSet<TData, TEntity>
        where TQuantity : IComparable<TQuantity>
    {
        readonly IQuantifiedTagTileDataSet<TData, TEntity, TQuantity> backend;
        readonly ObjectPool<List<SparseTagQueryResult<TData, (TEntity, TQuantity)>>> pool;
        readonly DowngradeMetaData metaData;

        public DowngradedQuantifiedTagTileDataSet(IQuantifiedTagTileDataSet<TData, TEntity, TQuantity> backend)
        {
            this.backend = backend;
            this.metaData = new DowngradeMetaData(backend.MetaData);
            this.pool = new DefaultObjectPool<List<SparseTagQueryResult<TData, (TEntity, TQuantity)>>>(new ListObjectPolicy<SparseTagQueryResult<TData, (TEntity, TQuantity)>>());
        }

        public ITileDataSetMetaData MetaData => metaData;
        
        public List<SparseTagQueryResult<TData, TEntity>> QuerySparse(in ContinuousMapArea area, int z, List<SparseTagQueryResult<TData, TEntity>>? result = null)
        {
            var backendResults = pool.Get();
            try
            {
                backendResults.Clear();
                backend.QuerySparse(area, z, backendResults);
                result ??= new List<SparseTagQueryResult<TData, TEntity>>(backendResults.Count);
                result.Clear();
                for (var i = 0; i < backendResults.Count; i++)
                {
                    var data = backendResults[i];
                    result.Add(new SparseTagQueryResult<TData, TEntity>(data.TagData, data.Entity.Item1, data.Position));
                }

                return result;
            }
            finally
            {
                backendResults.Clear();
                pool.Return(backendResults);
            }
        }

        public List<SparseTagQueryResult<TData, TEntity>> QueryPoint(in MapCoordinate location, int z, List<SparseTagQueryResult<TData, TEntity>>? result = null)
        {
            var backendResults = pool.Get();
            try
            {
                backendResults.Clear();
                backend.QueryPoint(location, z, backendResults);
                result ??= new List<SparseTagQueryResult<TData, TEntity>>(backendResults.Count);
                result.Clear();
                for (var i = 0; i < backendResults.Count; i++)
                {
                    var data = backendResults[i];
                    result.Add(new SparseTagQueryResult<TData, TEntity>(data.TagData, data.Entity.Item1, data.Position));
                }

                return result;
            }
            finally
            {
                backendResults.Clear();
                pool.Return(backendResults);
            }
        }

        class DowngradeMetaData: ITileDataSetMetaData
        {
            readonly ITileDataSetMetaData backend;

            public DowngradeMetaData(ITileDataSetMetaData backend)
            {
                this.backend = backend;
            }

            public DataSetType DataSetType => backend.DataSetType switch
            {
                DataSetType.ClassSet => DataSetType.ClassSet,
                DataSetType.TagMap => DataSetType.TagMap,
                DataSetType.QuantifiedClassSet => DataSetType.ClassSet,
                DataSetType.QuantifiedTagMap => DataSetType.TagMap,
                _ => throw new ArgumentOutOfRangeException()
            };

            public bool IsThreadSafe => backend.IsThreadSafe;
        }
    }
}