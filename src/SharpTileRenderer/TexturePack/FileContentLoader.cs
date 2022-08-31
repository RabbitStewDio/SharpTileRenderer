using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SharpTileRenderer.TexturePack
{
    public class FileContentLoader: IContentLoader
    {
        static readonly ContentUri baseUri = ContentUri.Make("file", "/");
        readonly string rootPath;
        readonly ContentUri rootUri;

        public FileContentLoader(string? rootPath = null)
        {
            this.rootPath = rootPath ?? ".";
            this.rootUri = baseUri.Combine(Path.GetFullPath(this.rootPath));
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        protected string ToFilePath(ContentUri name)
        {
            var targetUri = rootUri.Combine(name);
            if (targetUri.Scheme == "file" ||
                targetUri.Scheme == "content")
            {
                return targetUri.AbsolutePath;
            }

            throw new ArgumentException();
        }
        
        public TextReader LoadText(ContentUri name)
        {
            return File.OpenText(ToFilePath(name));
        }
    }
}