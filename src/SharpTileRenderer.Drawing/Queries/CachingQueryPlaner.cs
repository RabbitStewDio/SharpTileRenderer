using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.Queries
{
    public class CachingQueryPlaner: IQueryPlaner
    {
        Optional<(VirtualMapCoordinate, ScreenInsets, ScreenBounds, int)> cacheKey;
        readonly List<QueryPlan> cachedResults;
        readonly IQueryPlaner planer;

        public CachingQueryPlaner(IQueryPlaner planer)
        {
            this.planer = planer ?? throw new ArgumentNullException(nameof(planer));
            this.cachedResults = new List<QueryPlan>();
        }

        public List<QueryPlan> Plan(IViewPort v, List<QueryPlan>? results = null)
        {
            results ??= new List<QueryPlan>();
            results.Clear();
            
            var currentCacheKey = (v.Focus, v.PixelOverdraw, v.PixelBounds, v.ZLayer);
            if (cacheKey.TryGetValue(out var existingCacheKey) && existingCacheKey == currentCacheKey)
            {
                for (var i = 0; i < cachedResults.Count; i++)
                {
                    results.Add(cachedResults[i]);
                }
                return results;
            }

            cachedResults.Clear();
            planer.Plan(v, cachedResults);
            cacheKey = currentCacheKey;            
            for (var i = 0; i < cachedResults.Count; i++)
            {
                results.Add(cachedResults[i]);
            }
            return results;
        }
    }
}