using SharpTileRenderer.Navigation.Navigators;

namespace SharpTileRenderer.Navigation
{
    public static class LimitedRangeNavigation
    {
        public static IMapNavigator<T> LimitVertical<T>(this IMapNavigator<T> parent, Range y)
            where T : struct
        {
            return new LimitedVerticalRangeNavigator<T>(parent, y);
            
        }

        public static IMapNavigator<T> LimitHorizontal<T>(this IMapNavigator<T> parent, Range x)
            where T : struct
        {
            return new LimitedHorizontalRangeNavigator<T>(parent, x);
            
        }
        
        public static IMapNavigator<T> Limit<T>(this IMapNavigator<T> parent, Range x, Range y)
            where T : struct
        {
            return new LimitedRangeNavigator<T>(parent, x, y);
        }
    }
}