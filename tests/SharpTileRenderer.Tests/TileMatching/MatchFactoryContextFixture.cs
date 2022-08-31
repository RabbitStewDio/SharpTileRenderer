using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Selectors;

namespace SharpTileRenderer.Tests.TileMatching
{
    public class MatchFactoryContextFixture
    {
        public readonly EntityClassificationRegistry<EntityClassification16> EntityRegistration;
        public readonly GraphicTagMetaDataRegistry<EntityClassification16> TagMeta;
        public readonly DefaultTileDataSetProducer<Unit> DataSetProducer;
        public readonly IMapNavigator<GridDirection> Navigator;
        public readonly DefaultMatchFactoryContext<EntityClassification16> FactoryContext;

        public MatchFactoryContextFixture()
        {
            EntityRegistration = new EntityClassificationRegistry<EntityClassification16>();
            TagMeta = new GraphicTagMetaDataRegistry<EntityClassification16>(EntityRegistration);
            DataSetProducer = new DefaultTileDataSetProducer<Unit>();
            Navigator = GridNavigation.CreateNavigator(GridType.Grid);
            FactoryContext = new DefaultMatchFactoryContext<EntityClassification16>(DataSetProducer, Navigator, EntityRegistration, TagMeta);            
        }

        public static MatchFactoryContextFixture CreateDefault()
        {
            var matchFactoryContextFixture = new MatchFactoryContextFixture();
            matchFactoryContextFixture.EntityRegistration.Register("A-class");
            matchFactoryContextFixture.EntityRegistration.Register("B-class");

            matchFactoryContextFixture.TagMeta.Register(new GraphicTag("tag-A")).WithClassification("A-class");
            matchFactoryContextFixture.TagMeta.Register(new GraphicTag("tag-B")).WithClassification("B-class");
            matchFactoryContextFixture.TagMeta.Register(new GraphicTag("tag-$"))
                                      .WithClassification("A-class")
                                      .WithClassification("B-class");
            return matchFactoryContextFixture;
        }
    }
}