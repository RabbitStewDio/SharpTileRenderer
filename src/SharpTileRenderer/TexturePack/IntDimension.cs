﻿using System;

namespace SharpTileRenderer.TexturePack
{
    public readonly struct IntDimension : IEquatable<IntDimension>
    {
        public readonly int Width;
        public readonly int Height;

        public IntDimension(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public bool Equals(IntDimension other)
        {
            return Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        public override bool Equals(object? obj)
        {
            return obj is IntDimension dimension && Equals(dimension);
        }

        public void Deconstruct(out int width, out int height)
        {
            width = Width;
            height = Height;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width.GetHashCode() * 31) ^ Height.GetHashCode();
            }
        }

        public static bool operator ==(IntDimension left, IntDimension right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IntDimension left, IntDimension right)
        {
            return !left.Equals(right);
        }

        public static IntDimension operator +(IntDimension left, IntDimension right)
        {
            return new IntDimension(left.Width + right.Width, left.Height + right.Height);
        }

        public static IntDimension operator -(IntDimension left, IntDimension right)
        {
            return new IntDimension(left.Width - right.Width, left.Height - right.Height);
        }

        public IntPoint CenterPoint()
        {
            return new IntPoint((int)Math.Ceiling(Width / 2f),
                                (int)Math.Ceiling(Height / 2f));
        }

        public override string ToString()
        {
            return $"({nameof(Width)}: {Width}, {nameof(Height)}: {Height})";
        }
    }
}
