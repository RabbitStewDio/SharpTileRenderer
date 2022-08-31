using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public class BasicSpriteMatcher : ISpriteMatcher<GraphicTag>
    {
        public string MatcherType => "basic";
        public string? Prefix { get; }
        public string? Suffix { get; }
        public bool IsThreadSafe => true;

        public BasicSpriteMatcher(string? prefix, string? suffix)
        {
            Prefix = SpriteTag.Normalize(prefix);
            Suffix = SpriteTag.Normalize(suffix);
        }

        public bool Match(in SpriteMatcherInput<GraphicTag> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            if (q.TagData != GraphicTag.Empty)
            {
                var spriteTag = q.TagData.AsSpriteTag();
                resultCollector.Add((spriteTag.WithPrefix(Prefix).WithQualifier(Suffix), SpritePosition.Whole, q.Position));
                return true;
            }

            return false;
        }

        public static ISpriteMatcher<GraphicTag> Create<TClassification>(ISelectorModel model,
                                                                         IMatcherFactory<TClassification> factory,
                                                                         IMatchFactoryContext<TClassification> context)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (model is not BasicSelectorModel m)
            {
                throw new ArgumentException();
            }

            return new BasicSpriteMatcher(m.Prefix, m.Suffix);
        }
    }
}