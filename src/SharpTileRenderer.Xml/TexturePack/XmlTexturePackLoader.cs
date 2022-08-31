using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TexturePack.Model;
using SharpTileRenderer.TileMatching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TexturePack
{
    public class XmlTexturePackLoader
    {
        class TexturePackLoaderContext
        {
            public IContentLoader ContentLoader { get; }
            public ContentUri BasePath { get; }
            public TileShape TileShape { get; }
            public int CellWidth { get; }
            public int CellHeight { get; }

            public TexturePackLoaderContext(IContentLoader contentLoader,
                                            ContentUri basePath,
                                            TileShape tileShape,
                                            int cellWidth,
                                            int cellHeight)
            {
                ContentLoader = contentLoader;
                BasePath = basePath;
                TileShape = tileShape;
                CellWidth = cellWidth;
                CellHeight = cellHeight;
            }

            public TexturePackLoaderContext Derive(int width, int height)
            {
                return new TexturePackLoaderContext(ContentLoader, BasePath, TileShape, width, height);
            }
        }


        public TileCollectionPack Load(IContentLoader cl,
                                       ContentUri fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using var stream = cl.LoadText(fileName);
            var root = XDocument.Load(stream).Root;
            if (root == null)
            {
                throw new XmlTexturePackLoaderException($"Unable to parse file {fileName}");
            }

            return Read(root, cl, fileName, new HashSet<ContentUri>());
        }

        TileCollectionPack Read(XElement root, IContentLoader cl, ContentUri documentPath, HashSet<ContentUri> path)
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
                        throw new XmlTexturePackLoaderException("Texture pack requires width", root);
            var height = (int?)root.AttributeLocal("height") ??
                         throw new XmlTexturePackLoaderException("Texture pack requires height", root);
            var textureType = ParseTextureType((string?)root.AttributeLocal("type"));

            var name = root.AttributeLocal("name")?.Value ?? "unnamed";
            var basePath = documentPath.Combine("..");
            var collections = ReadContent(root, new TexturePackLoaderContext(cl, basePath, textureType, width, height), path);
            return new TileCollectionPack(name, new IntDimension(width, height), textureType, collections.ToArray());
        }

        static TileShape ParseTextureType(string? t, TileShape? defaultValue = null)
        {
            if (string.IsNullOrEmpty(t))
            {
                return defaultValue ?? throw new Exception("Texture type missing");
            }

            if (!Enum.TryParse(t, out TileShape result))
            {
                throw new XmlTexturePackLoaderException("Texture type invalid.");
            }

            return result;
        }

        IEnumerable<SpriteSheetTileCollection> ReadInclude(XElement includeDirective,
                                                           TexturePackLoaderContext context,
                                                           HashSet<ContentUri> visitedPaths)
        {
            var file = (string?)includeDirective.AttributeLocal("file");
            if (file == null)
            {
                return Enumerable.Empty<SpriteSheetTileCollection>();
            }

            var targetPath = context.BasePath.Combine(file);
            if (visitedPaths.Contains(targetPath))
            {
                throw new XmlTexturePackLoaderException(
                    $"Circular reference in include files or duplicate include while evaluating path {targetPath}",
                    includeDirective);
            }

            visitedPaths.Add(targetPath);
            using var stream = context.ContentLoader.LoadText(targetPath);
            var doc = XDocument.Load(stream);

            var root = doc.Root;
            if (root == null)
            {
                throw new XmlTexturePackLoaderException($"Unable to parse referenced file {targetPath}");
            }

            var width = (int?)root.AttributeLocal("width") ?? context.CellWidth;
            var height = (int?)root.AttributeLocal("height") ?? context.CellHeight;
            var textureType = ParseTextureType((string?)root.AttributeLocal("type"), context.TileShape);

            if (textureType != context.TileShape)
            {
                throw new XmlTexturePackLoaderException(
                    $"Include file '{targetPath}' is using a '{textureType}' tile type.", includeDirective);
            }

            var loaderContext = context.Derive(width, height);
            return ReadContent(root, loaderContext, visitedPaths);
        }

        IEnumerable<SpriteSheetTileCollection> ReadContent(XElement root,
                                                           TexturePackLoaderContext context,
                                                           HashSet<ContentUri> path)
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

            var retval = new List<SpriteSheetTileCollection>();
            foreach (var include in includes)
            {
                retval.AddRange(include);
            }

            retval.AddRange(c);
            return retval;
        }

        SpriteSheetTileCollection ReadCollection(XElement c, TexturePackLoaderContext context)
        {
            var image = c.AttributeLocal("id");
            if (image == null)
            {
                throw new Exception();
            }

            var textureName = context.BasePath.Combine(image.Value);

            var grids =
                from e in c.Elements()
                where e.Name.LocalName == "grid"
                select ParseGrid(e, context);

            return new SpriteSheetTileCollection(textureName.ToString(),
                                                 grids.ToArray());
        }

        SpriteSheetTileGrid ParseGrid(XElement grid, TexturePackLoaderContext context)
        {
            var halfCell = (bool?)grid.Attribute("half-cell-hint") ?? false;
            var defaultWidth = halfCell ? context.CellWidth / 2 : context.CellWidth;
            var defaultHeight = halfCell ? context.CellHeight / 2 : context.CellHeight;

            var x = (int?)grid.AttributeLocal("x") ?? throw new XmlTexturePackLoaderException();
            var y = (int?)grid.AttributeLocal("y") ?? throw new XmlTexturePackLoaderException();
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

            return new SpriteSheetTileGrid(width, height, x, y, anchorX, anchorY, border, border, tiles.ToArray());
        }

        static SpriteSheetTileDefinition ParseTile(XElement tile)
        {
            var x = (int?)tile.AttributeLocal("x") ??
                    throw new XmlTexturePackLoaderException("Mandatory attribute x is missing", tile);
            var y = (int?)tile.AttributeLocal("y") ??
                    throw new XmlTexturePackLoaderException("Mandatory attribute y is missing", tile);
            var anchorX = (int?)tile.AttributeLocal("anchor-x");
            var anchorY = (int?)tile.AttributeLocal("anchor-y");
            var name = (string?)tile.AttributeLocal("tag") ?? (string?)tile.AttributeLocal("name");

            var tags = tile.Elements().Where(e => e.Name.LocalName == "tag").SelectMany(e => SpriteTag.Parse((string)e));

            var tagsAsList = tags.ToList();
            if (name != null && tagsAsList.Count == 0)
            {
                if (SpriteTag.Parse(name).TryGetValue(out var tagFromName))
                {
                    tagsAsList.Add(tagFromName);
                }
                else
                {
                    throw new XmlTexturePackLoaderException("Tiles must have at least one tag name");
                }
            }

            if (tagsAsList.Count == 0)
            {
                throw new XmlTexturePackLoaderException("Tiles must have at least one tag name");
            }

            return new SpriteSheetTileDefinition(name, x, y, anchorX, anchorY, tagsAsList.ToArray());
        }
    }
}