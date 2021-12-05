using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TexturePack
{
    public interface ITileCollection
    {
        IEnumerable<TexturedTileSpec> ProduceTiles();
    }

    public readonly struct TexturedTileSpec : IEquatable<TexturedTileSpec>
    {
        public string TextureAssetName { get; }
        public IntRect Bounds { get; }
        public IntPoint Anchor { get; }
        public ReadOnlyListWrapper<string> Tags { get; }

        public TexturedTileSpec(string textureAssetName, IntRect bounds, IntPoint anchor, ReadOnlyListWrapper<string> tags)
        {
            TextureAssetName = textureAssetName;
            Bounds = bounds;
            Anchor = anchor;
            Tags = tags;
        }

        public bool Equals(TexturedTileSpec other)
        {
            return TextureAssetName == other.TextureAssetName && Bounds.Equals(other.Bounds) && Anchor.Equals(other.Anchor) && Tags.Equals(other.Tags);
        }

        public override bool Equals(object obj)
        {
            return obj is TexturedTileSpec other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (TextureAssetName != null ? TextureAssetName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Bounds.GetHashCode();
                hashCode = (hashCode * 397) ^ Anchor.GetHashCode();
                hashCode = (hashCode * 397) ^ Tags.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(TexturedTileSpec left, TexturedTileSpec right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TexturedTileSpec left, TexturedTileSpec right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{nameof(TexturedTileSpec)}({nameof(TextureAssetName)}: {TextureAssetName}, {nameof(Bounds)}: {Bounds}, {nameof(Anchor)}: {Anchor}, {nameof(Tags)}: {Tags})";
        }
    }
}

