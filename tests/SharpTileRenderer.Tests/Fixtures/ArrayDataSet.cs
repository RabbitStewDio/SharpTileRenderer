using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.Util;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.Fixtures
{
    public class TestDataSetProducer<TData> : ITileDataSetProducer<TData>
    {
        public bool ContainsDataSet(string id)
        {
            return false;
        }

        public ITileDataSet<GraphicTag, TData> CreateGraphicDataSet(string id)
        {
            throw new System.NotImplementedException();
        }

        public IQuantifiedTagTileDataSet<GraphicTag, TData, int> CreateCountedGraphicDataSet(string id)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ArrayDataSet<TData, TEntity> : ITileDataSet<TData, TEntity>
    {
        readonly int width;
        readonly int height;
        readonly Dictionary<int, Optional<(TData, TEntity)>[,]> entityData;

        public ArrayDataSet(DataSetType t, int width, int height)
        {
            this.width = width;
            this.height = height;
            MetaData = new DefaultTileDataSetMetaData(t, true);
            entityData = new Dictionary<int, Optional<(TData, TEntity)>[,]>();
        }

        public ArrayDataSet<TData, TEntity> WithDataAt(int x, int y, int z, (TData data, TEntity entity) e)
        {
            if (!entityData.TryGetValue(z, out var data))
            {
                data = new Optional<(TData, TEntity)>[width, height];
                entityData[z] = data;
            }

            data[x, y] = e;
            return this;
        }

        public ITileDataSetMetaData MetaData { get; }

        public List<SparseTagQueryResult<TData, TEntity>> QuerySparse(in ContinuousMapArea area, int z, List<SparseTagQueryResult<TData, TEntity>>? result = null)
        {
            return TileDataSetTools.ScanQueryArea(this, area, z, result);
        }

        public List<SparseTagQueryResult<TData, TEntity>> QueryPoint(in MapCoordinate location, int z, List<SparseTagQueryResult<TData, TEntity>>? result = null)
        {
            result ??= new List<SparseTagQueryResult<TData, TEntity>>();
            if (entityData.TryGetValue(z, out var data))
            {
                if (location.X >= 0 && location.X < width &&
                    location.Y >= 0 && location.Y < height &&
                    data[location.X, location.Y].TryGetValue(out var d))
                {
                    result.Add(new SparseTagQueryResult<TData, TEntity>(d.Item1, d.Item2, location));
                }
            }

            return result;
        }
    }

    public static class ArrayDataSet
    {
        
        public static ArrayDataSet<TData, Unit> WithDataAt<TData>(this ArrayDataSet<TData, Unit> ds, int x, int y, int z, TData e)
        {
            return ds.WithDataAt(x, y, z, (e, default));
        }


        public static ArrayDataSet<GraphicTag, TEntity> CreateBasicTagDataSet<TEntity>(int width, int height)
        {
            return new ArrayDataSet<GraphicTag, TEntity>(DataSetType.TagMap, width, height);
        }

        public static ArrayDataSet<TClassification, TEntity> CreateBasicClassDataSet<TClassification, TEntity>(int width, int height)
            where TClassification : IEntityClassification<TClassification>
        {
            return new ArrayDataSet<TClassification, TEntity>(DataSetType.ClassSet, width, height);
        }

        public static QuantifiedArrayDataSet<GraphicTag, TEntity, int> CreateCountedTagDataSet<TEntity>(int width, int height)
        {
            return new QuantifiedArrayDataSet<GraphicTag, TEntity, int>(DataSetType.QuantifiedTagMap, width, height);
        }

        public static QuantifiedArrayDataSet<TClassification, TEntity, int> CreateCountedClassDataSet<TClassification, TEntity>(int width, int height)
            where TClassification : IEntityClassification<TClassification>
        {
            return new QuantifiedArrayDataSet<TClassification, TEntity, int>(DataSetType.QuantifiedClassSet, width, height);
        }
    }
}