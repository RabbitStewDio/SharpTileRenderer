using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.TexturePack
{
    public interface ITextureTileModule : IFeatureModule
    {
        T WithTextureOperation<T>(ITileSetOperationFunc<T> t);
    }

    public interface ITexturedTileModule<TTexture> : ITextureTileModule
        where TTexture : ITexture<TTexture>
    {
        Optional<ITileResolver<SpriteTag, TexturedTile<TTexture>>> TileSet { get; }
        T WithTextureOperation<T>(ITextureOperationFunc<T, TTexture> t);
    }

    public interface ITileSetOperationFunc<T>
    {
        public T Apply<TTexture>(ITexturedTileModule<TTexture> mod) 
            where TTexture : ITexture<TTexture>;
    }

    public interface ITextureOperationFunc<T, TTexture>
        where TTexture : ITexture<TTexture>
    {
        public T Apply<TColor>(ITextureOperations<TTexture, TColor> op);
    }
}