using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.DataSets;
using System.Collections.Generic;

namespace SharpTileRenderer.RPG.Base.Map
{
    public abstract class SparseMapDataSet<TMapEntity, TEntity> : ITileDataSet<GraphicTag, TEntity>
    {
        readonly IReadOnlyList<TMapEntity> dataSet;

        public SparseMapDataSet(IReadOnlyList<TMapEntity> dataSet)
        {
            this.dataSet = dataSet;
            this.MetaData = new DefaultTileDataSetMetaData(DataSetType.TagMap, false);
        }

        public ITileDataSetMetaData MetaData { get; }
        public List<SparseTagQueryResult<GraphicTag, TEntity>> QuerySparse(in ContinuousMapArea area, int z, List<SparseTagQueryResult<GraphicTag, TEntity>>? result = null)
        {
            result ??= new List<SparseTagQueryResult<GraphicTag, TEntity>>();
            result.Clear();
            
            for (var i = 0; i < dataSet.Count; i++)
            {
                var entity = dataSet[i];
                var pos = ExtractPosition(entity);
                if (!area.Contains(pos))
                {
                    continue;
                }

                var gt = ExtractGraphicTag(entity);
                if (gt == GraphicTag.Empty)
                {
                    continue;
                }

                result.Add(new SparseTagQueryResult<GraphicTag, TEntity>(gt, ExtractEntityInfo(entity), pos));
            }

            return result;
        }

        public List<SparseTagQueryResult<GraphicTag, TEntity>> QueryPoint(in MapCoordinate location, int z, List<SparseTagQueryResult<GraphicTag, TEntity>>? result = null)
        {
            result ??= new List<SparseTagQueryResult<GraphicTag, TEntity>>();
            result.Clear();
            
            for (var i = 0; i < dataSet.Count; i++)
            {
                var entity = dataSet[i];
                var pos = ExtractPosition(entity);
                if (location != pos.Normalize())
                {
                    continue;
                }

                var gt = ExtractGraphicTag(entity);
                if (gt == GraphicTag.Empty)
                {
                    continue;
                }

                result.Add(new SparseTagQueryResult<GraphicTag, TEntity>(gt, ExtractEntityInfo(entity), pos));
            }

            return result;
        }

        public abstract ContinuousMapCoordinate ExtractPosition(TMapEntity e);
        public abstract GraphicTag ExtractGraphicTag(TMapEntity e);
        public abstract TEntity ExtractEntityInfo(TMapEntity e);
    }
}