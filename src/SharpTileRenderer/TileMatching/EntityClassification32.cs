using System;

namespace SharpTileRenderer.TileMatching
{
    public readonly struct EntityClassification32 : IEquatable<EntityClassification32>, IEntityClassification<EntityClassification32>
    {
        readonly uint flags;
        public int Cardinality => 32;
        public bool IsEmpty => default;

        public EntityClassification32(uint flags)
        {
            this.flags = flags;
        }

        public bool Equals(EntityClassification32 other)
        {
            return flags == other.flags;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityClassification32 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)flags;
        }

        public static bool operator ==(EntityClassification32 left, EntityClassification32 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityClassification32 left, EntityClassification32 right)
        {
            return !left.Equals(right);
        }

        public EntityClassification32 Merge(EntityClassification32 other)
        {
            var data = flags | other.flags;
            return new EntityClassification32(data);
        }

        public EntityClassification32 Matching(EntityClassification32 other)
        {
            var data = flags & other.flags;
            return new EntityClassification32(data);
        }

        public bool MatchesAny(EntityClassification32 other)
        {
            var data = flags & other.flags;
            return data != 0;
        }

        public EntityClassification32 Create(int cardinalPosition)
        {
            return new EntityClassification32((uint)(1 << cardinalPosition));
        }

        public override string ToString()
        {
            return $"{nameof(flags)}: {Convert.ToString(flags, 2).PadLeft(Cardinality, '0')}";
        }
    }
}