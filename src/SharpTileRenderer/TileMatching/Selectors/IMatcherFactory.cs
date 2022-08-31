using SharpTileRenderer.TileMatching.Model.Selectors;

namespace SharpTileRenderer.TileMatching.Selectors
{
    public interface IMatcherFactory<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        ISpriteMatcher<GraphicTag> CreateTagMatcher(ISelectorModel model, IMatchFactoryContext<TClassification> context);

        ISpriteMatcher<(GraphicTag, int)> CreateQuantifiedTagMatcher(ISelectorModel model, IMatchFactoryContext<TClassification> context);
/*
        ISpriteMatcher<TClassification> CreateClassMatcher(ISelectorModel model, IMatchFactoryContext<TClassification> context);

        ISpriteMatcher<(TClassification, int)> CreateQuantifiedClassMatcher(ISelectorModel model, IMatchFactoryContext<TClassification> context);
        */
    }
}