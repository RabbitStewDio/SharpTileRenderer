using SharpTileRenderer.Util;

namespace SharpTileRenderer.TileMatching.Selectors.TileTags
{
    public interface ITileTagEntrySelectionFactory
    {
        int Count { get; }
        ITileTagEntrySelection this[int idx] { get; }
        ITileTagEntrySelection this[string tag] { get; }
    }

    public interface ITileTagEntrySelectionFactory<TSelector> : ITileTagEntrySelectionFactory
    {
        new ITileTagEntrySelection<TSelector> this[int idx] { get; }
        new ITileTagEntrySelection<TSelector> this[string tag] { get; }
        ITileTagEntrySelection<TSelector> Lookup(TSelector key);
        public Optional<ITileTagEntrySelection<TSelector>> TryLookup(TSelector idx);
    }
}
