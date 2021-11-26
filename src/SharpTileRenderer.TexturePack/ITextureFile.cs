using System.Collections.Generic;

namespace SharpTileRenderer.TexturePack
{
    public interface ITextureFile<TTile>
    {
        IEnumerable<TTile> ProduceTiles();
    }
}
