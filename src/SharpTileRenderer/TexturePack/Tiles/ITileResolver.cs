using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.TexturePack.Tiles
{
    public interface ITileResolver<in TLookUp>
    {
        bool Exists(TLookUp tag);
    }
    
    public interface ITileResolver<in TLookUp, TTexture>: ITileResolver<TLookUp>
    {
        IntDimension TileSize { get; }
        bool TryFind(TLookUp tag, [MaybeNullWhen(false)] out TTexture tile);
    }
}