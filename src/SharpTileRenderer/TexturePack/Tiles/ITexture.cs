using SharpTileRenderer.TexturePack.Operations;

namespace SharpTileRenderer.TexturePack.Tiles
{
    /// <summary>
    ///  Represents a renderable data, usually a sub-element of a texture atlas.
    ///  This interface only contains management data. Subclass the interface
    ///  to provide your own additional properties.
    ///
    ///  This texture type uses native coordinates (whatever your engine's native
    ///  coordinate may be. 
    /// </summary>
    public interface ITexture<TTexture> 
        where TTexture: ITexture<TTexture>
    {
        string Name { get; }
        TextureCoordinateRect Bounds { get; }
        bool Valid { get; }
        
        TTexture CreateSubTexture(string name, TextureCoordinateRect bounds);
    }
}
