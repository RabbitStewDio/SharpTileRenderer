using SharpTileRenderer.TileMatching;
using System;

namespace SharpTileRenderer.RPG.Base.Map
{
    public class DefaultMapTileDataSet<TMapData, TEntity> : MapTileDataSet<TMapData, TEntity>
    {
        readonly Func<TMapData, GraphicTag> tagMapping;
        readonly Func<TMapData, TEntity> entityMapping;

        public DefaultMapTileDataSet(IMap2D<TMapData> rawData,
                                     Func<TMapData, GraphicTag> tagMapping,
                                     Func<TMapData, TEntity> entityMapping) : base(rawData)
        {
            this.tagMapping = tagMapping ?? throw new ArgumentNullException(nameof(tagMapping));
            this.entityMapping = entityMapping ?? throw new ArgumentNullException(nameof(entityMapping));
        }

        protected override TEntity ConvertDataToEntity(TMapData d)
        {
            return entityMapping(d);
        }

        protected override GraphicTag ConvertDataToGraphicTag(TMapData d)
        {
            return tagMapping(d);
        }
    }
}