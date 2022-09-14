using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.DataSets;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.RPG.Base.Map
{
    public abstract class QuantifiedMapTileDataSet<TMapData, TEntity> : IQuantifiedTagTileDataSet<GraphicTag, TEntity, int>
    {
        readonly IMap2D<TMapData> rawData;

        protected abstract GraphicTag ConvertDataToGraphicTag(TMapData d);
        protected abstract (TEntity, int) ConvertDataToEntity(TMapData d);

        protected QuantifiedMapTileDataSet(IMap2D<TMapData> rawData)
        {
            this.rawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
            this.MetaData = new DefaultTileDataSetMetaData(DataSetType.TagMap, true);
        }

        public ITileDataSetMetaData MetaData { get; }

        public List<SparseTagQueryResult<GraphicTag, (TEntity, int)>> QuerySparse(in ContinuousMapArea area, int z, List<SparseTagQueryResult<GraphicTag, (TEntity, int)>>? result = null)
        {
            result ??= new List<SparseTagQueryResult<GraphicTag, (TEntity, int)>>();
            result.Clear();

            var minX = (int)Math.Floor(area.MinExtent.X);
            var minY = (int)Math.Floor(area.MinExtent.Y);
            var maxX = (int)Math.Ceiling(area.MaxExtent.X);
            var maxY = (int)Math.Ceiling(area.MaxExtent.Y);

            for (var y = minY; y <= maxY; y += 1)
            {
                for (var x = minX; x <= maxX; x += 1)
                {
                    var data = rawData[x, y];
                    var graphicTag = ConvertDataToGraphicTag(data);
                    if (graphicTag == GraphicTag.Empty)
                    {
                        return result;
                    }
                    var entity = ConvertDataToEntity(data);
                    result.Add(new SparseTagQueryResult<GraphicTag, (TEntity, int)>(graphicTag, entity, new MapCoordinate(x, y)));
                }
            }

            return result;
        }

        public List<SparseTagQueryResult<GraphicTag, (TEntity, int)>> QueryPoint(in MapCoordinate location, int z, List<SparseTagQueryResult<GraphicTag, (TEntity, int)>>? result = null)
        {
            result ??= new List<SparseTagQueryResult<GraphicTag, (TEntity, int)>>();
            result.Clear();

            var data = rawData[location.X, location.Y];
            var graphicTag = ConvertDataToGraphicTag(data);
            if (graphicTag == GraphicTag.Empty)
            {
                return result;
            }
            var entity = ConvertDataToEntity(data);
            result.Add(new SparseTagQueryResult<GraphicTag, (TEntity, int)>(graphicTag, entity, new MapCoordinate(location.X, location.Y)));
            return result;
        }
    }
}