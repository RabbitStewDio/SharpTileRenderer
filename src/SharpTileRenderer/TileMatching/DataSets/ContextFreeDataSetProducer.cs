using System;

namespace SharpTileRenderer.TileMatching.DataSets
{
    public class ContextFreeDataSetProducer<TEntity> : ITileDataSetProducer<Unit>
    {
        readonly ITileDataSetProducer<TEntity> parent;

        public ContextFreeDataSetProducer(ITileDataSetProducer<TEntity> parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public bool ContainsDataSet(string id)
        {
            return parent.ContainsDataSet(id);
        }

        public ITileDataSet<GraphicTag, Unit> CreateGraphicDataSet(string id)
        {
            var ds = this.parent.CreateGraphicDataSet(id);
            if (ds is ITileDataSet<GraphicTag, Unit> validDs) return validDs;
            return new ContextFreeDataSet<GraphicTag, TEntity>(ds);
        }

        public IQuantifiedTagTileDataSet<GraphicTag, Unit, int> CreateCountedGraphicDataSet(string id)
        {
            var ds = this.parent.CreateCountedGraphicDataSet(id);
            if (ds is IQuantifiedTagTileDataSet<GraphicTag, Unit, int> validDs) return validDs;
            return new ContextFreeQuantifiedDataSet<GraphicTag, TEntity, int>(ds);
        }
/*
        public ITileDataSet<TClassDataSet, Unit> CreateClassDataSet(string id)
        {
            var ds = this.parent.CreateClassDataSet(id);
            if (ds is ITileDataSet<TClassDataSet, Unit> validDs) return validDs;
            return new ContextFreeDataSet<TClassDataSet, TEntity>(ds);
        }

        public IQuantifiedTagTileDataSet<TClassDataSet, Unit, int> CreateCountedClassDataSet(string id)
        {
            var ds = this.parent.CreateCountedClassDataSet(id);
            if (ds is IQuantifiedTagTileDataSet<TClassDataSet, Unit, int> validDs) return validDs;
            return new ContextFreeQuantifiedDataSet<TClassDataSet, TEntity, int>(ds);
        }
        */
    }
}