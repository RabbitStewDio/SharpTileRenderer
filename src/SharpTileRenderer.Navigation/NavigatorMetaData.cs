using System;

namespace SharpTileRenderer.Navigation
{
    public enum MapBorderOperation
    {
        None = 0,
        Limit = 1,
        Wrap = 2
    }

    public readonly struct NavigatorMetaData : IEquatable<NavigatorMetaData>
    {
        public GridType GridType { get; }
        public MapBorderOperation HorizontalBorderOperation { get; }
        public MapBorderOperation VerticalBorderOperation { get; }

        public Optional<Range> HorizontalRange { get; }
        public Optional<Range> VerticalRange { get; }


        public NavigatorMetaData(GridType gridType) : this()
        {
            GridType = gridType;
        }

        public NavigatorMetaData(GridType gridType,
                                 MapBorderOperation horizontalOperation,
                                 MapBorderOperation verticalOperation,
                                 Optional<Range> horizontalRange,
                                 Optional<Range> verticalRange)
        {
            HorizontalBorderOperation = horizontalOperation;
            HorizontalRange = horizontalRange;
            VerticalBorderOperation = verticalOperation;
            VerticalRange = verticalRange;
            GridType = gridType;
        }

        public NavigatorMetaData WithHorizontalLimit(Range horizontalLimit)
        {
            return new NavigatorMetaData(GridType, MapBorderOperation.Limit, VerticalBorderOperation, horizontalLimit, VerticalRange);
        }

        public NavigatorMetaData WithHorizontalWrap(Range horizontalLimit)
        {
            return new NavigatorMetaData(GridType, MapBorderOperation.Wrap, VerticalBorderOperation, horizontalLimit, VerticalRange);
        }

        public NavigatorMetaData WithoutHorizontalOperation() => new NavigatorMetaData(GridType, MapBorderOperation.None, VerticalBorderOperation, default, VerticalRange);
        public NavigatorMetaData WithoutHorizontalLimit() => HorizontalBorderOperation == MapBorderOperation.Limit ? 
            new NavigatorMetaData(GridType, MapBorderOperation.None, VerticalBorderOperation, default, VerticalRange) : this;

        public NavigatorMetaData WithVerticalLimit(Range horizontalLimit)
        {
            return new NavigatorMetaData(GridType, MapBorderOperation.Limit, VerticalBorderOperation, horizontalLimit, VerticalRange);
        }

        public NavigatorMetaData WithVerticalWrap(Range horizontalLimit)
        {
            return new NavigatorMetaData(GridType, MapBorderOperation.Wrap, VerticalBorderOperation, horizontalLimit, VerticalRange);
        }

        public NavigatorMetaData WithoutVerticalOperation() => new NavigatorMetaData(GridType, HorizontalBorderOperation, MapBorderOperation.None, HorizontalRange, default);

        public NavigatorMetaData WithoutVerticalLimit() => HorizontalBorderOperation == MapBorderOperation.Limit ? 
            new NavigatorMetaData(GridType, HorizontalBorderOperation, MapBorderOperation.None, HorizontalRange, default) : this;

        public override bool Equals(object? obj)
        {
            return obj is NavigatorMetaData other && Equals(other);
        }

        public bool Equals(NavigatorMetaData other)
        {
            return GridType == other.GridType && 
                   HorizontalBorderOperation == other.HorizontalBorderOperation && 
                   VerticalBorderOperation == other.VerticalBorderOperation &&
                   HorizontalRange.Equals(other.HorizontalRange) && 
                   VerticalRange.Equals(other.VerticalRange);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)GridType;
                hashCode = (hashCode * 397) ^ (int)HorizontalBorderOperation;
                hashCode = (hashCode * 397) ^ (int)VerticalBorderOperation;
                hashCode = (hashCode * 397) ^ HorizontalRange.GetHashCode();
                hashCode = (hashCode * 397) ^ VerticalRange.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(NavigatorMetaData left, NavigatorMetaData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NavigatorMetaData left, NavigatorMetaData right)
        {
            return !left.Equals(right);
        }
    }
}