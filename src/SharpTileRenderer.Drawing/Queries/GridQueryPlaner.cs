using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Navigation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.Drawing.Queries
{
    public class GridQueryPlaner : IQueryPlaner
    {
        public List<QueryPlan> Plan(IViewPort v, List<QueryPlan>? results = null)
        {
            results ??= new List<QueryPlan>();
            results.Clear();

            Span<QueryPlan> tmpList = stackalloc QueryPlan[9];
            Span<bool> tmpActiveFlags = stackalloc bool[9];
            
            var activeArea = v.PixelBounds + v.PixelOverdraw;
            var screenPos = new ScreenPosition(activeArea.X, activeArea.Y);
            var viewToWorld = v.ScreenSpaceNavigator.TranslateViewToWorld(v, screenPos);
            var origin = viewToWorld.VirtualCoordinate.Normalize();
            var tileSize = v.TileSize;
            var tileBounds = activeArea / tileSize;
            
            var mapNavigator = v.Navigation[MapNavigationType.Map].AsVirtualNavigator();
            var normalizedNavigator = v.Navigation[MapNavigationType.Map];

            for (int stepY = 0; stepY < tileBounds.Height; stepY += 1)
            {
                var lineOrigin = origin;
                for (int stepX = 0; stepX < tileBounds.Width; stepX += 1)
                {
                    if (normalizedNavigator.Navigate(GridDirection.None, lineOrigin, out var currentCoord, out var navInfo))
                    {
                        ProcessCoordinate(currentCoord, navInfo, tmpList, tmpActiveFlags);
                    }
                    
                    if (!mapNavigator.NavigateTo(GridDirection.East, lineOrigin, out lineOrigin))
                    {
                        break;
                    }
                }

                if (!mapNavigator.NavigateTo(GridDirection.South, origin, out origin))
                {
                    break;
                }
            }
            
            for (var i = 0; i < 9; i += 1)
            {
                if (tmpActiveFlags[i])
                {
                    results.Add(tmpList[i]);
                }
            }
            
            // at maximum, this list now contains 9 query zones with possible overlapping.
            // simplify by merging zones.
            return results;
        }

        void ProcessCoordinate(in MapCoordinate c, in NavigationInfo info, in Span<QueryPlan> queryPlan, in Span<bool> activeFlags)
        {
            var x = Clamp(info.WrapXIndicator + 1);
            var y = Clamp(info.WrapYIndicator + 1);
            var pos = y * 3 + x;
            if (activeFlags[pos])
            {
                queryPlan[pos] = queryPlan[pos].Expand(c);
            }
            else
            {
                activeFlags[pos] = true;
                queryPlan[pos] = QueryPlan.FromSingle(c);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Clamp(int value)
        {
            return Math.Max(0, Math.Min(value, 2));
        }
    }
}