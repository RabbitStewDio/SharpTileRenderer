using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    /// <summary>
    ///    Selects a child sprite matcher based on the given graphic tag.
    ///    Uses a fallback matcher if none of the choices matched.
    /// </summary>
    public class QuantitySelectionSpriteMatcher : ISpriteMatcher<(GraphicTag tag, int quantity)>
    {
        public string MatcherType => BuiltInSelectors.Choice;
        public bool IsThreadSafe { get; }
        readonly List<int> sortedKeys;
        readonly List<ISpriteMatcher<(GraphicTag, int)>> sortedValues;

        public QuantitySelectionSpriteMatcher(IReadOnlyList<(int, ISpriteMatcher<(GraphicTag, int)>)> matchers)
        {
            if (matchers == null) throw new ArgumentNullException();
            if (matchers.Count == 0) throw new ArgumentException();

            IsThreadSafe = true;
            sortedKeys = new List<int>();
            sortedValues = new List<ISpriteMatcher<(GraphicTag, int)>>();
            foreach (var m in matchers.OrderBy(e => e.Item1))
            {
                sortedKeys.Add(m.Item1);
                sortedValues.Add(m.Item2);
            }
        }

        public bool Match(in SpriteMatcherInput<(GraphicTag tag, int quantity)> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            var result = FindIndex(q);
            return sortedValues[result].Match(q, z, resultCollector);
        }

        int FindIndex(SpriteMatcherInput<(GraphicTag tag, int quantity)> q)
        {
            int result;
            var pos = sortedKeys.BinarySearch(q.TagData.quantity);
            if (pos >= 0)
            {
                result = pos;
            }
            else
            {
                result = ~pos;
                if (result == sortedKeys.Count)
                {
                    result = sortedKeys.Count - 1;
                }
            }

            return result;
        }

        public static ISpriteMatcher<(GraphicTag, int)> Create<TClassification>(ISelectorModel model,
                                                                                IMatcherFactory<TClassification> factory,
                                                                                IMatchFactoryContext<TClassification> context)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (model is not QuantitySelectorModel m)
            {
                throw new ArgumentException();
            }

            var childMatchers = new List<(int, ISpriteMatcher<(GraphicTag, int)>)>();
            foreach (var s in m.Choices)
            {
                var sm = factory.CreateQuantifiedTagMatcher(s.Selector ?? throw new ArgumentException(), context);
                childMatchers.Add((s.MatchedQuantity, sm));
            }

            return new QuantitySelectionSpriteMatcher(childMatchers);
        }
    }
}