using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpTileRenderer.TexturePack;
using System;
using System.IO;
using System.Text;

namespace SharpTileRenderer.Drawing.Monogame
{
    public class XnaContentLoader : IContentLoader<XnaTexture>
    {
        readonly ContentManager contentManager;
        readonly GraphicsDeviceManager device;
        readonly ContentUri root;

        public XnaContentLoader(ContentManager contentManager, GraphicsDeviceManager device)
        {
            this.root = ContentUri.Make("content", "");
            this.contentManager = contentManager;
            this.device = device;
        }

        public XnaTexture LoadTexture(ContentUri name)
        {
            var targetUri = root.Combine(name);
            switch (targetUri.Scheme)
            {
                case "file":
                {
                    var texture = Texture2D.FromFile(device.GraphicsDevice, targetUri.AbsolutePath);
                    return new XnaTexture(targetUri.ToString(), texture);
                }
                case "content":
                {
                    var texture = contentManager.Load<Texture2D>(MakeRelativePath(targetUri.AbsolutePath));
                    return new XnaTexture(targetUri.ToString(), texture);
                }
                default:
                    throw new ArgumentException();
            }
        }

        public TextReader LoadText(ContentUri name)
        {
            var targetUri = root.Combine(name);
            switch (targetUri.Scheme)
            {
                case "file":
                    return File.OpenText(targetUri.AbsolutePath);
                case "content":
                    var path = Path.Combine(contentManager.RootDirectory, MakeRelativePath(targetUri.AbsolutePath));
                    return new StreamReader(TitleContainer.OpenStream(path), Encoding.UTF8);
                default:
                    throw new ArgumentException();
            }
        }

        static string MakeRelativePath(string path)
        {
            if (path.StartsWith("/") || path.StartsWith("\\"))
            {
                return path.Substring(1);
            }

            return path;
        }
    }
}