using SharpTileRenderer.Navigation;
using System;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    /// <summary>
    ///    A virtual map coordinate. Ignores limits and wrapping to create a linear mapping between
    ///    screen space and world space.
    /// </summary>
    public readonly struct VirtualMapCoordinate : IEquatable<VirtualMapCoordinate>
    {
        public readonly float X;
        public readonly float Y;

        public VirtualMapCoordinate(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(VirtualMapCoordinate other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is VirtualMapCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(VirtualMapCoordinate left, VirtualMapCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VirtualMapCoordinate left, VirtualMapCoordinate right)
        {
            return !left.Equals(right);
        }

        public static VirtualMapCoordinate operator -(VirtualMapCoordinate left, MapCoordinate right)
        {
            return new VirtualMapCoordinate(left.X - right.X, left.Y - right.Y);
        }
        
        public static VirtualMapCoordinate operator -(MapCoordinate left, VirtualMapCoordinate right)
        {
            return new VirtualMapCoordinate(left.X - right.X, left.Y - right.Y);
        }
        
        public static VirtualMapCoordinate operator -(VirtualMapCoordinate left, VirtualMapCoordinate right)
        {
            return new VirtualMapCoordinate(left.X - right.X, left.Y - right.Y);
        }
        
        public static VirtualMapCoordinate operator +(VirtualMapCoordinate left, MapCoordinate right)
        {
            return new VirtualMapCoordinate(left.X + right.X, left.Y + right.Y);
        }
        
        public static VirtualMapCoordinate operator +(MapCoordinate left, VirtualMapCoordinate right)
        {
            return new VirtualMapCoordinate(left.X + right.X, left.Y + right.Y);
        }
        
        public static VirtualMapCoordinate operator +(VirtualMapCoordinate left, VirtualMapCoordinate right)
        {
            return new VirtualMapCoordinate(left.X + right.X, left.Y + right.Y);
        }
        
        public MapCoordinate Normalize()
        {
            return new MapCoordinate((int)Math.Floor(X + 0.5f), (int)Math.Floor(Y + 0.5f));
        }

        public MapCoordinate Ceiling()
        {
            return new MapCoordinate((int)Math.Ceiling(X), (int)Math.Ceiling(Y));
        }

        public MapCoordinate Floor()
        {
            return new MapCoordinate((int)Math.Floor(X), (int)Math.Floor(Y));
        }

        public override string ToString()
        {
            return $"VirtualMapCoordinate({X}, {Y})";
        }

        /// <summary>
        ///    Normalizes the map coordinate into virtual coordinates that are unwrapped.
        ///    Limit clamping has been removed, so any resulting coordinate is not clamped.
        /// </summary>
        /// <param name="nav"></param>
        /// <returns></returns>
        public VirtualMapCoordinate Unwrap(IMapNavigator<GridDirection> nav)
        {
            var md = nav.MetaData;
            md = md.VerticalBorderOperation switch
            {
                MapBorderOperation.Limit => md.WithoutVerticalOperation(),
                _ => md
            };

            md = md.HorizontalBorderOperation switch
            {
                MapBorderOperation.Limit => md.WithoutHorizontalOperation(),
                _ => md
            };

            var fractional = this - Normalize();
            md.BuildNavigator().Navigate(GridDirection.None, Normalize(), out var result, out _, 0);
            return fractional + result;
        }
    }
}