using Microsoft.Extensions.ObjectPool;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.DataSets
{
    public class ContextFreeQuantifiedDataSet<TData, TEntity, TQuantity> : IQuantifiedTagTileDataSet<TData, Unit, TQuantity> 
        where TQuantity : IComparable<TQuantity>
    {
        readonly IQuantifiedTagTileDataSet<TData, TEntity, TQuantity> parent;
        readonly ObjectPool<List<SparseTagQueryResult<TData, (TEntity, TQuantity)>>> cachedResultSource;

        public ContextFreeQuantifiedDataSet(IQuantifiedTagTileDataSet<TData, TEntity, TQuantity> parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.cachedResultSource = new DefaultObjectPool<List<SparseTagQueryResult<TData, (TEntity, TQuantity)>>>
                (new ListObjectPolicy<SparseTagQueryResult<TData, (TEntity, TQuantity)>>());
        }

        public ITileDataSetMetaData MetaData => parent.MetaData;

        public List<SparseTagQueryResult<TData, (Unit, TQuantity)>> QuerySparse(in ContinuousMapArea area, int z, 
                                                                                List<SparseTagQueryResult<TData, (Unit, TQuantity)>>? result = null)
        {
            var data = cachedResultSource.Get();
            try
            {
                parent.QuerySparse(area, z, data);
                result ??= new List<SparseTagQueryResult<TData, (Unit, TQuantity)>>();
                result.Clear();
                for (var index = 0; index < data.Count; index++)
                {
                    var d = data[index];
                    var se = new SparseTagQueryResult<TData, (Unit, TQuantity)>(d.TagData, (default, d.Entity.Item2), d.Position);
                    result.Add(se);
                }

                return result;
            }
            finally
            {
                data.Clear();
                cachedResultSource.Return(data);
            }
        }

        public List<SparseTagQueryResult<TData, (Unit, TQuantity)>> QueryPoint(in MapCoordinate location, int z, List<SparseTagQueryResult<TData, (Unit, TQuantity)>>? result = null)
        {
            var data = cachedResultSource.Get();
            try
            {
                parent.QueryPoint(location, z, data);
                result ??= new List<SparseTagQueryResult<TData, (Unit, TQuantity)>>();
                result.Clear();
                for (var index = 0; index < data.Count; index++)
                {
                    var d = data[index];
                    var se = new SparseTagQueryResult<TData, (Unit, TQuantity)>(d.TagData, (default, d.Entity.Item2), d.Position);
                    result.Add(se);
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