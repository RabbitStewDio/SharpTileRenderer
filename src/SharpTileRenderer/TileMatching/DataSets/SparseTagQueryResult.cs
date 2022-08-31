using SharpTileRenderer.Navigation;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.DataSets
{
    public readonly struct SparseTagQueryResult<TData, TEntity> : IEquatable<SparseTagQueryResult<TData, TEntity>>
    {
        public readonly TData TagData;
        public readonly TEntity Entity;
        public readonly ContinuousMapCoordinate Position;

        public SparseTagQueryResult(TData tagData, TEntity entity, ContinuousMapCoordinate position)
        {
            TagData = tagData;
            Entity = entity;
            Position = position;
        }

        public void Deconstruct(out TData tagData, out TEntity entity, out ContinuousMapCoordinate position)
        {
            tagData = TagData;
            entity = Entity;
            position = Position;
        }

        public bool Equals(SparseTagQueryResult<TData, TEntity> other)
        {
            return EqualityComparer<TData>.Default.Equals(TagData, other.TagData) && 
                   EqualityComparer<TEntity>.Default.Equals(Entity, other.Entity) && 
                   Position.Equals(other.Position);
        }

        public override bool Equals(object obj)
        {
            return obj is SparseTagQueryResult<TData, TEntity> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<TData>.Default.GetHashCode(TagData);
                hashCode = (hashCode * 397) ^ EqualityComparer<TEntity>.Default.GetHashCode(Entity);
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(SparseTagQueryResult<TData, TEntity> left, SparseTagQueryResult<TData, TEntity> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SparseTagQueryResult<TData, TEntity> left, SparseTagQueryResult<TData, TEntity> right)
        {
            return !left.Equals(right);
        }

        public SparseTagQueryResult<TData, TEntity2> ForEntity<TEntity2>(TEntity2 e)
        {
            return new SparseTagQueryResult<TData, TEntity2>(TagData, e, Position);
        }

        public override string ToString()
        {
            return $"{nameof(SparseTagQueryResult<TData, TEntity>)}({nameof(TagData)}: {TagData}, {nameof(Entity)}: {Entity}, {nameof(Position)}: {Position})";
        }
    }
}