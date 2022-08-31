using System.Collections.Generic;

namespace SharpTileRenderer.TexturePack
{
    public interface ITileCollection
    {
        TileShape TileShape { get; }
        IntDimension TileSize { get; }
        IReadOnlyList<TexturedTileSpec> ProduceTiles();
    }
}

