using SharpTileRenderer.TileMatching;
using SharpTileRenderer.Util;

namespace SharpTileRenderer.TexturePack.Tiles
{
    /// <summary>
    /// A representation of a tile rendering operation. 
    /// <para>
    /// There is one instance for each tagged tile, possibly sharing underlying
    /// raw rendering data objects (ie textures).
    /// </para>
    /// </summary>
    public interface ITexturedTile<TTexture>
    {
        /// <summary>
        ///  Anchor is given in pixels.
        /// </summary>
        IntPoint Anchor { get; }
        Optional<TTexture> Texture { get; }
        SpriteTag Tag { get; }
    }

    public readonly struct TexturedTile<TTexture>: ITexturedTile<TTexture>
    {
        public IntPoint Anchor { get; }
        public Optional<TTexture> Texture { get; }
        public SpriteTag Tag { get; }

        public TexturedTile(SpriteTag tag, TTexture texture, IntPoint anchor)
        {
            Anchor = anchor;
            Texture = texture;
            Tag = tag;
        }

        public TexturedTile(SpriteTag tag) : this()
        {
            Tag = tag;
        }

        public override string ToString()
        {
            return $"{nameof(Tag)}: {Tag}";
        }
    }
}
