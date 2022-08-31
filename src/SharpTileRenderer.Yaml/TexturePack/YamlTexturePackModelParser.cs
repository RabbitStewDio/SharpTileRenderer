using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TexturePack.Model;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.Yaml.TexturePack.Model;
using SharpYaml;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SharpTileRenderer.Yaml.TexturePack
{
    public class YamlTexturePackModelParser
    {
        public TileCollectionPack Load(IContentLoader cl,
                                       ContentUri fileName)
        {
            if (cl == null)
            {
                throw new ArgumentNullException(nameof(cl));
            }

            using var reader = cl.LoadText(fileName);
            var model = Parse(reader);
            model.Name ??= fileName.AbsolutePath;
            var basePath = fileName.Combine("..");
            
            foreach (var inc in model.Includes)
            {
                var path = basePath.Combine(inc);
                using var r = cl.LoadText(path);
                var tx = ParseTexture(r);
                tx.Name ??= path.AbsolutePath;
                model.TextureFiles.Insert(0, tx);
            }

            Validate(model);
            return Convert(model);
        }

        void Validate(TexturePackModel model)
        {
            ValidateAnnotations(model);
            if (model.TileSize == null) throw new YamlException("TileSize must be set");
            foreach (var x in model.TextureFiles)
            {
                ValidateAnnotations(x);
                x.DefaultCellSize ??= model.TileSize;
                foreach (var grid in x.Grids)
                {
                    ValidateAnnotations(grid);
                    var cellSize = grid.CellSize ?? x.DefaultCellSize;
                    grid.GridOrigin ??= new PointModel();
                    grid.CellPadding ??= new DimensionModel();
                    grid.CellAnchor ??= new PointModel(cellSize.Width / 2, cellSize.Height / 2);

                    foreach (var tile in grid.Tiles)
                    {
                        ValidateAnnotations(tile);
                        if (tile.Position == null)
                        {
                            throw new YamlException("Tile position is mandatory");
                        }

                        if (tile.Aliases == null)
                        {
                            throw new YamlException("Tile must have at least one tag");
                        }

                        tile.Aliases = tile.Aliases.Where(e => !string.IsNullOrEmpty(e)).Distinct().ToList();
                        if (tile.Aliases.Count == 0)
                        {
                            throw new YamlException("Tile must have at least one tag");
                        }
                    }
                }
            }
        }

        static void ValidateAnnotations(object model)
        {
            var context = new ValidationContext(model, null, null);
            Validator.ValidateObject(model, context, true);
        }

        TexturePackModel Parse(TextReader r)
        {
            var des = new Serializer(YamlSerializerSettings.CreateDefaultSerializerSettings());
            return des.Deserialize<TexturePackModel>(r);
        }

        TextureFileModel ParseTexture(TextReader r)
        {
            var des = new Serializer(YamlSerializerSettings.CreateDefaultSerializerSettings());
            return des.Deserialize<TextureFileModel>(r);
        }

        TileCollectionPack Convert(TexturePackModel model)
        {
            Debug.Assert(model.Name != null, "model.Name != null");
            Debug.Assert(model.TileSize != null, "model.TileSize != null");

            var x = model.TextureFiles.Select(ConvertFileModel).ToArray();
            return new TileCollectionPack(model.Name, model.TileSize, model.TileShape, x);
        }

        SpriteSheetTileCollection ConvertFileModel(TextureFileModel model)
        {
            Debug.Assert(model.Name != null, "model.Name != null");
            Debug.Assert(model.DefaultCellSize != null, "model.DefaultCellSize != null");

            var grids = model.Grids.Select(g => ConvertGrid(model.DefaultCellSize, g)).ToArray();
            return new SpriteSheetTileCollection(model.Name, grids);
        }

        SpriteSheetTileGrid ConvertGrid(DimensionModel defaultCellSize, TileGridModel tgm)
        {
            Debug.Assert(tgm.GridOrigin != null, "tgm.GridOrigin != null");
            Debug.Assert(tgm.CellAnchor != null, "tgm.CellAnchor != null");
            Debug.Assert(tgm.CellPadding != null, "tgm.CellPadding != null");

            static SpriteSheetTileDefinition ConvertTile(TileDefinitionModel arg)
            {
                var aliasesRaw = arg.Aliases ?? new List<string>();
                var aliases = aliasesRaw.SelectMany(a => SpriteTag.Parse(a)).ToArray();
                return new SpriteSheetTileDefinition(arg.Tag, arg.Position?.X ?? 0, arg.Position?.Y ?? 0, arg.CellAnchor?.X ?? 0, arg.CellAnchor?.Y ?? 0, aliases);
            }

            var tiles = tgm.Tiles.Select(ConvertTile).ToArray();
            return new SpriteSheetTileGrid(tgm.CellSize?.Width ?? defaultCellSize.Width,
                                           tgm.CellSize?.Height ?? defaultCellSize.Height,
                                           tgm.GridOrigin.X, tgm.GridOrigin.Y,
                                           tgm.CellAnchor.X, tgm.CellAnchor.Y,
                                           tgm.CellPadding.Width, tgm.CellPadding.Height, tiles);
        }
    }
}