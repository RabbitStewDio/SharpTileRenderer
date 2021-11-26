using System;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.Navigation
{
    public readonly struct MapCoordinate : IEquatable<MapCoordinate>
    {
        public MapCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly int X;

        public readonly int Y;

        public MapCoordinate With(int x, int y)
        {
            return new MapCoordinate(x, y);
        }
        
        public MapCoordinate WithY(int y)
        {
            return new MapCoordinate(X, y);
        }
        
        public MapCoordinate WithX(int x)
        {
            return new MapCoordinate(x, Y);
        }
        
        public bool Equals(MapCoordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is MapCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator ==(MapCoordinate left, MapCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MapCoordinate left, MapCoordinate right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{nameof(MapCoordinate)}={{{nameof(X)}: {X}, {nameof(Y)}: {Y}}}";
        }

        public static MapCoordinate Normalize(double x, double y)
        {
            return new MapCoordinate((int)Math.Floor(x + 0.5), (int)Math.Floor(y + 0.5));
        }

        public static MapCoordinate ToMapCoordinate(double x, double y)
        {
            return new MapCoordinate((int)Math.Round(x), (int)Math.Round(y));
        }
    }
}
