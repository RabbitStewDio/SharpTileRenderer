using SharpTileRenderer.Navigation.Navigators;
using System;

namespace SharpTileRenderer.Navigation
{
    public static class GridNavigation
    {
        public static MapCoordinate[] NavigateNeighbours(this IMapNavigator<GridDirection> nav,
                                                         MapCoordinate coord,
                                                         MapCoordinate[]? retval = null)
        {
            if (retval == null || retval.Length < 8)
            {
                retval = new MapCoordinate[8];
            }

            nav.NavigateTo(GridDirection.North, coord, out retval[0]);
            nav.NavigateTo(GridDirection.NorthEast, coord, out retval[1]);
            nav.NavigateTo(GridDirection.East, coord, out retval[2]);
            nav.NavigateTo(GridDirection.SouthEast, coord, out retval[3]);
            nav.NavigateTo(GridDirection.South, coord, out retval[4]);
            nav.NavigateTo(GridDirection.SouthWest, coord, out retval[5]);
            nav.NavigateTo(GridDirection.West, coord, out retval[6]);
            nav.NavigateTo(GridDirection.NorthWest, coord, out retval[7]);
            return retval;
        }

        public static MapCoordinate[] NavigateCardinalNeighbours(this IMapNavigator<GridDirection> nav,
                                                                 MapCoordinate coord,
                                                                 MapCoordinate[]? retval = null)
        {
            if (retval == null || retval.Length < 4)
            {
                retval = new MapCoordinate[4];
            }

            nav.NavigateTo(GridDirection.North, coord, out retval[0]);
            nav.NavigateTo(GridDirection.East, coord, out retval[1]);
            nav.NavigateTo(GridDirection.South, coord, out retval[2]);
            nav.NavigateTo(GridDirection.West, coord, out retval[3]);
            return retval;
        }

        public static IMapNavigator<GridDirection> CreateNavigator(GridType type)
        {
            return type switch
            {
                GridType.Grid => new GridNavigator(),
                GridType.IsoStaggered => new IsoStaggeredGridNavigator(),
                GridType.IsoDiamond => new IsoDiamondGridNavigator(),
                GridType.Hex => throw new ArgumentException("Hex is not a supported grid navigation schema."),
                GridType.HexDiamond => throw new ArgumentException("Hex is not a supported grid navigation schema."),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static IMapNavigator<GridDirection> BuildNavigator(this NavigatorMetaData md)
        {
            var root = CreateNavigator(md.GridType);
            if (md.HorizontalBorderOperation == md.VerticalBorderOperation)
            {
                root = md.HorizontalBorderOperation switch
                {
                    MapBorderOperation.None => root,
                    MapBorderOperation.Limit => root.Limit(md.HorizontalRange.Value, md.VerticalRange.Value),
                    MapBorderOperation.Wrap => root.Wrap(md.HorizontalRange.Value, md.VerticalRange.Value),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else
            {
                root = md.HorizontalBorderOperation switch
                {
                    MapBorderOperation.None => root,
                    MapBorderOperation.Limit => root.LimitHorizontal(md.HorizontalRange.Value),
                    MapBorderOperation.Wrap => root.WrapHorizontal(md.HorizontalRange.Value),
                    _ => throw new ArgumentOutOfRangeException()
                };
                root = md.VerticalBorderOperation switch
                {
                    MapBorderOperation.None => root,
                    MapBorderOperation.Limit => root.LimitVertical(md.VerticalRange.Value),
                    MapBorderOperation.Wrap => root.WrapVertical(md.VerticalRange.Value),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (md.Rotation != 0)
            {
                root = new FixedOffsetRotationGridNavigator(root, md.Rotation);
            }

            return root;
        }

        public static MapCoordinate NavigateUnconditionally(this IMapNavigator<GridDirection> nav,
                                                            GridDirection d,
                                                            MapCoordinate coord,
                                                            int steps = 1)
        {
            nav.NavigateTo(d, coord, out MapCoordinate results, steps);
            return results;
        }

        public static IMapNavigator<GridDirection> AsVirtualNavigator(this IMapNavigator<GridDirection> nav)
        {
            var md = nav.MetaData.WithoutHorizontalOperation().WithoutVerticalOperation();
            return md.BuildNavigator();
        }
    }
}