using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.DataSets;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.RPG.Base.Map
{
    public abstract class MapTileDataSet<TMapData, TEntity> : ITileDataSet<GraphicTag, TEntity>
    {
        readonly IMap2D<TMapData> rawData;

        protected abstract GraphicTag ConvertDataToGraphicTag(TMapData d);
        protected abstract TEntity ConvertDataToEntity(TMapData d);

        protected MapTileDataSet(IMap2D<TMapData> rawData)
        {
            this.rawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
            this.MetaData = new DefaultTileDataSetMetaData(DataSetType.TagMap, true);
        }

        public ITileDataSetMetaData MetaData { get; }

        public List<SparseTagQueryResult<GraphicTag, TEntity>> QuerySparse(in ContinuousMapArea area, int z, List<SparseTagQueryResult<GraphicTag, TEntity>>? result = null)
        {
            result ??= new List<SparseTagQueryResult<GraphicTag, TEntity>>();
            result.Clear();

            var minX = (int)Math.Floor(Math.Max(0, area.MinExtent.X));
            var minY = (int)Math.Floor(Math.Max(0, area.MinExtent.Y));
            var maxX = (int)Math.Ceiling(Math.Min(rawData.Width - 1, area.MaxExtent.X));
            var maxY = (int)Math.Ceiling(Math.Min(rawData.Height - 1, area.MaxExtent.Y));

            for (var y = minY; y <= maxY; y += 1)
            {
                for (var x = minX; x <= maxX; x += 1)
                {
                    var data = rawData[x, y];
                    var graphicTag = ConvertDataToGraphicTag(data);
                    if (graphicTag == GraphicTag.Empty)
                    {
                        continue;
                    }

                    var entity = ConvertDataToEntity(data);
                    result.Add(new SparseTagQueryResult<GraphicTag, TEntity>(graphicTag, entity, new MapCoordinate(x, y)));
                }
            }

            return result;
        }

        public List<SparseTagQueryResult<GraphicTag, TEntity>> QueryPoint(in MapCoordinate location, int z, List<SparseTagQueryResult<GraphicTag, TEntity>>? result = null)
        {
            result ??= new List<SparseTagQueryResult<GraphicTag, TEntity>>();
            result.Clear();

            if (location.X < 0 || location.X >= rawData.Width ||
                location.Y < 0 || location.Y >= rawData.Height)
            {
                return result;
            }

            var data = rawData[location.X, location.Y];
            var graphicTag = ConvertDataToGraphicTag(data);
            if (graphicTag == GraphicTag.Empty)
            {
                return result;
            }

            var entity = ConvertDataToEntity(data);
            result.Add(new SparseTagQueryResult<GraphicTag, TEntity>(graphicTag, entity, location));
            return result;
        }
    }
}