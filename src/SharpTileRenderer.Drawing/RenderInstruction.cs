using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing
{
    public readonly struct RenderInstruction<TEntity> : IEquatable<RenderInstruction<TEntity>>
    {
        public readonly TEntity Context;
        public readonly SpriteTag Tag;
        public readonly SpritePosition SpritePosition;
        public readonly ContinuousMapCoordinate MapPosition;

        public RenderInstruction(TEntity context,
                                 in SpriteTag tag,
                                 in SpritePosition spritePosition,
                                 in ContinuousMapCoordinate mapPosition)
        {
            this.Context = context;
            this.Tag = tag;
            this.SpritePosition = spritePosition;
            this.MapPosition = mapPosition;
        }

        public bool Equals(RenderInstruction<TEntity> other)
        {
            return EqualityComparer<TEntity>.Default.Equals(Context, other.Context) && 
                   Tag.Equals(other.Tag) && 
                   SpritePosition == other.SpritePosition && 
                   MapPosition.Equals(other.MapPosition);
        }

        public override bool Equals(object? obj)
        {
            return obj is RenderInstruction<TEntity> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<TEntity>.Default.GetHashCode(Context);
                hashCode = (hashCode * 397) ^ Tag.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)SpritePosition;
                hashCode = (hashCode * 397) ^ MapPosition.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(RenderInstruction<TEntity> left, RenderInstruction<TEntity> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RenderInstruction<TEntity> left, RenderInstruction<TEntity> right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{nameof(Context)}: {Context}, {nameof(Tag)}: {Tag}, {nameof(SpritePosition)}: {SpritePosition}, {nameof(MapPosition)}: {MapPosition}";
        }
    }
}