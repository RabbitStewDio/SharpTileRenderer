using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Navigation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.Drawing.Queries
{
    public class IsometricQueryPlaner : IQueryPlaner
    {
        public List<QueryPlan> Plan(IViewPort v, List<QueryPlan>? results = null)
        {
            results ??= new List<QueryPlan>();
            results.Clear();

            Span<QueryPlan> tmpList = stackalloc QueryPlan[9];
            Span<bool> tmpActiveFlags = stackalloc bool[9];

            var normalizedNavigator = v.Navigation[MapNavigationType.Screen];
            var mapNavigator = normalizedNavigator.AsVirtualNavigator();

            // represents the rendered tile area
            var activeBounds = v.PixelBounds + v.PixelOverdraw + new ScreenInsets(v.TileSize.Height, v.TileSize.Width);
            // represents the focus point, the map position directly under the center of the active bounds area
            var focusPointScreen = v.PixelBounds.Center;
            var centerRaw = v.ScreenSpaceNavigator.TranslateViewToWorld(v, focusPointScreen).VirtualCoordinate;
            var tilesToBoundsOriginPx = (focusPointScreen - activeBounds.TopLeft);
            var tilesToBoundsOrigin = ((int)Math.Ceiling(tilesToBoundsOriginPx.X / v.TileSize.Width),
                                       (int)Math.Ceiling(tilesToBoundsOriginPx.Y / v.TileSize.Height));

            mapNavigator.NavigateTo(GridDirection.North, centerRaw.Normalize(), out var origin, tilesToBoundsOrigin.Item2);
            mapNavigator.NavigateTo(GridDirection.West, origin, out origin, tilesToBoundsOrigin.Item1);

            var tileSize = v.TileSize;
            var tileBounds = activeBounds / tileSize;
            
            for (int stepY = 0; stepY < tileBounds.Height * 2; stepY += 1)
            {
                var lineOrigin = origin;
                //Console.WriteLine($"== Line Start {lineOrigin}");
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

                var navDirection = (stepY % 2) == 0 ? GridDirection.SouthEast : GridDirection.SouthWest;
                if (!mapNavigator.NavigateTo(navDirection, origin, out origin))
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
           // Console.WriteLine($"Processing QP = {c}");
           // WrapIndicator is either -1, 0, or +1 and gets mapped to 0..2. 
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

        IMapNavigator<GridDirection> Fetch(NavigatorMetaData md)
        {
            var normalizedMetaData = md.WithoutHorizontalOperation()
                                       .WithoutVerticalOperation();
            var navigator = normalizedMetaData.BuildNavigator();
            return navigator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Clamp(int value)
        {
            return Math.Max(0, Math.Min(value, 2));
        }
    }
}