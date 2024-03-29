﻿using SharpTileRenderer.TileMatching.Selectors.TileTags;
using System;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public class CellGroupSelectorKey : IEquatable<CellGroupSelectorKey>
    {
        public ITileTagEntrySelection MatchA { get; }
        public ITileTagEntrySelection MatchB { get; }
        public ITileTagEntrySelection MatchC { get; }
        public ITileTagEntrySelection MatchD { get; }

        public CellGroupSelectorKey(ITileTagEntrySelection north,
                                    ITileTagEntrySelection east,
                                    ITileTagEntrySelection south,
                                    ITileTagEntrySelection west)
        {
            MatchA = north ?? throw new ArgumentNullException(nameof(north));
            MatchB = east ?? throw new ArgumentNullException(nameof(east));
            MatchC = south ?? throw new ArgumentNullException(nameof(south));
            MatchD = west ?? throw new ArgumentNullException(nameof(west));
        }

        public bool Equals(CellGroupSelectorKey? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MatchA.Equals(other.MatchA) &&
                   MatchB.Equals(other.MatchB) &&
                   MatchC.Equals(other.MatchC) &&
                   MatchD.Equals(other.MatchD);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CellGroupSelectorKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MatchA.GetHashCode();
                hashCode = (hashCode * 397) ^ MatchB.GetHashCode();
                hashCode = (hashCode * 397) ^ MatchC.GetHashCode();
                hashCode = (hashCode * 397) ^ MatchD.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(CellGroupSelectorKey left, CellGroupSelectorKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CellGroupSelectorKey left, CellGroupSelectorKey right)
        {
            return !Equals(left, right);
        }

        public string Format(string tag, string? format = null)
        {
            format ??= "_{0}_{1}_{2}_{3}_{4}";
            return string.Format(format, tag, MatchA.Tag, MatchB.Tag, MatchC.Tag, MatchD.Tag);
        }

        public string FormatSuffix(string? format = null)
        {
            format ??= "_{0}_{1}_{2}_{3}";
            return string.Format(format, MatchA.Tag, MatchB.Tag, MatchC.Tag, MatchD.Tag);
        }

        public int LinearIndex
        {
            get
            {
                return LinearIndexOf(MatchA, MatchB, MatchC, MatchD);
            }
        }

        public static int LinearIndexOf(ITileTagEntrySelection a,
                                        ITileTagEntrySelection b,
                                        ITileTagEntrySelection c,
                                        ITileTagEntrySelection d)
        {
            int card = 1;
            int index = 0;

            index += a.Index;
            card *= Math.Max(1, a.Cardinality);

            index += b.Index * card;
            card *= Math.Max(1, b.Cardinality);

            index += c.Index * card;
            card *= Math.Max(1, c.Cardinality);

            index += d.Index * card;
            return index;
        }

        public override string ToString()
        {
            return $"{nameof(MatchA)}: {MatchA}, {nameof(MatchB)}: {MatchB}, {nameof(MatchC)}: {MatchC}, {nameof(MatchD)}: {MatchD}, {nameof(LinearIndex)}: {LinearIndex}";
        }
    }
}