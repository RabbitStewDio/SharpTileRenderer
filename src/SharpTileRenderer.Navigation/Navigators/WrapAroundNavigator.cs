using System;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.Navigation.Navigators
{
    class WrapAroundNavigator<T> : IMapNavigator<T>
        where T : struct
    {
        readonly int lowerX;
        readonly int lowerY;
        readonly int deltaX;
        readonly int deltaY;
        readonly IMapNavigator<T> parent;

        public WrapAroundNavigator(IMapNavigator<T> parent, Range x, Range y)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));

            lowerX = Math.Min(x.Min, x.Max);
            lowerY = Math.Min(y.Min, y.Max);

            var upperX = Math.Max(x.Min, x.Max);
            var upperY = Math.Max(y.Min, y.Max);
            deltaX = upperX - lowerX;
            deltaY = upperY - lowerY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int WrapX(int value)
        {
            return ((value - lowerX) % deltaX + deltaX) % deltaX + lowerX;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int WrapY(int value)
        {
            return ((value - lowerY) % deltaY + deltaY) % deltaY + lowerY;
        }

        public WrapAroundNavigator(IMapNavigator<T> parent, int upperX, int upperY) : this(parent, new Range(0, upperX), new Range(0, upperY))
        {
        }

        public bool NavigateTo(T direction, in MapCoordinate origin, out MapCoordinate result, int steps)
        {
            var resultFlag = parent.NavigateTo(direction, origin, out result, steps);
            result = new MapCoordinate(WrapX(result.X), WrapY(result.Y));
            return resultFlag;
        }

        public bool Navigate(T direction, in MapCoordinate origin, out MapCoordinate result, out NavigationInfo info, int steps = 1)
        {
            var resultFlag = parent.Navigate(direction, origin, out var rawResult, out info, steps);
            result = new MapCoordinate(WrapX(rawResult.X), WrapY(rawResult.Y));
            var wrapIndicatorX = (rawResult.X < lowerX) ? -1 : (rawResult.X > (deltaX + lowerX)) ? 1 : 0;
            var wrapIndicatorY = (rawResult.Y < lowerY) ? -1 : (rawResult.Y > (deltaY + lowerY)) ? 1 : 0;
            info = new NavigationInfo(wrapIndicatorX, wrapIndicatorY, info.LimitedX, info.LimitedY);
            return resultFlag;
        }

        public NavigatorMetaData MetaData => parent.MetaData
                                                   .WithHorizontalWrap(new Range(lowerX, lowerX + deltaX))
                                                   .WithVerticalWrap(new Range(lowerY, lowerY + deltaY));
    }

    class WrapAroundVertical<T> : IMapNavigator<T>
        where T : struct
    {
        readonly int lowerY;
        readonly int deltaY;
        readonly IMapNavigator<T> parent;

        public WrapAroundVertical(IMapNavigator<T> parent, Range y)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));

            lowerY = Math.Min(y.Min, y.Max);

            var upperY = Math.Max(y.Min, y.Max);
            deltaY = upperY - lowerY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int WrapY(int value)
        {
            return ((value - lowerY) % deltaY + deltaY) % deltaY + lowerY;
        }

        public WrapAroundVertical(IMapNavigator<T> parent, int upperY) : this(parent, new Range(0, upperY))
        {
        }

        public NavigatorMetaData MetaData => parent.MetaData.WithVerticalWrap(new Range(lowerY, lowerY + deltaY));

        public bool NavigateTo(T direction, in MapCoordinate origin, out MapCoordinate result, int steps)
        {
            var resultFlag = parent.NavigateTo(direction, origin, out result, steps);
            result = result.WithY(WrapY(result.Y));
            return resultFlag;
        }

        public bool Navigate(T direction, in MapCoordinate origin, out MapCoordinate result, out NavigationInfo info, int steps = 1)
        {
            var resultFlag = parent.Navigate(direction, origin, out var rawResult, out info, steps);
            result = new MapCoordinate(rawResult.X, WrapY(rawResult.Y));
            var wrapIndicatorY = (rawResult.Y < lowerY) ? -1 : (rawResult.Y > (deltaY + lowerY)) ? 1 : 0;
            info = new NavigationInfo(info.WrapXIndicator, wrapIndicatorY, info.LimitedX, info.LimitedY);
            return resultFlag;
        }
    }

    class WrapAroundHorizontal<T> : IMapNavigator<T>
        where T : struct
    {
        readonly int lowerX;
        readonly int deltaX;
        readonly IMapNavigator<T> parent;

        public WrapAroundHorizontal(IMapNavigator<T> parent, Range x)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));

            lowerX = Math.Min(x.Min, x.Max);

            var upperX = Math.Max(x.Min, x.Max);
            deltaX = upperX - lowerX;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int WrapX(int value)
        {
            return ((value - lowerX) % deltaX + deltaX) % deltaX + lowerX;
        }

        public WrapAroundHorizontal(IMapNavigator<T> parent, int upperX) : this(parent, new Range(0, upperX))
        {
        }

        public NavigatorMetaData MetaData => parent.MetaData.WithHorizontalWrap(new Range(lowerX, lowerX + deltaX));

        public bool NavigateTo(T direction, in MapCoordinate origin, out MapCoordinate result, int steps)
        {
            var resultFlag = parent.NavigateTo(direction, origin, out result, steps);
            result = result.WithX(WrapX(result.X));
            return resultFlag;
        }

        public bool Navigate(T direction, in MapCoordinate origin, out MapCoordinate result, out NavigationInfo info, int steps = 1)
        {
            var resultFlag = parent.Navigate(direction, origin, out var rawResult, out info, steps);
            result = new MapCoordinate(WrapX(rawResult.X), rawResult.Y);
            var wrapIndicatorX = (rawResult.X < lowerX) ? -1 : (rawResult.X > (deltaX + lowerX)) ? 1 : 0;
            info = new NavigationInfo(wrapIndicatorX, info.WrapYIndicator, info.LimitedX, info.LimitedY);
            return resultFlag;
        }
    }
}