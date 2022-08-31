using SharpTileRenderer.Navigation;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors
{
    public static class SpriteMatcherInput
    {
        public static SpriteMatcherInput<TData> From<TData>(TData data, ContinuousMapCoordinate position)
        {
            return new SpriteMatcherInput<TData>(data, position);
        }
    }
    
    public readonly struct SpriteMatcherInput<TData> : IEquatable<SpriteMatcherInput<TData>>
    {
        public readonly TData TagData;
        public readonly ContinuousMapCoordinate Position;

        public SpriteMatcherInput(TData tagData, ContinuousMapCoordinate position)
        {
            TagData = tagData;
            Position = position;
        }

        public void Deconstruct(out TData tagData, out ContinuousMapCoordinate position)
        {
            tagData = TagData;
            position = Position;
        }

        public bool Equals(SpriteMatcherInput<TData> other)
        {
            return EqualityComparer<TData>.Default.Equals(TagData, other.TagData) && Position.Equals(other.Position);
        }

        public override bool Equals(object? obj)
        {
            return obj is SpriteMatcherInput<TData> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TData>.Default.GetHashCode(TagData) * 397) ^ Position.GetHashCode();
            }
        }

        public static bool operator ==(SpriteMatcherInput<TData> left, SpriteMatcherInput<TData> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SpriteMatcherInput<TData> left, SpriteMatcherInput<TData> right)
        {
            return !left.Equals(right);
        }
    }
}