using Serilog;
using System;

namespace SharpTileRenderer.Navigation.Navigators
{
    class LimitedRangeNavigator<T> : IMapNavigator<T>
        where T : struct
    {
        readonly ILogger logger = SLog.ForContext<LimitedRangeNavigator<T>>();
        readonly int lowerX;
        readonly int lowerY;
        readonly IMapNavigator<T> parent;
        readonly int upperX;
        readonly int upperY;

        public LimitedRangeNavigator(IMapNavigator<T> parent, Range horizontal, Range vertical)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.lowerX = Math.Min(horizontal.Min, horizontal.Max);
            this.upperX = Math.Max(horizontal.Min, horizontal.Max) - 1;
            if (upperX <= lowerX)
            {
                throw new ArgumentException("Invalid range");
            }

            this.lowerY = Math.Min(vertical.Min, vertical.Max);
            this.upperY = Math.Max(vertical.Min, vertical.Max) - 1;
            if (upperY <= lowerY)
            {
                throw new ArgumentException("Invalid range");
            }
        }

        public NavigatorMetaData MetaData => parent.MetaData
                                                   .WithHorizontalLimit(new Range(lowerX, upperY + 1))
                                                   .WithVerticalLimit(new Range(lowerY, upperY + 1));

        public bool NavigateTo(T direction, in MapCoordinate origin, out MapCoordinate result, int steps)
        {
            bool resultFlag = parent.NavigateTo(direction, origin, out result, steps);

            var clampedX = Clamp(result.X, lowerX, upperX);
            var clampedY = Clamp(result.Y, lowerY, upperY);

            var xValid = clampedX == result.X;
            var yValid = clampedY == result.Y;
            if (xValid && yValid)
            {
                return resultFlag;
            }

            logger.Verbose("Invalid navigation {X}, {Y} vs {Result}", clampedX, clampedY, result);
            return false;
        }

        public bool Navigate(T direction, in MapCoordinate origin, out MapCoordinate result, out NavigationInfo info, int steps = 1)
        {
            bool resultFlag = parent.Navigate(direction, origin, out result, out info, steps);

            var clampedX = Clamp(result.X, lowerX, upperX);
            var clampedY = Clamp(result.Y, lowerY, upperY);

            var xValid = clampedX == result.X;
            var yValid = clampedY == result.Y;
            if (xValid && yValid)
            {
                return resultFlag;
            }

            info = new NavigationInfo(0, 0, info.LimitedX || !xValid, info.LimitedY || !yValid);
            logger.Verbose("Invalid navigation {X}, {Y} vs {Result}", clampedX, clampedY, result);
            return false;
        }

        static int Clamp(int value, int lower, int upper)
        {
            return Math.Min(Math.Max(value, lower), upper);
        }
    }

    class LimitedHorizontalRangeNavigator<T> : IMapNavigator<T>
        where T : struct
    {
        readonly ILogger logger = SLog.ForContext<LimitedHorizontalRangeNavigator<T>>();
        readonly int lowerX;
        readonly IMapNavigator<T> parent;
        readonly int upperX;

        public LimitedHorizontalRangeNavigator(IMapNavigator<T> parent, Range horizontal)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.lowerX = Math.Min(horizontal.Min, horizontal.Max);
            this.upperX = Math.Max(horizontal.Min, horizontal.Max) - 1;
            if (upperX <= lowerX)
            {
                throw new ArgumentException("Invalid range");
            }
        }

        public NavigatorMetaData MetaData => parent.MetaData.WithHorizontalLimit(new Range(lowerX, upperX + 1));

        public bool NavigateTo(T direction, in MapCoordinate origin, out MapCoordinate result, int steps)
        {
            bool resultFlag = parent.NavigateTo(direction, origin, out result, steps);

            var clampedX = Clamp(result.X, lowerX, upperX);
            var validNavigation = clampedX == result.X;
            if (!validNavigation)
            {
                logger.Verbose("Invalid navigation {X}, {Y} vs {Result}", clampedX, result.Y, result);
            }

            return resultFlag && validNavigation;
        }

        public bool Navigate(T direction, in MapCoordinate origin, out MapCoordinate result, out NavigationInfo info, int steps = 1)
        {
            bool resultFlag = parent.Navigate(direction, origin, out result, out info, steps);

            var clampedX = Clamp(result.X, lowerX, upperX);
            var validNavigation = clampedX == result.X;
            if (!validNavigation)
            {
                logger.Verbose("Invalid navigation {X}, {Y} vs {Result}", clampedX, result.Y, result);
                info = new NavigationInfo(0, info.WrapYIndicator, true, info.LimitedY);
            }

            return resultFlag && validNavigation;
        }

        static int Clamp(int value, int lower, int upper)
        {
            return Math.Min(Math.Max(value, lower), upper);
        }
    }

    class LimitedVerticalRangeNavigator<T> : IMapNavigator<T>
        where T : struct
    {
        readonly ILogger logger = SLog.ForContext<LimitedHorizontalRangeNavigator<T>>();
        readonly int lowerY;
        readonly IMapNavigator<T> parent;
        readonly int upperY;

        public LimitedVerticalRangeNavigator(IMapNavigator<T> parent, Range vertical)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.lowerY = Math.Min(vertical.Min, vertical.Max);
            this.upperY = Math.Max(vertical.Min, vertical.Max) - 1;
            if (upperY <= lowerY)
            {
                throw new ArgumentException("Invalid range");
            }
        }

        public NavigatorMetaData MetaData => parent.MetaData.WithVerticalLimit(new Range(lowerY, upperY + 1));

        public bool NavigateTo(T direction, in MapCoordinate origin, out MapCoordinate result, int steps)
        {
            bool resultFlag = parent.NavigateTo(direction, origin, out result, steps);

            var clampedY = Clamp(result.Y, lowerY, upperY);
            var validNavigation = clampedY == result.X;
            if (!validNavigation)
            {
                logger.Verbose("Invalid navigation {X}, {Y} vs {Result}", result.X, clampedY, result);
            }

            return resultFlag && validNavigation;
        }

        public bool Navigate(T direction, in MapCoordinate origin, out MapCoordinate result, out NavigationInfo info, int steps = 1)
        {
            bool resultFlag = parent.Navigate(direction, origin, out result, out info, steps);

            var clampedY = Clamp(result.Y, lowerY, upperY);
            var validNavigation = clampedY == result.X;
            if (!validNavigation)
            {
                logger.Verbose("Invalid navigation {X}, {Y} vs {Result}", result.X, clampedY, result);
                info = new NavigationInfo(info.WrapXIndicator, 0, info.LimitedX, true);
            }

            return resultFlag && validNavigation;
        }

        static int Clamp(int value, int lower, int upper)
        {
            return Math.Min(Math.Max(value, lower), upper);
        }
    }
}