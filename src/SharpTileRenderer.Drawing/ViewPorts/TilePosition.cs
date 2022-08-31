using System;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    public readonly struct TilePosition : IEquatable<TilePosition>
    {
        public readonly float X;
        public readonly float Y;

        public TilePosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(TilePosition other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is TilePosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(TilePosition left, TilePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TilePosition left, TilePosition right)
        {
            return !left.Equals(right);
        }
    }
}