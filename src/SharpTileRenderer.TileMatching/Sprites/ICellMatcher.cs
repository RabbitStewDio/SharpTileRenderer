using SharpTileRenderer.TileMatching.TileTags;

namespace SharpTileRenderer.TileMatching.Sprites
{
    public interface ICellMatcher
    {
        bool Match(int x, int y, out ITileTagEntrySelection result);
        int Cardinality { get; }
        ITileTagEntrySelectionFactory Owner { get; }
    }
}
