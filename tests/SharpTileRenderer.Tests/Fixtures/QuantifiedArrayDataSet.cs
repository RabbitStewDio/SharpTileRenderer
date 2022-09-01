using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.Fixtures
{
    public class QuantifiedArrayDataSet<TData, TEntity, TQuantity> : IQuantifiedTagTileDataSet<TData, TEntity, TQuantity> 
        where TQuantity : IComparable<TQuantity>
    {
        readonly int width;
        readonly int height;
        readonly Dictionary<int, Optional<(TData, TEntity, TQuantity)>[]> entityData;

        public QuantifiedArrayDataSet(DataSetType t, int width, int height)
        {
            this.width = width;
            this.height = height;
            MetaData = new DefaultTileDataSetMetaData(t, true);
            entityData = new Dictionary<int, Optional<(TData, TEntity, TQuantity)>[]>();
        }

        public QuantifiedArrayDataSet<TData, TEntity, TQuantity> WithDataAt(int x, int y, int z, (TData data, TEntity entity, TQuantity q) e)
        {
            if (!entityData.TryGetValue(z, out var data))
            {
                data = new Optional<(TData, TEntity, TQuantity)>[width * height];
                entityData[z] = data;
            }

            data[x + y * width] = e;
            return this;
        }

        public ITileDataSetMetaData MetaData { get; }

        public List<SparseTagQueryResult<TData, (TEntity, TQuantity)>> QuerySparse(in ContinuousMapArea area, int z, List<SparseTagQueryResult<TData, (TEntity, TQuantity)>>? result = null)
        {
            return TileDataSetTools.ScanQueryArea(this, area, z, result);
        }

        public List<SparseTagQueryResult<TData, (TEntity, TQuantity)>> QueryPoint(in MapCoordinate location, int z, List<SparseTagQueryResult<TData, (TEntity, TQuantity)>>? result = null)
        {
            result ??= new List<SparseTagQueryResult<TData, (TEntity, TQuantity)>>();
            if (entityData.TryGetValue(z, out var data) &&
                data[location.X + location.Y * width].TryGetValue(out var d))
            {
                result.Add(new SparseTagQueryResult<TData, (TEntity, TQuantity)>(d.Item1, (d.Item2, d.Item3), location));
            }

            return result;
        }
    }
}