using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.TexturePack
{
    public interface ITextureTileModule : IFeatureModule
    {
        /// <summary>
        ///   This is a generic parameter enriching interface. This trick allows
        ///   the code to promote type information known in one part to be transferred
        ///   to dependent parts without having to ever explicitly name the parameters
        ///   when calling. 
        /// 
        ///   Calls the given operation with the current texture type parameter as
        ///   the <see cref="ITileSetOperationFunc{T}.Apply{TTexture}"/> method's
        ///   type parameter.  
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
       // T LiftTexture<T>(ITileSetOperationFunc<T> t);
    }

    public interface ITexturedTileModule<TTexture> : ITextureTileModule
        where TTexture : ITexture<TTexture>
    {
        Optional<ITileResolver<SpriteTag, TexturedTile<TTexture>>> TileSet { get; }
        // T LiftTextureColorOperation<T>(ITextureOperationFunc<T, TTexture> t);
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