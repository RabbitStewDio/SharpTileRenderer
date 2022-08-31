using Microsoft.Xna.Framework.Graphics;
using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using System;

namespace SharpTileRenderer.Drawing.Monogame
{
    public readonly struct XnaTexture : ITexture<XnaTexture>
    {
        public string Name { get; }
        public TextureCoordinateRect Bounds { get; }
        public Texture2D Texture { get; }
        public bool Valid => !(Texture?.IsDisposed ?? true);

        public XnaTexture(string name, Texture2D texture) : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            var bounds = texture.Bounds;
            Bounds = new TextureCoordinateRect(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        public XnaTexture(string name, Texture2D texture, TextureCoordinateRect bounds)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Bounds = bounds;
        }

        public XnaTexture CreateSubTexture(string? name, TextureCoordinateRect bounds)
        {
            return new XnaTexture(name ?? Name, Texture, bounds);
        }
    }
}