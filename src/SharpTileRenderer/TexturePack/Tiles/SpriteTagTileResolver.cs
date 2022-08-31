using JetBrains.Annotations;
using SharpTileRenderer.TileMatching;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.TexturePack.Tiles
{
    public static class SpriteTagTileResolverExtensions
    {
        public static SpriteTagTileResolver<TTexturedTile> Populate<TTexturedTile, TTexture>(this SpriteTagTileResolver<TTexturedTile> r,
                                                                                             ITileProducer<TTexturedTile, TTexture> tp,
                                                                                             IContentLoader<TTexture> cl,
                                                                                             ITileCollection tilePack)
            where TTexturedTile : ITexturedTile<TTexture>
            where TTexture : ITexture<TTexture>
        {
            foreach (var t in tilePack.ProduceTiles())
            {
                if (t.TextureAssetName == null) continue;

                var tx = cl.LoadTexture(ContentUri.MakeRelative(t.TextureAssetName));
                foreach (var tag in t.Tags)
                {
                    var tile = tp.Produce(tx, tilePack.TileSize, t.Bounds, t.Anchor, tag);
                    r.Add(tag, tile);
                }
            }

            return r;
        }
    }

    [UsedImplicitly]
    public class SpriteTagTileResolver<TTexturedTile> : ITileResolver<SpriteTag, TTexturedTile>
    {
        public IntDimension TileSize { get; }
        readonly Dictionary<SpriteTag, (bool result, TTexturedTile? texture)> directCache;
        readonly Dictionary<SpriteTag, TTexturedTile> rawData;

        public SpriteTagTileResolver(IntDimension tileSize)
        {
            TileSize = tileSize;
            this.directCache = new Dictionary<SpriteTag, (bool result, TTexturedTile? texture)>();
            this.rawData = new Dictionary<SpriteTag, TTexturedTile>();
        }

        public void Add(SpriteTag tag, TTexturedTile texture)
        {
            rawData[tag] = texture;
        }

        public bool Exists(SpriteTag tag)
        {
            return TryFind(tag, out _);
        }

        public int Count => rawData.Count;
        
        public bool TryFind(SpriteTag tag, [MaybeNullWhen(false)] out TTexturedTile tile)
        {
            if (directCache.TryGetValue(tag, out var result))
            {
                tile = result.texture;
                return result.result;
            }

            var name = tag.ToString();
            if (SpriteTag.Parse(name).TryGetValue(out var normalizedTag) && 
                rawData.TryGetValue(normalizedTag, out tile))
            {
                directCache[tag] = (true, tile);
                return true;
            }

            directCache[tag] = (false, default);
            tile = default;
            return false;
        }
    }
}