using Microsoft.Extensions.ObjectPool;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.DataSets
{
    public class ContextFreeDataSet<TData, TEntity> : ITileDataSet<TData, Unit>
    {
        readonly ITileDataSet<TData, TEntity> parent;
        readonly ObjectPool<List<SparseTagQueryResult<TData, TEntity>>> cachedResultSource;

        public ContextFreeDataSet(ITileDataSet<TData, TEntity> parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.cachedResultSource = new DefaultObjectPool<List<SparseTagQueryResult<TData, TEntity>>>
                (new ListObjectPolicy<SparseTagQueryResult<TData, TEntity>>());
        }

        public ITileDataSetMetaData MetaData => parent.MetaData;
        
        public List<SparseTagQueryResult<TData, Unit>> QuerySparse(in ContinuousMapArea area, int z, 
                                                                   List<SparseTagQueryResult<TData, Unit>>? result = null)
        {
            var data = cachedResultSource.Get();
            try
            {
                parent.QuerySparse(area, z, data);
                result ??= new List<SparseTagQueryResult<TData, Unit>>();
                result.Clear();
                for (var index = 0; index < data.Count; index++)
                {
                    result.Add(data[index].ForEntity(default(Unit)));
                }

                return result;
            }
            finally
            {
                data.Clear();
                cachedResultSource.Return(data);
            }
        }

        public List<SparseTagQueryResult<TData, Unit>> QueryPoint(in MapCoordinate location, int z, List<SparseTagQueryResult<TData, Unit>>? result = null)
        {
            var data = cachedResultSource.Get();
            try
            {
                parent.QueryPoint(location, z, data);
                result ??= new List<SparseTagQueryResult<TData, Unit>>();
                result.Clear();
                for (var index = 0; index < data.Count; index++)
                {
                    result.Add(data[index].ForEntity(default(Unit)));
                }

                return result;
            }
            finally
            {
                data.Clear();
                cachedResultSource.Return(data);
            }
        }
    }
}