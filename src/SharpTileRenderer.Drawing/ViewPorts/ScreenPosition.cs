using System;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    /// <summary>
    ///   A coordinate pair in pixel units. 
    /// </summary>
    public readonly struct ScreenPosition : IEquatable<ScreenPosition>
    {
        public readonly float X;
        public readonly float Y;

        public ScreenPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(ScreenPosition other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is ScreenPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(ScreenPosition left, ScreenPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ScreenPosition left, ScreenPosition right)
        {
            return !left.Equals(right);
        }

        public static ScreenPosition operator +(ScreenPosition left, VirtualMapCoordinate right)
        {
            return new ScreenPosition((float)Math.Floor(left.X + right.X), (float)Math.Floor(left.Y + right.Y));
        }

        public static ScreenPosition operator -(ScreenPosition left, VirtualMapCoordinate right)
        {
            return new ScreenPosition((float)Math.Floor(left.X - right.X), (float)Math.Floor(left.Y - right.Y));
        }

        public static ScreenPosition operator -(ScreenPosition left, ScreenPosition right)
        {
            return new ScreenPosition(left.X - right.X, left.Y - right.Y);
        }

        public static ScreenPosition operator +(ScreenPosition left, ScreenPosition right)
        {
            return new ScreenPosition(left.X + right.X, left.Y + right.Y);
        }

        public override string ToString()
        {
            return $"ScreenPosition({X}, {Y})";
        }
    }
}