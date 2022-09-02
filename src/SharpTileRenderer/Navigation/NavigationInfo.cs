using System;

namespace SharpTileRenderer.Navigation
{
    public readonly struct NavigationInfo : IEquatable<NavigationInfo>
    {
        /// <summary>
        ///   Indicates the number and direction of horizontal wrap-arounds.
        /// </summary>
        public readonly int WrapXIndicator;

        /// <summary>
        ///   Indicates the number and direction of vertical wrap-arounds.
        /// </summary>
        public readonly int WrapYIndicator;

        /// <summary>
        ///   Indicates that the map has been limited horizontally.
        /// </summary>
        public readonly bool LimitedX;

        /// <summary>
        ///   Indicates that the map has been limited vertically.
        /// </summary>
        public readonly bool LimitedY;

        public NavigationInfo(int wrapXIndicator, int wrapYIndicator, bool limitedX, bool limitedY)
        {
            WrapXIndicator = wrapXIndicator;
            WrapYIndicator = wrapYIndicator;
            LimitedX = limitedX;
            LimitedY = limitedY;
        }

        public bool Equals(NavigationInfo other)
        {
            return WrapXIndicator == other.WrapXIndicator && 
                   WrapYIndicator == other.WrapYIndicator && 
                   LimitedX == other.LimitedX && 
                   LimitedY == other.LimitedY;
        }

        public override bool Equals(object? obj)
        {
            return obj is NavigationInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = WrapXIndicator;
                hashCode = (hashCode * 397) ^ WrapYIndicator;
                hashCode = (hashCode * 397) ^ LimitedX.GetHashCode();
                hashCode = (hashCode * 397) ^ LimitedY.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(NavigationInfo left, NavigationInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NavigationInfo left, NavigationInfo right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"NavigationInfo({nameof(WrapXIndicator)}: {WrapXIndicator}, {nameof(WrapYIndicator)}: {WrapYIndicator}, {nameof(LimitedX)}: {LimitedX}, {nameof(LimitedY)}: {LimitedY})";
        }
    }
}