﻿using System;

namespace SharpTileRenderer.TexturePack.Operations
{
    public readonly struct TextureCoordinatePoint : IEquatable<TextureCoordinatePoint>
    {
        public readonly int X;
        public readonly int Y;

        public TextureCoordinatePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        public bool Equals(TextureCoordinatePoint other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is TextureCoordinatePoint point && Equals(point);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 31) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(TextureCoordinatePoint left, TextureCoordinatePoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextureCoordinatePoint left, TextureCoordinatePoint right)
        {
            return !left.Equals(right);
        }

        public static TextureCoordinatePoint operator +(TextureCoordinatePoint left, TextureCoordinatePoint right)
        {
            return new TextureCoordinatePoint(left.X + right.X, left.Y + right.Y);
        }

        public static TextureCoordinatePoint operator -(TextureCoordinatePoint left, TextureCoordinatePoint right)
        {
            return new TextureCoordinatePoint(left.X - right.X, left.Y - right.Y);
        }
    }
}
