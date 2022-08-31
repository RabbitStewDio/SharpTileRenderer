using SharpTileRenderer.TileMatching;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TexturePack.Tiles
{
    public static class TileProducerExtensions
    {
        
        
        public static IEnumerable<TTile> ProduceAll<TTile, TTexture>(this IDerivedTileProducer<TTile, TTexture> p,
                                                                     TTexture texture,
                                                                     IntDimension tileSize,
                                                                     IntPoint anchor,
                                                                     params SpriteTag[] tags)
            where TTile : ITexturedTile<TTexture>
            where TTexture : ITexture<TTexture>
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            foreach (var t in tags)
            {
                yield return p.Produce(texture, tileSize, anchor, t);
            }
        }

        public static IEnumerable<TTile> ProduceAll<TTile, TTexture>(this ITileProducer<TTile, TTexture> p,
                                                                     TTexture texture,
                                                                     IntDimension tileSize,
                                                                     IntRect gridBounds,
                                                                     IntPoint anchor,
                                                                     params SpriteTag[] tags)
            where TTile : ITexturedTile<TTexture>
            where TTexture : ITexture<TTexture>
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            foreach (var t in tags)
            {
                yield return p.Produce(texture, tileSize, gridBounds, anchor, t);
            }
        }
    }
}