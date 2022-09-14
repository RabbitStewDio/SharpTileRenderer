using Serilog;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    public class ScreenPositionMapping : IScreenPositionMapper
    {
        static readonly ILogger logger = SLog.ForContext<ScreenPositionMapping>();

        readonly GridType gridType;
        readonly ScreenPositionMapper mapper;
        IMapNavigator<GridDirection>? navigator;
        Optional<NavigatorMetaData> metaData;
        Optional<MapArea> bounds;

        public ScreenPositionMapping(GridType gridType, IntDimension tileSize)
        {
            this.gridType = gridType;
            this.mapper = new ScreenPositionMapper(gridType, tileSize);
        }

        IMapNavigator<GridDirection> Fetch(NavigatorMetaData md)
        {
            var normalizedMetaData = md.WithoutHorizontalOperation()
                                       .WithoutVerticalOperation();

            if (metaData.TryGetValue(out var cachedMd) && normalizedMetaData == cachedMd)
            {
                Debug.Assert(navigator != null, nameof(navigator) + " != null");
                return navigator;
            }

            navigator = normalizedMetaData.BuildNavigator();
            metaData = normalizedMetaData;
            return navigator;
        }

        public bool TryMapVirtual(VirtualMapCoordinate pos, out ScreenPosition sp)
        {
            return mapper.TryMapVirtual(pos, out sp);
        }

        public bool TryMapPhysical(ContinuousMapCoordinate pos, List<ScreenPosition> results)
        {
            return mapper.TryMapPhysical(pos, results);
        }

        public void Refresh(IViewPort v)
        {
            var centre = v.Focus;
            var tileBounds = v.TileBounds;
            var queryArea = new MapArea(centre.Normalize(),
                                        (int)Math.Ceiling(tileBounds.Width / 2),
                                        (int)Math.Ceiling(tileBounds.Height / 2));
            if (bounds.TryGetValue(out var cachedBounds) && cachedBounds == queryArea)
            {
                return;
            }

            bounds = queryArea;
            mapper.Clear();

            switch (v.TileShape)
            {
                case TileShape.Grid:
                {
                    ProcessGrid(v);
                    break;
                }
                case TileShape.Isometric:
                {
                    ProcessIsometric(v);
                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }
        }

        void ProcessGrid(IViewPort v)
        {
            var activeArea = v.PixelBounds + v.PixelOverdraw;
            var screenPos = new ScreenPosition(activeArea.X, activeArea.Y);
            var viewToWorld = v.ScreenSpaceNavigator.TranslateViewToWorld(v, screenPos);
            var origin = viewToWorld.VirtualCoordinate.Normalize();
            var tileSize = v.TileSize;
            var tileBounds = activeArea / tileSize;
            var mapNavigator = v.Navigation[MapNavigationType.Map];
            var mapNav = Fetch(mapNavigator.MetaData);


            for (int stepY = 0; stepY < tileBounds.Height; stepY += 1)
            {
                var lineOrigin = origin;
                var lineOriginScreen = screenPos;
                for (int stepX = 0; stepX < tileBounds.Width; stepX += 1)
                {
                    if (mapNavigator.NavigateTo(GridDirection.None, lineOrigin, out var mc, 0))
                    {
                        mapper.AddPhysical(mc, lineOriginScreen);
                    }

                    mapper.AddVirtual(lineOrigin, lineOriginScreen);

                    if (!mapNav.NavigateTo(GridDirection.East, lineOrigin, out lineOrigin))
                    {
                        break;
                    }

                    lineOriginScreen = new ScreenPosition(lineOriginScreen.X + tileSize.Width, lineOriginScreen.Y);
                }

                if (!mapNav.NavigateTo(GridDirection.South, origin, out origin))
                {
                    break;
                }

                screenPos = new ScreenPosition(screenPos.X, lineOriginScreen.Y + tileSize.Height);
            }
        }

        void ProcessIsometric(IViewPort v)
        {
            var normalizedNavigator = v.Navigation[MapNavigationType.Screen];
            var mapNavigator = Fetch(normalizedNavigator.MetaData);


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

            var screenPos = new ScreenPosition(focusPointScreen.X - tilesToBoundsOrigin.Item1 * v.TileSize.Width, 
                                               focusPointScreen.Y - tilesToBoundsOrigin.Item2 * v.TileSize.Height);

            var tileSize = v.TileSize;
            var tileBounds = activeBounds / tileSize;


            logger.Debug("== Using navigator {Navigator} with origin {Origin}", normalizedNavigator.MetaData, origin);

            for (int stepY = 0; stepY < tileBounds.Height * 2; stepY += 1)
            {
                logger.Debug("== Processing Screen={ScreenPos} Map={Origin}", screenPos, origin);
                var lineOrigin = origin;
                var lineOriginScreen = screenPos;
                for (int stepX = 0; stepX < tileBounds.Width; stepX += 1)
                {
                    if (normalizedNavigator.NavigateTo(GridDirection.None, lineOrigin, out var mc, 0))
                    {
                        mapper.AddPhysical(mc, lineOriginScreen);
                        mapper.AddVirtual(mc, lineOriginScreen);
                        logger.Debug("   Physical Screen={LineOriginScreen} Map={MapCoordinate}", lineOriginScreen, mc);
                    }

                    mapper.AddVirtual(lineOrigin, lineOriginScreen);
                    logger.Debug("   Virtual Screen={LineOriginScreen} Map={LineOrigin}", lineOriginScreen, lineOrigin);

                    if (!mapNavigator.NavigateTo(GridDirection.East, lineOrigin, out lineOrigin))
                    {
                        break;
                    }

                    lineOriginScreen = new ScreenPosition(lineOriginScreen.X + tileSize.Width, lineOriginScreen.Y);
                }

                var (navDirection, navFactor) = (stepY % 2) == 0 ? (GridDirection.SouthEast, +1) : (GridDirection.SouthWest, -1);
                if (!mapNavigator.NavigateTo(navDirection, origin, out origin))
                {
                    break;
                }

                screenPos = new ScreenPosition(screenPos.X + navFactor * (tileSize.Width / 2f), lineOriginScreen.Y + tileSize.Height / 2f);
            }
        }
    }
}