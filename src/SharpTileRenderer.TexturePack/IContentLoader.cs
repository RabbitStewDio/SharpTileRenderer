using System;
using System.IO;

namespace SharpTileRenderer.TexturePack
{
    public interface IContentLoader<TRawTexture>
    {
        TRawTexture LoadTexture(Uri name);
        TextReader LoadText(Uri name);
    }
}
