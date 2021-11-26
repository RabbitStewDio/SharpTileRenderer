using SharpTileRenderer.TileMatching.Sprites;
using System;

namespace SharpTileRenderer.TileMatching
{
    public abstract class TransformingTileMatcherBase<TRenderTile, TContextSource, TContextTarget> : ITileMatcher<TRenderTile, TContextTarget>
    {
        readonly ITileMatcher<TRenderTile, TContextSource> parent;

        protected TransformingTileMatcherBase(ITileMatcher<TRenderTile, TContextSource> parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        protected abstract TContextTarget Convert(int x, int y, TRenderTile tile, TContextSource source);

        public bool Match(int x, int y, TileResultCollector<TRenderTile, TContextTarget> onMatchFound)
        {
            void Collect(SpritePosition pos, TRenderTile result, TContextSource context)
            {
                onMatchFound(pos, result, Convert(x, y, result, context));
            }

            return parent.Match(x, y, Collect);
        }
    }
}
