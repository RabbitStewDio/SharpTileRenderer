using SharpTileRenderer.TexturePack.Grids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharpTileRenderer.TexturePack.Xml
{
    public class TexturePackLoader
    {
        class TexturePackLoaderContext<TRawTexture>
        {
            public IContentLoader<TRawTexture> ContentLoader { get; }
            public Uri BasePath { get; }
            public TextureType TextureType { get; }
            public int CellWidth { get; }
            public int CellHeight { get; }

            public TexturePackLoaderContext(IContentLoader<TRawTexture> contentLoader,
                                            Uri basePath,
                                            TextureType textureType,
                                            int cellWidth,
                                            int cellHeight)
            {
                ContentLoader = contentLoader;
                BasePath = basePath;
                TextureType = textureType;
                CellWidth = cellWidth;
                CellHeight = cellHeight;
            }

            public TexturePackLoaderContext<TRawTexture> Derive(int width, int height)
            {
                return new TexturePackLoaderContext<TRawTexture>(ContentLoader, BasePath, TextureType, width, height);
            }
        }


        public TexturePack Load<TRawTexture>(IContentLoader<TRawTexture> cl,
                                             Uri fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using var stream = cl.LoadText(fileName);
            var root = XDocument.Load(stream).Root;
            return Read(root, cl, fileName, new HashSet<Uri>());
        }

        TexturePack Read<TRawTexture>(XElement root, IContentLoader<TRawTexture> cl, Uri documentPath, HashSet<Uri> path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var width = (int?)root.AttributeLocal("width") ??
                        throw new TexturePackLoaderException("Texture pack requires width", root);
            var height = (int?)root.AttributeLocal("height") ??
                         throw new TexturePackLoaderException("Texture pack requires height", root);
            var textureType = ParseTextureType((string)root.AttributeLocal("type"));

            var name = root.AttributeLocal("name")?.Value ?? "unnamed";
            var basePath = new Uri(documentPath, "..");
            var collections = ReadContent(root, new TexturePackLoaderContext<TRawTexture>(cl, basePath, textureType, width, height), path);
            return new TexturePack(name, new IntDimension(width, height), textureType, collections.ToArray());
        }

        static TextureType ParseTextureType(string t, TextureType? defaultValue = null)
        {
            if (string.IsNullOrEmpty(t))
            {
                return defaultValue ?? throw new Exception("Texture type missing");
            }

            if (!Enum.TryParse(t, out TextureType result))
            {
                throw new TexturePackLoaderException("Texture type invalid.");
            }

            return result;
        }

        IEnumerable<ITileCollection> ReadInclude<TRawTexture>(XElement includeDirective,
                                                              TexturePackLoaderContext<TRawTexture> context,
                                                              HashSet<Uri> visitedPaths)
        {
            var file = (string)includeDirective.AttributeLocal("file");
            if (file == null)
            {
                return Enumerable.Empty<ITileCollection>();
            }

            var targetPath = new Uri(context.BasePath, file);
            if (visitedPaths.Contains(targetPath))
            {
                throw new TexturePackLoaderException(
                    $"Circular reference in include files or duplicate include while evaluating path {targetPath}",
                    includeDirective);
            }

            visitedPaths.Add(targetPath);
            using var stream = context.ContentLoader.LoadText(targetPath);
            var doc = XDocument.Load(stream);

            var root = doc.Root;
            var width = (int?)root.AttributeLocal("width") ?? context.CellWidth;
            var height = (int?)root.AttributeLocal("height") ?? context.CellHeight;
            var textureType = ParseTextureType((string)root.AttributeLocal("type"), context.TextureType);

            if (textureType != context.TextureType)
            {
                throw new TexturePackLoaderException(
                    $"Include file '{targetPath}' is using a '{textureType}' tile type.", includeDirective);
            }

            var loaderContext = context.Derive(width, height);
            return ReadContent(root, loaderContext, visitedPaths);
        }

        IEnumerable<ITileCollection> ReadContent<TRawTexture>(XElement root,
                                                              TexturePackLoaderContext<TRawTexture> context,
                                                              HashSet<Uri> path)
        {
            var includes =
                from e in root.Elements()
                where e.Name.LocalName == "include"
                where e.AttributeLocal("file") != null
                select ReadInclude(e, context, path);

            var c =
                from e2 in root.Elements()
                where e2.Name.LocalName == "collection"
                select ReadCollection(e2, context);

            var retval = new List<ITileCollection>();
            foreach (var include in includes)
            {
                retval.AddRange(include);
            }

            retval.AddRange(c);
            return retval;
        }

        ITileCollection ReadCollection<TRawTexture>(XElement c, TexturePackLoaderContext<TRawTexture> context)
        {
            var image = c.AttributeLocal("id");
            if (image == null)
            {
                throw new Exception();
            }

            var textureName = new Uri(context.BasePath, image.Value);

            var grids =
                from e in c.Elements()
                where e.Name.LocalName == "grid"
                select ParseGrid(e, context);

            return new GridTileCollection(textureName.AbsolutePath,
                                          grids.ToArray());
        }

        TileGrid ParseGrid<TRawTexture>(XElement grid, TexturePackLoaderContext<TRawTexture> context)
        {
            var halfCell = (bool?)grid.Attribute("half-cell-hint") ?? false;
            var defaultWidth = halfCell ? context.CellWidth / 2 : context.CellWidth;
            var defaultHeight = halfCell ? context.CellHeight / 2 : context.CellHeight;

            var x = (int)grid.AttributeLocal("x");
            var y = (int)grid.AttributeLocal("y");
            var width = (int?)grid.AttributeLocal("cell-width") ?? (int?)grid.AttributeLocal("width") ?? defaultWidth;
            var height = (int?)grid.AttributeLocal("cell-height") ??
                         (int?)grid.AttributeLocal("height") ?? defaultHeight;

            var anchorX = (int?)grid.AttributeLocal("anchor-x") ?? width / 2;
            var anchorY = (int?)grid.AttributeLocal("anchor-y") ?? height - defaultHeight / 2;

            var border = (int?)grid.AttributeLocal("cell-spacing") ?? (int?)grid.AttributeLocal("border") ?? 0;

            var tiles =
                from e in grid.Elements()
                where e.Name.LocalName == "tile"
                select ParseTile(e);

            return new TileGrid(width, height, x, y, anchorX, anchorY, border, border, tiles.ToArray());
        }

        static GridTileDefinition ParseTile(XElement tile)
        {
            var x = (int?)tile.AttributeLocal("x") ??
                    throw new TexturePackLoaderException("Mandatory attribute x is missing", tile);
            var y = (int?)tile.AttributeLocal("y") ??
                    throw new TexturePackLoaderException("Mandatory attribute y is missing", tile);
            var anchorX = (int?)tile.AttributeLocal("anchor-x");
            var anchorY = (int?)tile.AttributeLocal("anchor-y");
            var name = (string)tile.AttributeLocal("tag") ?? (string)tile.AttributeLocal("name");

            var tags =
                from e in tile.Elements()
                where e.Name.LocalName == "tag"
                select (string)e;

            var tagsAsList = tags.ToList();
            if (name != null && tagsAsList.Count == 0)
            {
                tagsAsList.Add(name);
            }

            if (tagsAsList.Count == 0)
            {
                throw new TexturePackLoaderException("Tiles must have at least one tag name");
            }

            return new GridTileDefinition(name, x, y, anchorX, anchorY, tagsAsList.ToArray());
        }
    }
}