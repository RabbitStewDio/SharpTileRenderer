using SharpTileRenderer.TileMatching.Selectors;
using System;

namespace SharpTileRenderer.TileMatching
{
    public readonly struct EntityClassification64 : IEquatable<EntityClassification64>, IEntityClassification<EntityClassification64>
    {
        readonly ulong flags;
        public int Cardinality => 64;
        public bool IsEmpty => default;

        public EntityClassification64(ulong flags)
        {
            this.flags = flags;
        }

        public bool Equals(EntityClassification64 other)
        {
            return flags == other.flags;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityClassification64 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)flags;
        }

        public static bool operator ==(EntityClassification64 left, EntityClassification64 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityClassification64 left, EntityClassification64 right)
        {
            return !left.Equals(right);
        }

        public EntityClassification64 Merge(EntityClassification64 other)
        {
            var data = flags | other.flags;
            return new EntityClassification64(data);
        }

        public EntityClassification64 Matching(EntityClassification64 other)
        {
            var data = flags & other.flags;
            return new EntityClassification64(data);
        }

        public bool MatchesAny(EntityClassification64 other)
        {
            var data = flags & other.flags;
            return data != 0;
        }

        public EntityClassification64 Create(int cardinalPosition)
        {
            return new EntityClassification64((uint)(1 << cardinalPosition));
        }
        
        public override string ToString()
        {
            return $"{nameof(flags)}: {Convert.ToString((long)flags, 2).PadLeft(Cardinality, '0')}";
        }
    }
}