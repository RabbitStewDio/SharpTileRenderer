using System;
using System.IO;
using System.Text;

namespace SharpTileRenderer.TexturePack
{
    public interface IContentLoader
    {
        TextReader LoadText(ContentUri name);
    }
    
    public interface IContentLoader<out TRawTexture>: IContentLoader
    {
        TRawTexture LoadTexture(ContentUri name);
    }

    /// <summary>
    ///    A sane Uri class. For some unexplicable reason, the System.Uri class does not
    ///    have a sane API for working with relative URIs or for using URIs as resource
    ///    identifiers in contexts other than the narrow use-cases predefined by Microsoft.
    ///
    ///    This struct here is a saner, toned down version for loading content from various
    ///    sources whilst allowing to work with path information, even on relative URIs.
    /// </summary>
    public readonly struct ContentUri : IEquatable<ContentUri>
    {
        readonly string? scheme;
        readonly string? path;
        readonly string? host;
        readonly string? fragment;
        readonly string? query;
        readonly int port;

        public ContentUri(string? scheme, string? path, string? host = null, string? fragment = null, string? query = null, int port = -1)
        {
            this.scheme = scheme;
            this.path = path;
            this.host = host;
            this.fragment = fragment;
            this.query = query;
            this.port = port;
        }

        public string? Scheme => scheme;

        public string? Path => path;

        public string AbsolutePath
        {
            get
            {
                if (scheme == null) throw new ArgumentException();
                return path ?? ".";
            }
        }

        public string? Host => host;

        public string? Fragment => fragment;

        public string? Query => query;

        public int Port => port;

        public static ContentUri Make(string schema, string path)
        {
            return new ContentUri(schema, path, null, null, null, -1);
        }

        public ContentUri Combine(ContentUri path)
        {
            if (path.Scheme == Scheme || path.Scheme == null)
            {
                // combine elements as relative 
                return new ContentUri(path.scheme ?? scheme,
                                      CombinePath(path.path),
                                      path.host ?? host,
                                      path.query ?? query,
                                      path.fragment ?? fragment,
                                      path.port == -1 ? port : path.port);
            }

            return path;
        }
        
        public ContentUri Combine(string? path)
        {
            if (path == null) return this;
            
            var other = Parse(path);
            return Combine(other);
        }

        string? CombinePath(string? path)
        {
            if (path == null) return this.path;
            
            var p = this.path != null ? System.IO.Path.Combine(this.path, path) : path;
            if (string.IsNullOrEmpty(p))
            {
                return this.path;
            }
            
            var currentDir = System.IO.Path.GetFullPath(".");
            var absPath = System.IO.Path.GetFullPath(p);
            if (currentDir == absPath)
            {
                return ".";
            }
            
            var combinePath = System.IO.Path.GetRelativePath(".", p);
            return combinePath;
        }

        public override bool Equals(object? obj)
        {
            return obj is ContentUri other && Equals(other);
        }

        public bool Equals(ContentUri other)
        {
            return scheme == other.scheme && path == other.path && host == other.host && fragment == other.fragment && query == other.query;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (scheme != null ? scheme.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (path != null ? path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (host != null ? host.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (fragment != null ? fragment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (query != null ? query.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ContentUri left, ContentUri right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ContentUri left, ContentUri right)
        {
            return !left.Equals(right);
        }

        public Uri ToUri()
        {
            var ub = new UriBuilder();
            ub.Scheme = scheme;
            ub.Host = host;
            ub.Port = port;
            ub.Path = path;
            ub.Fragment = fragment;
            ub.Query = query;
            return ub.Uri;
        }

        public static ContentUri MakeRelative(string path)
        {
            return new ContentUri(null, path, null, null, null, -1);
        }

        public static ContentUri Parse(string text)
        {
            if (text.StartsWith("/"))
            {
                return MakeRelative(text);
            }

            var idx = text.IndexOf("://", StringComparison.Ordinal);
            if (idx == -1)
            {
                return MakeRelative(text);
            }

            // not a relative URL, so it should be safe to use the built in parsers 
            var ub = new UriBuilder(text);
            return new ContentUri(ub.Scheme, ub.Path, ub.Host, ub.Fragment, ub.Query, ub.Port);

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(scheme))
            {
                sb.Append(scheme);
                sb.Append("://");
                sb.Append(host);
                if (port != -1)
                {
                    sb.Append(":");
                    sb.Append(port);
                }

                sb.Append("/");
            }

            sb.Append(this.path);
            if (!string.IsNullOrEmpty(query))
            {
                sb.Append("?");
                sb.Append(query);
            }
            if (!string.IsNullOrEmpty(fragment))
            {
                sb.Append("#");
                sb.Append(fragment);
            }

            return sb.ToString();
        }
    }
}
