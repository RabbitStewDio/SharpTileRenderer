using System;

namespace SharpTileRenderer.Strategy.Base.Map
{
    public readonly struct Point : IEquatable<Point>
    {
        public readonly int X;
        public readonly int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }
    }
}