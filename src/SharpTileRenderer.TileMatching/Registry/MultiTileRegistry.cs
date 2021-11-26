﻿using System.Collections;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Registry
{
    public class MultiTileRegistry<TRenderTile> : ITileRegistry<IReadOnlyList<TRenderTile>>,
                                                  IEnumerable<KeyValuePair<string, IReadOnlyList<TRenderTile>>>
    {
        readonly List<TRenderTile> fallback;
        readonly Dictionary<string, IReadOnlyList<TRenderTile>> tilesByName;

        public MultiTileRegistry(TRenderTile fallback)
        {
            this.fallback = new List<TRenderTile> {fallback};
            tilesByName = new Dictionary<string, IReadOnlyList<TRenderTile>>();
        }

        public IEnumerator<KeyValuePair<string, IReadOnlyList<TRenderTile>>> GetEnumerator()
        {
            return tilesByName.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IReadOnlyList<TRenderTile> Find(string tag)
        {
            if (tilesByName.TryGetValue(tag, out IReadOnlyList<TRenderTile> tile))
            {
                return tile;
            }

            TileRegistryTracing.EmitMissingTileWarning(tag);
            return fallback;
        }

        public bool TryFind(string tag, out IReadOnlyList<TRenderTile> tile)
        {
            return tilesByName.TryGetValue(tag, out tile);
        }

        public void Add(string name, TRenderTile tile)
        {
            if (tilesByName.TryGetValue(name, out IReadOnlyList<TRenderTile> tiles))
            {
                var list = new List<TRenderTile>(tiles) {tile};
                tilesByName[name] = list;
            }
            else
            {
                tilesByName.Add(name, new List<TRenderTile> {tile});
            }
        }
    }
}