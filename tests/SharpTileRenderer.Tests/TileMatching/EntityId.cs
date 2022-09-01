using System;

namespace SharpTileRenderer.Tests.TileMatching
{
    public readonly struct EntityId : IEquatable<EntityId>
    {
        public readonly int Id;

        public EntityId(int id)
        {
            Id = id;
        }

        public bool Equals(EntityId other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityId left, EntityId right)
        {
            return !left.Equals(right);
        }
    }
}