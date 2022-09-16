using SharpTileRenderer.Util;
using System;
using System.Text.RegularExpressions;

namespace SharpTileRenderer.TileMatching
{
    public readonly struct SpriteTag : IEquatable<SpriteTag>
    {
        readonly string? prefix;
        readonly string? id;
        readonly string? qualifier;

        SpriteTag(string? prefix, string? id, string? qualifier)
        {
            this.prefix = prefix;
            this.id = id;
            this.qualifier = qualifier;
        }

        public static string? Normalize(string? prefix)
        {
            // ReSharper disable once ReplaceWithStringIsNullOrEmpty
            if (prefix == null || prefix.Length == 0) return null;
            return string.Intern(prefix);
        }

        public bool HasPrefix(string? prefix) => string.Equals(prefix, this.prefix, StringComparison.Ordinal);
        
        public SpriteTag WithQualifier(string? qualifier) => new SpriteTag(prefix, id, Normalize(qualifier));
        public SpriteTag WithPrefix(string? prefix) => new SpriteTag(Normalize(prefix), id, qualifier);

        public bool Equals(SpriteTag other)
        {
            return prefix == other.prefix && id == other.id && qualifier == other.qualifier;
        }

        public override bool Equals(object? obj)
        {
            return obj is SpriteTag other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (prefix != null ? prefix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (id != null ? id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (qualifier != null ? qualifier.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(SpriteTag left, SpriteTag right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SpriteTag left, SpriteTag right)
        {
            return !left.Equals(right);
        }

        public static SpriteTag Create(string? prefix, string? id, string? suffix) => new SpriteTag(Normalize(prefix), Normalize(id), Normalize(suffix));

        public SpriteTag With(GraphicTag t)
        {
            return new SpriteTag(prefix, t.Id, qualifier);
        }
        
        public static SpriteTag FromGraphicTag(GraphicTag t)
        {
            return new SpriteTag(null, t.Id, null);
        }

        public static Optional<SpriteTag> Parse(string raw, string prefixSeparator = ".", string suffixSeparator = "_")
        {
            var prefixEsc = Regex.Escape(prefixSeparator);
            var suffixEsc = Regex.Escape(suffixSeparator);
            var r = new Regex($"(?<prefix>.*{prefixEsc})?(?<tag>[^{suffixEsc}]+)(?<suffix>{suffixEsc}.+)?", RegexOptions.Singleline | RegexOptions.CultureInvariant);
            var mc = r.Match(raw);
            if (!mc.Success)
            {
                return default;
            }

            var prefix = mc.Groups[1].Success ? mc.Groups[1].Value : null;
            var body = mc.Groups[2].Success ? mc.Groups[2].Value : null;
            var suffix = mc.Groups[3].Success ? mc.Groups[3].Value : null;
            return new SpriteTag(prefix, body, suffix);
        }

        public string? Prefix
        {
            get { return prefix; }
        }

        public string? Id
        {
            get { return id; }
        }

        public string? Qualifier
        {
            get { return qualifier; }
        }

        public override string ToString()
        {
            return $"{prefix}{id}{qualifier}";
        }
    }
}