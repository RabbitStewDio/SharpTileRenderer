using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using System;

namespace SharpTileRenderer.TileMatching.Selectors
{
    public interface IMatchFactoryContext<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        public ITileDataSetProducer<Unit> ContextDataSetProducer { get; }
        public IMapNavigator<GridDirection> GridNavigator { get; }
        public EntityClassificationRegistry<TClassification> ClassRegistry { get; }
        public IGraphicTagMetaDataRegistry<TClassification> TagMetaData { get; }
    }

    public class DefaultMatchFactoryContext<TClassification> : IMatchFactoryContext<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        public DefaultMatchFactoryContext(ITileDataSetProducer<Unit> dataSetProducer, 
                                          IMapNavigator<GridDirection> gridNavigator,
                                          EntityClassificationRegistry<TClassification> classRegistry,
                                          IGraphicTagMetaDataRegistry<TClassification> tagMetaData)
        {
            ClassRegistry = classRegistry ?? throw new ArgumentNullException(nameof(classRegistry));
            TagMetaData = tagMetaData;
            ContextDataSetProducer = dataSetProducer ?? throw new ArgumentNullException(nameof(dataSetProducer));
            GridNavigator = gridNavigator ?? throw new ArgumentNullException(nameof(gridNavigator));
        }

        public IMapNavigator<GridDirection> GridNavigator { get; }
        public EntityClassificationRegistry<TClassification> ClassRegistry { get; }
        public IGraphicTagMetaDataRegistry<TClassification> TagMetaData { get; }
        public ITileDataSetProducer<Unit> ContextDataSetProducer { get; }
    }

    public static class DefaultMatchFactoryContext
    {
        public static DefaultMatchFactoryContext<TClassification> From<TEntity, TClassification>(ITileDataSetProducer<TEntity> dataSetProducer,
                                                                                                 IMapNavigator<GridDirection> gridType,
                                                                                                 EntityClassificationRegistry<TClassification> classRegistry,
                                                                                                 IGraphicTagMetaDataRegistry<TClassification> tagMetaData)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            return new DefaultMatchFactoryContext<TClassification>(new ContextFreeDataSetProducer<TEntity>(dataSetProducer), gridType, classRegistry, tagMetaData);
        }
    }
}