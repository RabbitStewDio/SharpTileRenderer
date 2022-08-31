using SharpTileRenderer.TexturePack;
using System;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    public readonly struct ScreenBounds : IEquatable<ScreenBounds>
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Width;
        public readonly float Height;

        public ScreenBounds(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public ScreenPosition Center => new ScreenPosition(X + Width / 2, Y + Height / 2);

        public bool Equals(ScreenBounds other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        public override bool Equals(object? obj)
        {
            return obj is ScreenBounds other && Equals(other);
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

        public static bool operator ==(ScreenBounds left, ScreenBounds right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ScreenBounds left, ScreenBounds right)
        {
            return !left.Equals(right);
        }

        public static ScreenBounds operator +(ScreenBounds bounds, ScreenInsets insets)
        {
            return new ScreenBounds(bounds.X - insets.Left,
                                    bounds.Y - insets.Top,
                                    bounds.Width + insets.Right + insets.Left,
                                    bounds.Height + insets.Bottom + insets.Top
            );
        }

        public static TileBounds operator /(ScreenBounds bounds, IntDimension tileSize)
        {
            var x = (int)Math.Floor(bounds.X / tileSize.Width);
            var y = (int)Math.Floor(bounds.Y / tileSize.Height);
            var screenX2 = bounds.X + bounds.Width;
            var screenY2 = bounds.Y + bounds.Height;
            var tileX2 = (int)Math.Ceiling(screenX2 / tileSize.Width);
            var tileY2 = (int)Math.Ceiling(screenY2 / tileSize.Height);
            return new TileBounds(x, y,
                                  tileX2 - x,
                                  tileY2 - y
            );
        }
    }
}