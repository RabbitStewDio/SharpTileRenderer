using SharpTileRenderer.Navigation;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors
{
    class DowngradedSpriteMatcher<TData, TQuantity>: ISpriteMatcher<TData>
    {
        readonly ISpriteMatcher<(TData, TQuantity)> baseline;
        readonly TQuantity defaultQuantity;

        public DowngradedSpriteMatcher(ISpriteMatcher<(TData, TQuantity)> baseline, TQuantity defaultQuantity)
        {
            this.baseline = baseline;
            this.defaultQuantity = defaultQuantity;
        }

        public string MatcherType => baseline.MatcherType + ":downgrade";
        public bool IsThreadSafe => baseline.IsThreadSafe;
        
        public bool Match(in SpriteMatcherInput<TData> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            return baseline.Match(new SpriteMatcherInput<(TData, TQuantity)>((q.TagData, defaultQuantity), q.Position), z, resultCollector);
        }
    }
}