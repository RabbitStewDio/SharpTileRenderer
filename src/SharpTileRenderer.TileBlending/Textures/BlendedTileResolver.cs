using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileBlending.Textures
{
    /// <summary>
    ///   Resolved a blending instruction to a possibly generated tile.
    ///
    ///   Blending instructions are given as SpriteTag consisting of a layer identifying prefix (default: "t.blend."),
    ///   a source tile specified in "id", and a qualifying suffix specifying the blend direction.
    /// </summary>
    /// <typeparam name="TTexture"></typeparam>
    public class BlendedTileResolver<TTexture> : ITileResolver<SpriteTag, TexturedTile<TTexture>>
        where TTexture : ITexture<TTexture>
    {
        readonly SpriteTag blendMaskSprite;
        readonly ITileResolver<SpriteTag, TexturedTile<TTexture>> parent;
        readonly Dictionary<SpriteTag, TexturedTile<TTexture>> generatedTiles;
        readonly Dictionary<string, string> suffixCache;
        readonly IBlendTileGenerator<TTexture> generator;

        public BlendedTileResolver(ITileResolver<SpriteTag, TexturedTile<TTexture>> parent,
                                   SpriteTag blendMaskSprite,
                                   IBlendTileGenerator<TTexture> generator)
        {
            if (blendMaskSprite.Equals(default)) throw new ArgumentException();

            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.blendMaskSprite = blendMaskSprite;
            this.generator = generator;
            this.generatedTiles = new Dictionary<SpriteTag, TexturedTile<TTexture>>();
            this.suffixCache = new Dictionary<string, string>();
        }

        public IntDimension TileSize => parent.TileSize;

        public bool Exists(SpriteTag tag)
        {
            if (generatedTiles.TryGetValue(tag, out _))
            {
                return true;
            }

            if (!parent.TryFind(tag.WithQualifier(null).WithPrefix(null), out _))
            {
                return false;
            }

            return true;
        }

        public bool TryFind(SpriteTag tag, out TexturedTile<TTexture> tile)
        {
            if (generatedTiles.TryGetValue(tag, out tile))
            {
                return true;
            }

            if (!parent.TryFind(tag.WithQualifier(null).WithPrefix(null), out var baseTile))
            {
                tile = default;
                return false;
            }

            // generate tiles, store in generator cache
            GenerateAllBlendShapes(tag, baseTile);
            return generatedTiles.TryGetValue(tag, out tile);
        }

        /// <summary>
        ///    A blend mask is an texture with alpha values. We first try to find a specialised blend mask for
        ///    the tile that gets blended (ie for a tile "desert" we will try to find a mask with qualifier "desert")
        ///    before looking for a shared blend mask.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        bool TryFindBlendMask(string? tag, out TexturedTile<TTexture> t)
        {
            if (tag != null)
            {
                if (!suffixCache.TryGetValue(tag, out var suffix))
                {
                    suffix = "_" + tag;
                    suffixCache[tag] = suffix;
                }

                if (parent.TryFind(blendMaskSprite.WithQualifier(suffix), out t))
                {
                    return true;
                }
            }

            if (parent.TryFind(blendMaskSprite, out t))
            {
                return true;
            }

            t = default;
            return false;
        }

        void GenerateAllBlendShapes(SpriteTag tag, TexturedTile<TTexture> baseTile)
        {
            var direction = tag.Qualifier;

            if (!TryParseDirection(direction, out var dir))
            {
                return;
            }
            
            if (!TryFindBlendMask(tag.Id, out var blendMask))
            {
                return;
            }

            if (generator.TryCreateBlendTile(baseTile, blendMask, dir, out var tile))
            {
                generatedTiles[tag] = tile;
            }
        }

        bool TryParseDirection(string? d, out TextureQuadrantIndex idx)
        {
            if (d == "_north")
            {
                idx = TextureQuadrantIndex.North;
                return true;
            }

            if (d == "_east")
            {
                idx = TextureQuadrantIndex.East;
                return true;
            }

            if (d == "_south")
            {
                idx = TextureQuadrantIndex.South;
                return true;
            }
            
            if (d == "_west")
            {
                idx = TextureQuadrantIndex.West;
                return true;
            }

            idx = default;
            return false;
        }
    }
}