using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;

namespace SharpTileRenderer.TileBlending.Textures
{
    public interface IBlendTileGenerator<TTexture>
    {
        public bool TryCreateBlendTile(TexturedTile<TTexture> baseTile, TexturedTile<TTexture> blendMask, TextureQuadrantIndex direction, out TexturedTile<TTexture> result);
    }
}