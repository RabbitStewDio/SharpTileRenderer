using System;

namespace SharpTileRenderer.TileMatching
{
    public readonly struct GraphicTag : IEquatable<GraphicTag>
    {
        public static readonly GraphicTag Empty = default;

        public readonly string? Id;

        public GraphicTag(string? id)
        {
            this.Id = Normalize(id);
        }

        public static string? Normalize(string? prefix)
        {
            return string.IsNullOrEmpty(prefix) ? null : string.Intern(prefix);
        }

        public bool Equals(GraphicTag other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is GraphicTag other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = (Id != null ? Id.GetHashCode() : 0);
            return hashCode;
        }

        public static bool operator ==(GraphicTag left, GraphicTag right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GraphicTag left, GraphicTag right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return Id ?? "";
        }

        public SpriteTag AsSpriteTag() => SpriteTag.FromGraphicTag(this);

        public static GraphicTag From(string s) => new GraphicTag(s);
    }
}