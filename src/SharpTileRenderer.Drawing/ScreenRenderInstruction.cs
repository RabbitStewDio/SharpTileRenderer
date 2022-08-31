using SharpTileRenderer.Drawing.ViewPorts;
using System;

namespace SharpTileRenderer.Drawing
{
    public readonly struct ScreenRenderInstruction<TEntity> : IEquatable<ScreenRenderInstruction<TEntity>>
    {
        public readonly RenderInstruction<TEntity> RenderInstruction;
        public readonly ScreenPosition RenderPosition;
        public readonly int RenderOrder;

        public ScreenRenderInstruction(RenderInstruction<TEntity> renderInstruction, 
                                       ScreenPosition renderPosition,
                                       int renderOrder)
        {
            RenderInstruction = renderInstruction;
            RenderPosition = renderPosition;
            RenderOrder = renderOrder;
        }

        public bool Equals(ScreenRenderInstruction<TEntity> other)
        {
            return RenderInstruction.Equals(other.RenderInstruction) && RenderPosition.Equals(other.RenderPosition) && RenderOrder == other.RenderOrder;
        }

        public override bool Equals(object? obj)
        {
            return obj is ScreenRenderInstruction<TEntity> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = RenderInstruction.GetHashCode();
                hashCode = (hashCode * 397) ^ RenderPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ RenderOrder;
                return hashCode;
            }
        }

        public static bool operator ==(ScreenRenderInstruction<TEntity> left, ScreenRenderInstruction<TEntity> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ScreenRenderInstruction<TEntity> left, ScreenRenderInstruction<TEntity> right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"ScreenRenderInstruction({nameof(RenderInstruction)}: {RenderInstruction}, {nameof(RenderPosition)}: {RenderPosition}, {nameof(RenderOrder)}: {RenderOrder})";
        }
    }
}