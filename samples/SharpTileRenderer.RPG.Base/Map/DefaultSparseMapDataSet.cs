using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.RPG.Base.Map
{
    public class DefaultSparseMapDataSet<TMapEntity, TEntity> : SparseMapDataSet<TMapEntity, TEntity>
    {
        readonly Func<TMapEntity, ContinuousMapCoordinate> positionExtractor;
        readonly Func<TMapEntity, GraphicTag> graphicExtractor;
        readonly Func<TMapEntity, TEntity> entityExtractor;

        public DefaultSparseMapDataSet(IReadOnlyList<TMapEntity> dataSet,
                                       Func<TMapEntity, ContinuousMapCoordinate> positionExtractor,
                                       Func<TMapEntity, GraphicTag> graphicExtractor,
                                       Func<TMapEntity, TEntity> entityExtractor) : base(dataSet)
        {
            this.positionExtractor = positionExtractor;
            this.graphicExtractor = graphicExtractor;
            this.entityExtractor = entityExtractor;
        }

        public override ContinuousMapCoordinate ExtractPosition(TMapEntity e)
        {
            return positionExtractor(e);
        }

        public override GraphicTag ExtractGraphicTag(TMapEntity e)
        {
            return graphicExtractor(e);
        }

        public override TEntity ExtractEntityInfo(TMapEntity e)
        {
            return entityExtractor(e);
        }
    }
}