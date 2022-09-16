using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.TileBlending.Textures
{
    public class BlendTileGenerator<TTexture, TColor>: IBlendTileGenerator<TTexture>
        where TTexture : ITexture<TTexture>
    {
        readonly ITextureOperations<TTexture, TColor> textureOperations;
        readonly IntDimension tileSize;
        readonly Dictionary<(SpriteTag, TextureQuadrantIndex), BoundedTextureData<TColor>> textureDataCache;
        readonly ITextureAtlasBuilder<TTexture> textureAtlasBuilder;

        public BlendTileGenerator(ITextureOperations<TTexture, TColor> textureOperations, 
                                  IntDimension tileSize)
        {
            this.textureOperations = textureOperations;
            this.tileSize = tileSize;
            this.textureDataCache = new Dictionary<(SpriteTag, TextureQuadrantIndex), BoundedTextureData<TColor>>();
            this.textureAtlasBuilder = textureOperations.CreateAtlasBuilder();
        }

        bool TryExtractData(TexturedTile<TTexture> tile, TextureQuadrantIndex direction, [MaybeNullWhen(false)] out BoundedTextureData<TColor> t)
        {
            if (textureDataCache.TryGetValue((tile.Tag, direction), out t))
            {
                return true;
            }
            
            var sourceArea = textureOperations.TileAreaForCardinalDirection(tileSize, direction);
            if (!tile.Texture.TryGetValue(out var baseTexture))
            {
                t = default;
                return false;
            }

            t = textureOperations.ExtractData(baseTexture, sourceArea);
            textureDataCache[(tile.Tag, direction)] = t;
            return true;
        }

        public bool TryCreateBlendTile(TexturedTile<TTexture> baseTile, 
                                       TexturedTile<TTexture> blendMask, 
                                       TextureQuadrantIndex direction, 
                                       out TexturedTile<TTexture> result)
        {
            if (!TryExtractData(baseTile, direction, out var data))
            {
                result = default;
                return false;
            }

            if (!TryExtractData(blendMask, direction, out var effectiveMask))
            {
                result = default;
                return false;
            }
            
            var sourceArea = textureOperations.TileAreaForCardinalDirection(tileSize, direction);
            var resultData = textureOperations.CombineMask(data, effectiveMask);

            var textureName = baseTile.Tag.WithPrefix("blend-tile.").WithQualifier($"_{direction}");
            var wrappedTextureSize = new IntDimension(tileSize.Width, tileSize.Height);
            var wrappedTexture = textureOperations.CreateTexture(textureName.ToString(), wrappedTextureSize);
            var resultTexture = textureOperations.ApplyTextureData(wrappedTexture, resultData, sourceArea.Origin);

            var atlasEntry = textureAtlasBuilder.Add(resultTexture);
            result = new TexturedTile<TTexture>(textureName, atlasEntry, blendMask.Anchor);
            return true;
        }
    }
}