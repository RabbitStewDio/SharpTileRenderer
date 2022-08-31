using System;

namespace SharpTileRenderer.TileMatching
{
    public readonly struct EntityClassification16 : IEquatable<EntityClassification16>, IEntityClassification<EntityClassification16>
    {
        readonly ushort flags;
        public int Cardinality => 16;

        public bool IsEmpty => default;

        public EntityClassification16(ushort flags)
        {
            this.flags = flags;
        }

        public bool Equals(EntityClassification16 other)
        {
            return flags == other.flags;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityClassification16 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return flags.GetHashCode();
        }

        public static bool operator ==(EntityClassification16 left, EntityClassification16 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityClassification16 left, EntityClassification16 right)
        {
            return !left.Equals(right);
        }

        public EntityClassification16 Merge(EntityClassification16 other)
        {
            var data = flags | other.flags;
            return new EntityClassification16((ushort) data);
        }

        public EntityClassification16 Matching(EntityClassification16 other)
        {
            var data = flags & other.flags;
            return new EntityClassification16((ushort) data);
        }

        public bool MatchesAny(EntityClassification16 other)
        {
            var data = flags & other.flags;
            return data != 0;
        }
        
        public EntityClassification16 Create(int cardinalPosition)
        {
            return new EntityClassification16((ushort)(1 << cardinalPosition));
        }

        public override string ToString()
        {
            return $"{nameof(flags)}: {Convert.ToString(flags, 2).PadLeft(Cardinality, '0')}";
        }
    }
}