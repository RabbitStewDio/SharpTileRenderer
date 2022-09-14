using SharpTileRenderer.TexturePack;
using System.Collections.Generic;
using System.IO;

namespace SharpTileRenderer.Tests.TileMatching
{
    public class InMemoryContentLoader : IContentLoader
    {
        readonly Dictionary<ContentUri, string> contents;

        public InMemoryContentLoader()
        {
            this.contents = new Dictionary<ContentUri, string>();
        }

        public InMemoryContentLoader WithData(ContentUri uri, string data)
        {
            this.contents[uri] = data;
            return this;
        }

        public TextReader LoadText(ContentUri name)
        {
            if (contents.TryGetValue(name, out var data))
            {
                return new StringReader(data);
            }

            throw new IOException($"Unable to locate data for {name}");
        }
    }
}