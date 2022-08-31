namespace SharpTileRenderer.TileMatching.DataSets
{
/*
    public interface ITileDataSetProducer<TEntity, TClassDataSet>
        where TClassDataSet : struct, IEntityClassification<TClassDataSet>
    {
        ITileDataSet<GraphicTag, TEntity> CreateGraphicDataSet(string id);
        IQuantifiedTagTileDataSet<GraphicTag, TEntity, int> CreateCountedGraphicDataSet(string id);
        ITileDataSet<TClassDataSet, TEntity> CreateClassDataSet(string id);
        IQuantifiedTagTileDataSet<TClassDataSet, TEntity, int> CreateCountedClassDataSet(string id);
    }
        */

    public interface ITileDataSetProducer<TEntity>
    {
        bool ContainsDataSet(string id);
        ITileDataSet<GraphicTag, TEntity> CreateGraphicDataSet(string id);
        IQuantifiedTagTileDataSet<GraphicTag, TEntity, int> CreateCountedGraphicDataSet(string id);
    }

}
 