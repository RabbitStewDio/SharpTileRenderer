using SharpTileRenderer.Navigation.Navigators;

namespace SharpTileRenderer.Navigation
{
    public static class WrapAroundNavigation
    {
        public static IMapNavigator<T> WrapHorizontal<T>(this IMapNavigator<T> parent, Range x)
            where T : struct
        {
            return new WrapAroundHorizontal<T>(parent, x);
        }

        public static IMapNavigator<T> WrapVertical<T>(this IMapNavigator<T> parent, Range y)
            where T : struct
        {
            return new WrapAroundVertical<T>(parent, y);
        }

        public static IMapNavigator<T> Wrap<T>(this IMapNavigator<T> parent, Range x, Range y)
            where T : struct
        {
            return new WrapAroundNavigator<T>(parent, x, y);
        }

        public static IMapNavigator<T> Wrap<T>(this IMapNavigator<T> parent, int? x, int? y)
            where T : struct
        {
            if (x.HasValue && y.HasValue)
            {
                return new WrapAroundNavigator<T>(parent, x.Value, y.Value);
            }

            if (x.HasValue)
            {
                return new WrapAroundHorizontal<T>(parent, x.Value);
            }

            if (y.HasValue)
            {
                return new WrapAroundVertical<T>(parent, y.Value);
            }

            return parent;
        }
    }
}