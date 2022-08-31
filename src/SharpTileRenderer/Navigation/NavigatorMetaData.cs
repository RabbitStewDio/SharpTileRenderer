using SharpTileRenderer.TexturePack;
using SharpTileRenderer.Util;
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
        public int Rotation { get; }

        NavigatorMetaData(GridType gridType) : this()
        {
            GridType = gridType;
        }

        public static NavigatorMetaData FromGridType(GridType t) => new NavigatorMetaData(t);
        
        public NavigatorMetaData(GridType gridType,
                                 MapBorderOperation horizontalOperation,
                                 MapBorderOperation verticalOperation,
                                 Optional<Range> horizontalRange,
                                 Optional<Range> verticalRange,
                                 int rotation)
        {
            HorizontalBorderOperation = horizontalOperation;
            HorizontalRange = horizontalRange;
            VerticalBorderOperation = verticalOperation;
            VerticalRange = verticalRange;
            GridType = gridType;
            Rotation = rotation;
        }

        public NavigatorMetaData WithHorizontalLimit(int max) => WithHorizontalLimit(new Range(0, max));
        public NavigatorMetaData WithHorizontalLimit(int min, int max) => WithHorizontalLimit(new Range(min, max));
        public NavigatorMetaData WithVerticalLimit(int max) => WithVerticalLimit(new Range(0, max));
        public NavigatorMetaData WithVerticalLimit(int min, int max) => WithVerticalLimit(new Range(min, max));
        public NavigatorMetaData WithHorizontalWrap(int max) => WithHorizontalWrap(new Range(0, max));
        public NavigatorMetaData WithHorizontalWrap(int min, int max) => WithHorizontalWrap(new Range(min, max));
        public NavigatorMetaData WithVerticalWrap(int max) => WithVerticalWrap(new Range(0, max));
        public NavigatorMetaData WithVerticalWrap(int min, int max) => WithVerticalWrap(new Range(min, max));
        public NavigatorMetaData WithRotation(int rotation) => new NavigatorMetaData(GridType, HorizontalBorderOperation, VerticalBorderOperation, HorizontalRange, VerticalRange, rotation);
        
        public NavigatorMetaData WithHorizontalLimit(Range horizontalLimit)
        {
            return new NavigatorMetaData(GridType, MapBorderOperation.Limit, VerticalBorderOperation, horizontalLimit, VerticalRange, Rotation);
        }

        public NavigatorMetaData WithHorizontalWrap(Range horizontalLimit)
        {
            return new NavigatorMetaData(GridType, MapBorderOperation.Wrap, VerticalBorderOperation, horizontalLimit, VerticalRange, Rotation);
        }

        public NavigatorMetaData WithoutHorizontalOperation() => new NavigatorMetaData(GridType, MapBorderOperation.None, VerticalBorderOperation, default, VerticalRange, Rotation);
        public NavigatorMetaData WithoutHorizontalLimit() => HorizontalBorderOperation == MapBorderOperation.Limit ? 
            new NavigatorMetaData(GridType, MapBorderOperation.None, VerticalBorderOperation, default, VerticalRange, Rotation) : this;

        public NavigatorMetaData WithVerticalLimit(Range verticalLimit)
        {
            return new NavigatorMetaData(GridType, HorizontalBorderOperation, MapBorderOperation.Limit, HorizontalRange, verticalLimit, Rotation);
        }

        public NavigatorMetaData WithVerticalWrap(Range verticalLimit)
        {
            return new NavigatorMetaData(GridType, HorizontalBorderOperation, MapBorderOperation.Wrap, HorizontalRange, verticalLimit, Rotation);
        }

        public NavigatorMetaData WithoutVerticalOperation() => new NavigatorMetaData(GridType, HorizontalBorderOperation, MapBorderOperation.None, HorizontalRange, default, Rotation);

        public NavigatorMetaData WithoutVerticalLimit() => HorizontalBorderOperation == MapBorderOperation.Limit ? 
            new NavigatorMetaData(GridType, HorizontalBorderOperation, MapBorderOperation.None, HorizontalRange, default, Rotation) : this;

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

        public override string ToString()
        {
            return $"NavigatorMetaData({nameof(GridType)}: {GridType}, {nameof(HorizontalBorderOperation)}: {HorizontalBorderOperation}, {nameof(VerticalBorderOperation)}: {VerticalBorderOperation}, {nameof(HorizontalRange)}: {HorizontalRange}, {nameof(VerticalRange)}: {VerticalRange}, {nameof(Rotation)}: {Rotation})";
        }
    }
}