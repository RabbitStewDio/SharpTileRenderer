using System.Collections.Generic;

namespace SharpTileRenderer.TexturePack
{
    public interface ITileProducer<TTile, TTexture, TRawTexture>
        where TTile : ITexturedTile<TTexture>
        where TTexture : ITexture
    {
        TTile Produce(TRawTexture texture,
                      IntDimension tileSize,
                      IntRect gridBounds,
                      IntPoint anchor,
                      string tag);
    }

    public interface IDerivedTileProducer<TTile, TTexture>
        where TTile : ITexturedTile<TTexture>
        where TTexture : ITexture
    {
        TTile Produce(TTexture texture,
                      IntDimension tileSize,
                      IntPoint anchor,
                      string tag);
    }

    public static class TileProducerExtensions
    {
        public static IEnumerable<TTile> ProduceAll<TTile, TTexture>(this IDerivedTileProducer<TTile, TTexture> p,
                                                                     TTexture texture,
                                                                     IntDimension tileSize,
                                                                     IntPoint anchor,
                                                                     params string[] tags)
            where TTile : ITexturedTile<TTexture>
            where TTexture : ITexture
        {
            foreach (var t in tags)
            {
                yield return p.Produce(texture, tileSize, anchor, t);
            }
        }

        public static IEnumerable<TTile> ProduceAll<TTile, TTexture, TRawTexture>(this ITileProducer<TTile, TTexture, TRawTexture> p,
                                                                                  TRawTexture texture,
                                                                                  IntDimension tileSize,
                                                                                  IntRect gridBounds,
                                                                                  IntPoint anchor,
                                                                                  params string[] tags)
            where TTile : ITexturedTile<TTexture>
            where TTexture : ITexture
        {
            foreach (var t in tags)
            {
                yield return p.Produce(texture, tileSize, gridBounds, anchor, t);
            }
        }
    }
}