using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using System.Collections.Generic;

namespace SharpTileRenderer.TexturePack.Atlas
{
    public class MultiTextureAtlasBuilder<TTexture, TColor> : ITextureAtlasBuilder<TTexture>
        where TTexture : ITexture<TTexture>
    {
        readonly int size;
        readonly List<TextureAtlasBuilder<TTexture, TColor>> builders;
        readonly ITextureOperations<TTexture, TColor> textureOperations;

        public MultiTextureAtlasBuilder(ITextureOperations<TTexture, TColor> textureOperations,
                                        int size = TextureAtlasBuilder.DefaultMaxTextureSize)
        {
            this.textureOperations = textureOperations;
            this.size = size;
            builders = new List<TextureAtlasBuilder<TTexture, TColor>>();
        }

        public TTexture Add(TTexture tile)
        {
            foreach (var b in builders)
            {
                if (b.Insert(tile, out var result))
                {
                    return result;
                }
            }

            var rt = textureOperations.CreateTexture("TextureAtlas-" + builders.Count,
                                                     new IntDimension(size, size));
            var b2 = new TextureAtlasBuilder<TTexture, TColor>(textureOperations, rt);

            if (b2.Insert(tile, out var result2))
            {
                builders.Add(b2);
                return result2;
            }

            return tile;
        }

        public IEnumerable<TTexture> GetTextures()
        {
            foreach (var b in builders)
            {
                yield return b.Texture;
            }
        }
    }
}
