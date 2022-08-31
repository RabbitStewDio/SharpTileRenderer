using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.TileMatching.Selectors.BuiltIn;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors
{
    public class MatcherFactory<TClassification>: IMatcherFactory<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        public delegate ISpriteMatcher<TData> MatcherFactoryDelegate<TData>(ISelectorModel model,
                                                                            IMatcherFactory<TClassification> factory,
                                                                            IMatchFactoryContext<TClassification> context);
        
        readonly Dictionary<string, MatcherFactoryDelegate<GraphicTag>> tagMatchers;
        readonly Dictionary<string, MatcherFactoryDelegate<(GraphicTag, int)>> quantifiedTagMatchers;

        public MatcherFactory()
        {
            tagMatchers = new Dictionary<string, MatcherFactoryDelegate<GraphicTag>>();
            quantifiedTagMatchers = new Dictionary<string, MatcherFactoryDelegate<(GraphicTag, int)>>();
        }

        public MatcherFactory<TClassification> RegisterTagSelector(string id, MatcherFactoryDelegate<GraphicTag> f)
        {
            tagMatchers[id] = f;
            return this;
        }

        public MatcherFactory<TClassification> RegisterQuantifiedTagSelector(string id, MatcherFactoryDelegate<(GraphicTag, int)> f)
        {
            quantifiedTagMatchers[id] = f;
            return this;
        }

        public ISpriteMatcher<GraphicTag> CreateTagMatcher(ISelectorModel model, IMatchFactoryContext<TClassification> context)
        {
            if (tagMatchers.TryGetValue(model.Kind, out var factory))
            {
                return factory(model, this, context);
            }
            throw new ArgumentException();
        }


        public ISpriteMatcher<(GraphicTag, int)> CreateQuantifiedTagMatcher(ISelectorModel model, IMatchFactoryContext<TClassification> context)
        {
            if (quantifiedTagMatchers.TryGetValue(model.Kind, out var factory))
            {
                return factory(model, this, context);
            }
            
            if (tagMatchers.TryGetValue(model.Kind, out var baseFactory))
            {
                var x = baseFactory(model, this, context);
                return new UpgradedSpriteMatcher<GraphicTag, int>(x);
            }
            
            throw new ArgumentException();
        }
/*
        public ISpriteMatcher<TClassification> CreateClassMatcher(ISelectorModel model, IMatchFactoryContext<TClassification> context)
        {
            if (classMatchers.TryGetValue(model.Kind, out var factory))
            {
                return factory(model, this, context);
            }
            throw new ArgumentException();
        }

        public ISpriteMatcher<(TClassification, int)> CreateQuantifiedClassMatcher(ISelectorModel model, IMatchFactoryContext<TClassification> context)
        {
            if (quantifiedClassMatchers.TryGetValue(model.Kind, out var factory))
            {
                return factory(model, this, context);
            }

            if (classMatchers.TryGetValue(model.Kind, out var baseFactory))
            {
                var x = baseFactory(model, this, context);
                return new UpgradedSpriteMatcher<TClassification, int>(x);
            }
            
            throw new ArgumentException();
        }
*/
        public MatcherFactory<TClassification> WithDefaultMatchers()
        {
            RegisterTagSelector(BuiltInSelectors.Basic, BasicSpriteMatcher.Create);
            RegisterTagSelector(BuiltInSelectors.Cardinal, CardinalSpriteMatcher<TClassification>.Create);
            RegisterTagSelector(BuiltInSelectors.CellGroup, CellGroupSpriteMatcher<TClassification>.Create);
            RegisterTagSelector(BuiltInSelectors.List, ListSpriteMatcher.Create);
            RegisterTagSelector(BuiltInSelectors.Choice, ChoiceSpriteMatcher.Create);
            RegisterTagSelector(BuiltInSelectors.Corner, CornerSpriteMatcher<TClassification>.Create);
            RegisterTagSelector(BuiltInSelectors.Diagonal, DiagonalSpriteMatcher<TClassification>.Create);
            RegisterTagSelector(BuiltInSelectors.NeighbourOverlay, NeighbourOverlaySpriteMatcher<TClassification>.Create);
            RegisterTagSelector(BuiltInSelectors.RiverOutlet, RiverOutletSpriteMatcher<TClassification>.Create);
            RegisterTagSelector(BuiltInSelectors.RoadCorner, RoadCornerSpriteMatcher<TClassification>.Create);
            RegisterTagSelector(BuiltInSelectors.RoadParity, RoadParitySpriteMatcher<TClassification>.Create);

            RegisterQuantifiedTagSelector(BuiltInSelectors.List, ListSpriteMatcher.CreateQuantified);
            RegisterQuantifiedTagSelector(BuiltInSelectors.Choice, QuantifiedChoiceSpriteMatcher.Create);
            RegisterQuantifiedTagSelector(BuiltInSelectors.QuantityChoice, QuantitySelectionSpriteMatcher.Create);
            return this;
        }
    }
}