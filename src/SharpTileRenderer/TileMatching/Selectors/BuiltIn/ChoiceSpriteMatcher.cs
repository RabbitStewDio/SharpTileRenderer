using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    /// <summary>
    ///    Selects a child sprite matcher based on the given graphic tag.
    ///    Uses a fallback matcher if none of the choices matched.
    /// </summary>
    public class ChoiceSpriteMatcher : ISpriteMatcher<GraphicTag>
    {
        public string MatcherType => BuiltInSelectors.Choice;
        public bool IsThreadSafe { get; }
        readonly Dictionary<GraphicTag, ISpriteMatcher<GraphicTag>> cachedLookup;

        public ChoiceSpriteMatcher(IReadOnlyList<(GraphicTag, ISpriteMatcher<GraphicTag>)> matchers)
        {
            cachedLookup = new Dictionary<GraphicTag, ISpriteMatcher<GraphicTag>>();
            IsThreadSafe = true;
            foreach (var m in matchers)
            {
                cachedLookup[m.Item1] = m.Item2;
                IsThreadSafe &= m.Item2.IsThreadSafe;
            }
        }

        public bool Match(in SpriteMatcherInput<GraphicTag> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            if (cachedLookup.TryGetValue(q.TagData, out var value))
            {
                return value.Match(q, z, resultCollector);
            }

            return false;
        }

        public static ISpriteMatcher<GraphicTag> Create<TClassification>(ISelectorModel model,
                                                                         IMatcherFactory<TClassification> factory,
                                                                         IMatchFactoryContext<TClassification> context)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (model is not ChoiceSelectorModel m)
            {
                throw new ArgumentException();
            }

            var childMatchers = new List<(GraphicTag, ISpriteMatcher<GraphicTag>)>();
            foreach (var s in m.Choices)
            {
                var sm = factory.CreateTagMatcher(s.Selector ?? throw new ArgumentException(), context);
                foreach (var gt in s.MatchedTags)
                {
                    if (string.IsNullOrEmpty(gt))
                    {
                        throw new ArgumentException();
                    }

                    childMatchers.Add((new GraphicTag(gt), sm));
                }
            }

            return new ChoiceSpriteMatcher(childMatchers);
        }
    }

    /// <summary>
    ///    Selects a child sprite matcher based on the given graphic tag.
    ///    Uses a fallback matcher if none of the choices matched.
    /// </summary>
    public class QuantifiedChoiceSpriteMatcher : ISpriteMatcher<(GraphicTag, int)>
    {
        public string MatcherType => BuiltInSelectors.Choice;
        public bool IsThreadSafe { get; }
        readonly Dictionary<GraphicTag, ISpriteMatcher<(GraphicTag, int)>> cachedLookup;

        public QuantifiedChoiceSpriteMatcher(IReadOnlyList<(GraphicTag, ISpriteMatcher<(GraphicTag, int)>)> matchers)
        {
            cachedLookup = new Dictionary<GraphicTag, ISpriteMatcher<(GraphicTag, int)>>();
            IsThreadSafe = true;
            foreach (var m in matchers)
            {
                cachedLookup[m.Item1] = m.Item2;
                IsThreadSafe &= m.Item2.IsThreadSafe;
            }
        }

        public bool Match(in SpriteMatcherInput<(GraphicTag, int)> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            if (cachedLookup.TryGetValue(q.TagData.Item1, out var value))
            {
                return value.Match(q, z, resultCollector);
            }

            return false;
        }

        public static ISpriteMatcher<(GraphicTag, int)> Create<TClassification>(ISelectorModel model,
                                                                                IMatcherFactory<TClassification> factory,
                                                                                IMatchFactoryContext<TClassification> context)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (model is not ChoiceSelectorModel m)
            {
                throw new ArgumentException();
            }

            var childMatchers = new List<(GraphicTag, ISpriteMatcher<(GraphicTag, int)>)>();
            foreach (var s in m.Choices)
            {
                var sm = factory.CreateQuantifiedTagMatcher(s.Selector ?? throw new ArgumentException(), context);
                foreach (var gt in s.MatchedTags)
                {
                    if (string.IsNullOrEmpty(gt))
                    {
                        throw new ArgumentException();
                    }

                    childMatchers.Add((new GraphicTag(gt), sm));
                }
            }

            return new QuantifiedChoiceSpriteMatcher(childMatchers);
        }
    }
}