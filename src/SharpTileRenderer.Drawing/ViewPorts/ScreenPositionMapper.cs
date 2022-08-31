using SharpTileRenderer.Drawing.Utils;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    /// <summary>
    ///    Contains the translation from screen positions to map position and their
    ///    reverse counterpart of mapping map positions to possibly multiple screen positions.
    ///
    ///    The class contains two forms of mappings: A normalized mapping takes into account
    ///    possible wrapping operations and thus is a 1:n mapping of physical map coordinates
    ///    to screen positions. (ie: In a tightly wrapped map, the same map position might be
    ///    rendered multiple times, for instance if we have a 2x2 map displayed on a 10x10 screen).
    ///
    ///    The virtual mapping ignores wrapping and possibly contains map coordinates that are
    ///    outside of the valid physical map range. Use these if you want to calculate distances
    ///    on the screen, but don't forget to normalize the map coordinate before using it on
    ///    the map itself. 
    /// </summary>
    public class ScreenPositionMapper : IScreenPositionMapper
    {
        readonly Dictionary<MapCoordinate, FreeListIndex> normalizedMapping;
        readonly Dictionary<MapCoordinate, ScreenPosition> virtualMapping;
        readonly SmartFreeList<ScreenPositionEntry> data;
        readonly IntDimension tileSize;
        
        public ScreenPositionMapper(IntDimension tileSize)
        {
            this.tileSize = tileSize;
            normalizedMapping = new Dictionary<MapCoordinate, FreeListIndex>();
            virtualMapping = new Dictionary<MapCoordinate, ScreenPosition>();
            data = new SmartFreeList<ScreenPositionEntry>();
        }

        public void Clear()
        {
            normalizedMapping.Clear();
            data.Clear();
        }

        public void AddPhysical(in MapCoordinate c, in ScreenPosition p)
        {
            // Treats ScreenPositionEntry as linked list, inserting entry at front.
            var dto = normalizedMapping.TryGetValue(c, out var fi) ? new ScreenPositionEntry(p, fi) : new ScreenPositionEntry(p, FreeListIndex.Empty);
            var idx = data.Add(dto);
            normalizedMapping[c] = idx;
        }

        public void AddVirtual(in MapCoordinate c, in ScreenPosition p)
        {
            virtualMapping[c] = p;
        }

        public bool TryMapVirtual(VirtualMapCoordinate pos, out ScreenPosition sp)
        {
            return virtualMapping.TryGetValue(pos.Normalize(), out sp);
        }

        public bool TryMapPhysical(ContinuousMapCoordinate pos, List<ScreenPosition> results)
        {
            results.Clear();

            var normalized = pos.Normalize();
            var deltaX = (pos.X - normalized.X) * tileSize.Width;
            var deltaY = (pos.Y - normalized.Y) * tileSize.Height;
            if (!normalizedMapping.TryGetValue(normalized, out var idx))
            {
                return false;
            }

            while (!idx.IsEmpty)
            {
                var screenPositionEntry = data[idx];
                var screenPos = screenPositionEntry.Position;
                results.Add(new ScreenPosition(screenPos.X + deltaX, screenPos.Y + deltaY));
                idx = screenPositionEntry.Next;
            }

            return true;
        }

        readonly struct ScreenPositionEntry : ISmartFreeListElement<ScreenPositionEntry>
        {
            public readonly ScreenPosition Position;
            public readonly FreeListIndex Next;

            public FreeListIndex FreePointer => Next;

            public ScreenPositionEntry AsFreePointer(FreeListIndex ptr)
            {
                return new ScreenPositionEntry(new ScreenPosition(float.NaN, float.NaN), ptr);
            }

            public ScreenPositionEntry(ScreenPosition position, FreeListIndex next)
            {
                Position = position;
                Next = next;
            }
        }
    }
}