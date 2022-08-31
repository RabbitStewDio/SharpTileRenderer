using System;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public sealed class CardinalSelectorKey : IEquatable<CardinalSelectorKey>
    {
        public bool North { get; }
        public bool East { get; }
        public bool South { get; }
        public bool West { get; }

        CardinalSelectorKey(bool north, bool east, bool south, bool west)
        {
            North = north;
            East = east;
            South = south;
            West = west;
        }

        public bool Equals(CardinalSelectorKey? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return North == other.North && East == other.East && South == other.South && West == other.West;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CardinalSelectorKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = North.GetHashCode();
                hashCode = (hashCode * 397) ^ East.GetHashCode();
                hashCode = (hashCode * 397) ^ South.GetHashCode();
                hashCode = (hashCode * 397) ^ West.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(CardinalSelectorKey left, CardinalSelectorKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CardinalSelectorKey left, CardinalSelectorKey right)
        {
            return !Equals(left, right);
        }

        public int LinearIndex
        {
            get
            {
                int index = 0;
                index += North ? 1 : 0;
                index += East ? 2 : 0;
                index += South ? 4 : 0;
                index += West ? 8 : 0;
                return index;
            }
        }

        static string FlagToString(bool flag)
        {
            return flag ? "1" : "0";
        }

        public override string ToString()
        {
            return $"{FlagToString(North)}{FlagToString(East)}{FlagToString(South)}{FlagToString(West)}";
        }

        public string ToString(string format)
        {
            return string.Format(format, FlagToString(North), FlagToString(East), FlagToString(South), FlagToString(West));
        }

        public static CardinalSelectorKey ValueOf(bool north, bool east, bool south, bool west)
        {
            int index = 0;
            index += north ? 1 : 0;
            index += east ? 2 : 0;
            index += south ? 4 : 0;
            index += west ? 8 : 0;
            return values[index];
        }

        public static CardinalSelectorKey[] Values
        {
            get
            {
                var x = new CardinalSelectorKey[values.Length];
                values.CopyTo(x, 0);
                return x;
            }
        }

        static readonly CardinalSelectorKey[] values;

        static CardinalSelectorKey()
        {
            values = new CardinalSelectorKey[16];
            for (var idx = 0; idx < 16; idx += 1)
            {
                var n = (idx & 1) != 0;
                var e = (idx & 2) != 0;
                var s = (idx & 4) != 0;
                var w = (idx & 8) != 0;
                values[idx] = new CardinalSelectorKey(n, e, s, w);
            }
        }
    }
}