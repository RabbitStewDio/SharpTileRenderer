using System;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public sealed class DiagonalSelectorKey : IEquatable<DiagonalSelectorKey>
    {
        static readonly DiagonalSelectorKey[] values;

        static DiagonalSelectorKey()
        {
            values = new DiagonalSelectorKey[16];
            for (var idx = 0; idx < 16; idx += 1)
            {
                var nw = (idx & 1) != 0;
                var ne = (idx & 2) != 0;
                var se = (idx & 4) != 0;
                var sw = (idx & 8) != 0;
                values[idx] = new DiagonalSelectorKey(nw, ne, se, sw);
            }
        }

        DiagonalSelectorKey(bool northWest, bool northEast, bool southEast, bool southWest)
        {
            NorthWest = northWest;
            NorthEast = northEast;
            SouthEast = southEast;
            SouthWest = southWest;
        }

        public bool NorthWest { get; }

        public bool NorthEast { get; }

        public bool SouthEast { get; }

        public bool SouthWest { get; }

        public int LinearIndex
        {
            get
            {
                var index = 0;
                index += NorthWest ? 1 : 0;
                index += NorthEast ? 2 : 0;
                index += SouthEast ? 4 : 0;
                index += SouthWest ? 8 : 0;
                return index;
            }
        }

        public static DiagonalSelectorKey[] Values
        {
            get
            {
                var x = new DiagonalSelectorKey[values.Length];
                values.CopyTo(x, 0);
                return x;
            }
        }

        public bool Equals(DiagonalSelectorKey? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return NorthWest == other.NorthWest &&
                   NorthEast == other.NorthEast &&
                   SouthEast == other.SouthEast &&
                   SouthWest == other.SouthWest;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((DiagonalSelectorKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = NorthWest.GetHashCode();
                hashCode = (hashCode * 397) ^ NorthEast.GetHashCode();
                hashCode = (hashCode * 397) ^ SouthEast.GetHashCode();
                hashCode = (hashCode * 397) ^ SouthWest.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(DiagonalSelectorKey left, DiagonalSelectorKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DiagonalSelectorKey left, DiagonalSelectorKey right)
        {
            return !Equals(left, right);
        }

        static string FlagToString(bool flag)
        {
            return flag ? "1" : "0";
        }

        public override string ToString()
        {
            return $"{FlagToString(NorthWest)}{FlagToString(NorthEast)}{FlagToString(SouthEast)}{FlagToString(SouthWest)}";
        }

        public string ToString(string format)
        {
            return string.Format(format, FlagToString(NorthWest), FlagToString(NorthEast), FlagToString(SouthEast), FlagToString(SouthWest));
        }

        public static DiagonalSelectorKey ValueOf(bool north, bool east, bool south, bool west)
        {
            var index = 0;
            index += north ? 1 : 0;
            index += east ? 2 : 0;
            index += south ? 4 : 0;
            index += west ? 8 : 0;
            return values[index];
        }
    }
}