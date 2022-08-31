using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    /// <summary>
    ///    Processes all child sprite matchers in the given order.
    /// </summary>
    /// <typeparam name="TInputData"></typeparam>
    public class ListSpriteMatcher<TInputData> : ISpriteMatcher<TInputData>
    {
        public string MatcherType => "list";
        public IReadOnlyList<ISpriteMatcher<TInputData>> Matchers { get; }
        public bool IsThreadSafe { get; }

        public ListSpriteMatcher(IReadOnlyList<ISpriteMatcher<TInputData>> matchers)
        {
            Matchers = matchers ?? throw new ArgumentNullException(nameof(matchers));
            IsThreadSafe = true;
            foreach (var m in matchers)
            {
                IsThreadSafe &= m.IsThreadSafe;
            }
        }

        public bool Match(in SpriteMatcherInput<TInputData> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            bool result = false;
            for (var index = 0; index < Matchers.Count; index++)
            {
                var m = Matchers[index];
                result |= m.Match(q, z, resultCollector);
            }

            return result;
        }
    }

    public static class ListSpriteMatcher
    {
        public static ISpriteMatcher<GraphicTag> Create<TClassification>(ISelectorModel model,
                                                                         IMatcherFactory<TClassification> factory,
                                                                         IMatchFactoryContext<TClassification> context)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (model is not ListSelectorModel m)
            {
                throw new ArgumentException();
            }

            var selectors = new List<ISpriteMatcher<GraphicTag>>();
            foreach (var selector in m.Selectors)
            {
                selectors.Add(factory.CreateTagMatcher(selector, context));
            }

            return new ListSpriteMatcher<GraphicTag>(selectors);
        }

        public static ISpriteMatcher<(GraphicTag, int)> CreateQuantified<TClassification>(ISelectorModel model,
                                                                                          IMatcherFactory<TClassification> factory,
                                                                                          IMatchFactoryContext<TClassification> context)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (model is not ListSelectorModel m)
            {
                throw new ArgumentException();
            }

            var selectors = new List<ISpriteMatcher<(GraphicTag, int)>>();
            foreach (var selector in m.Selectors)
            {
                selectors.Add(factory.CreateQuantifiedTagMatcher(selector, context));
            }

            return new ListSpriteMatcher<(GraphicTag, int)>(selectors);
        }
    }
}