using System;

namespace SharpTileRenderer.Navigation
{
    public readonly struct ContinuousMapCoordinate : IEquatable<ContinuousMapCoordinate>
    {
        public ContinuousMapCoordinate(float x, float y)
        {
            X = x;
            Y = y;
        }

        public readonly float X;

        public readonly float Y;

        public bool Equals(ContinuousMapCoordinate other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is ContinuousMapCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(ContinuousMapCoordinate left, ContinuousMapCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ContinuousMapCoordinate left, ContinuousMapCoordinate right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{nameof(ContinuousMapCoordinate)}={{{nameof(X)}: {X}, {nameof(Y)}: {Y}}}";
        }

        public MapCoordinate Normalize()
        {
            return new MapCoordinate((int)Math.Floor(X + 0.5f), (int)Math.Floor(Y + 0.5f));
        }

        public ContinuousMapCoordinate InTilePosition()
        {
            var n = Normalize();
            return new ContinuousMapCoordinate(X - n.X, Y - n.Y);
        }

        public static implicit operator ContinuousMapCoordinate(MapCoordinate m)
        {
            return new ContinuousMapCoordinate(m.X, m.Y);
        }
    }
}