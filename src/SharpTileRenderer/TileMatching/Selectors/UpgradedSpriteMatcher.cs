using SharpTileRenderer.Navigation;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors
{
    class UpgradedSpriteMatcher<TData, TQuantity>: ISpriteMatcher<(TData, TQuantity)>
    {
        readonly ISpriteMatcher<TData> baseline;

        public UpgradedSpriteMatcher(ISpriteMatcher<TData> baseline)
        {
            this.baseline = baseline;
        }

        public string MatcherType => baseline.MatcherType + ":upgrade";
        public bool IsThreadSafe => baseline.IsThreadSafe;

        public bool Match(in SpriteMatcherInput<(TData, TQuantity)> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            return baseline.Match(new SpriteMatcherInput<TData>(q.TagData.Item1, q.Position), z, resultCollector);
        }
    }
}