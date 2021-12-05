using SharpTileRenderer.TexturePack.Grids;
using SharpTileRenderer.TexturePack.Yaml.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace SharpTileRenderer.TexturePack.Yaml
{
    public class TexturePackYamlLoader
    {
        public TexturePack Load<TRawTexture>(IContentLoader<TRawTexture> cl,
                                             Uri fileName)
        {
            using var reader = cl.LoadText(fileName);
            var model = Parse(reader);
            model.Name ??= fileName.AbsolutePath;
            model.TextureFiles ??= new List<TextureFileModel>();
            model.Includes ??= new List<string>();

            foreach (var inc in model.Includes)
            {
                var path = new Uri(new Uri(fileName, "..") + inc);
                using var r = cl.LoadText(path);
                var tx = ParseTexture(r);
                tx.Name ??= path.AbsolutePath;
                tx.Grids ??= new List<TileGridModel>();
                model.TextureFiles.Insert(0, tx);
            }

            Validate(model);
            return Convert(model);
        }

        void Validate(TexturePackModel model)
        {
            if (model.TileSize == null) throw new YamlException("TileSize must be set");
            foreach (var x in model.TextureFiles)
            {
                x.DefaultCellSize ??= model.TileSize;
                foreach (var grid in x.Grids)
                {
                    var cellSize = grid.CellSize ?? x.DefaultCellSize;
                    grid.GridOrigin ??= new PointModel();
                    grid.CellPadding ??= new DimensionModel();
                    grid.CellAnchor ??= new PointModel(cellSize.Width / 2, cellSize.Height / 2);

                    foreach (var tile in grid.Tiles)
                    {
                        if (tile.Position == null)
                        {
                            throw new YamlException("Tile position is mandatory");
                        }

                        if (tile.Tags == null)
                        {
                            throw new YamlException("Tile must have at least one tag");
                        }

                        tile.Tags = tile.Tags.Where(e => !string.IsNullOrEmpty(e)).Distinct().ToList();
                        if (tile.Tags.Count == 0)
                        {
                            throw new YamlException("Tile must have at least one tag");
                        }
                    }
                }
            }
        }

        public class ValidatingNodeDeserializer : INodeDeserializer
        {
            readonly INodeDeserializer nodeDeserializer;

            public ValidatingNodeDeserializer(INodeDeserializer nodeDeserializer)
            {
                this.nodeDeserializer = nodeDeserializer;
            }

            public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
            {
                if (!nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
                {
                    return false;
                }

                if (value == null)
                {
                    return true;
                }

                var context = new ValidationContext(value, null, null);
                Validator.ValidateObject(value, context, true);
                return true;
            }
        }

        TexturePackModel Parse(TextReader r)
        {
            var deserializer = new DeserializerBuilder()
                               .WithNamingConvention(LowerCaseNamingConvention.Instance)
                               .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), s => s.InsteadOf<ObjectNodeDeserializer>())
                               .Build();
            return deserializer.Deserialize<TexturePackModel>(r);
        }

        TextureFileModel ParseTexture(TextReader r)
        {
            var deserializer = new DeserializerBuilder()
                               .WithNamingConvention(LowerCaseNamingConvention.Instance)
                               .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), s => s.InsteadOf<ObjectNodeDeserializer>())
                               .Build();
            return deserializer.Deserialize<TextureFileModel>(r);
        }

        TexturePack Convert(TexturePackModel model)
        {
            var x = model.TextureFiles.Select(ConvertFileModel).ToArray();
            return new TexturePack(model.Name, model.TileSize, model.TextureType, x);
        }

        ITileCollection ConvertFileModel(TextureFileModel model)
        {
            var grids = model.Grids.Select(g => ConvertGrid(model.DefaultCellSize, g)).ToArray();
            return new GridTileCollection(model.Name, grids);
        }

        TileGrid ConvertGrid(DimensionModel defaultCellSize, TileGridModel tgm)
        {
            var tiles = tgm.Tiles.Select(ConvertTile).ToArray();
            return new TileGrid(tgm.CellSize?.Width ?? defaultCellSize.Width,
                                tgm.CellSize?.Height ?? defaultCellSize.Height,
                                tgm.GridOrigin.X, tgm.GridOrigin.Y,
                                tgm.CellAnchor.X, tgm.CellAnchor.Y,
                                tgm.CellPadding.Width, tgm.CellPadding.Height, tiles);
        }

        GridTileDefinition ConvertTile(TileDefinitionModel arg)
        {
            return new GridTileDefinition(arg.Name, arg.Position.X, arg.Position.Y, arg.CellAnchor?.X, arg.CellAnchor?.Y, arg.Tags.ToArray());
        }
    }
}