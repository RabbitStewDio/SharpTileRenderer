using SharpTileRenderer.Navigation;
using System;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.Drawing.Queries
{
    /// <summary>
    ///   a rectangular area over the map. 
    /// </summary>
    public readonly struct QueryPlan : IEquatable<QueryPlan>
    {
        public readonly ContinuousMapCoordinate UpperLeft;
        public readonly ContinuousMapCoordinate LowerRight;

        QueryPlan(ContinuousMapCoordinate upperLeft,
                  ContinuousMapCoordinate lowerRight)
        {
            UpperLeft = upperLeft;
            LowerRight = lowerRight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryPlan FromSingle(ContinuousMapCoordinate c) => new QueryPlan(c, c);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryPlan FromCorners(ContinuousMapCoordinate upperLeft,
                                            ContinuousMapCoordinate lowerRight) => new QueryPlan(upperLeft, lowerRight);

        public QueryPlan Expand(ContinuousMapCoordinate c)
        {
            var minX = Math.Min(c.X, Math.Min(UpperLeft.X, LowerRight.X));
            var minY = Math.Min(c.Y, Math.Min(UpperLeft.Y, LowerRight.Y));
            var maxX = Math.Max(c.X, Math.Max(UpperLeft.X, LowerRight.X));
            var maxY = Math.Max(c.Y, Math.Max(UpperLeft.Y, LowerRight.Y));
            
            return new QueryPlan(new ContinuousMapCoordinate(minX, minY), new ContinuousMapCoordinate(maxX, maxY));
        }
        
        public ContinuousMapArea ToGridArea()
        {
            var minX = Math.Min(UpperLeft.X, LowerRight.X);
            var minY = Math.Min(UpperLeft.Y, LowerRight.Y);
            var maxX = Math.Max(UpperLeft.X, LowerRight.X);
            var maxY = Math.Max(UpperLeft.Y, LowerRight.Y);
            return new ContinuousMapArea(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        public bool Equals(QueryPlan other)
        {
            return UpperLeft.Equals(other.UpperLeft) &&
                   LowerRight.Equals(other.LowerRight);
        }

        public override bool Equals(object? obj)
        {
            return obj is QueryPlan other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = UpperLeft.GetHashCode();
                hashCode = (hashCode * 397) ^ LowerRight.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(QueryPlan left, QueryPlan right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(QueryPlan left, QueryPlan right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"QueryPlan({nameof(UpperLeft)}: {UpperLeft}, {nameof(LowerRight)}: {LowerRight})";
        }
    }
}