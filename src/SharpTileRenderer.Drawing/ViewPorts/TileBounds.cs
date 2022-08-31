using System;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    public readonly struct TileBounds : IEquatable<TileBounds>
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Width;
        public readonly float Height;

        public TileBounds(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public TilePosition Center => new TilePosition(X + Width / 2, Y + Height / 2);

        public bool Equals(TileBounds other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        public override bool Equals(object? obj)
        {
            return obj is TileBounds other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(TileBounds left, TileBounds right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TileBounds left, TileBounds right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"TileBounds({nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height})";
        }
    }
}