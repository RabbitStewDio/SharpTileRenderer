using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpTileRenderer.TexturePack
{
    public class TexturePack : ITileCollection
    {
        public string Name { get; }
        public List<ITileCollection> TileCollections { get; }
        public IntDimension TileSize { get; }
        public TextureType TextureType { get; }
        readonly Lazy<List<TexturedTileSpec>> tiles;

        public TexturePack(string name,
                           IntDimension tileSize,
                           TextureType textureType,
                           params ITileCollection[] textureFiles)
        {
            TileSize = tileSize;
            TextureType = textureType;
            if (textureFiles == null)
            {
                throw new ArgumentNullException(nameof(textureFiles));
            }

            Name = name ?? throw new ArgumentNullException(nameof(name));
            TileCollections = new List<ITileCollection>(textureFiles);
            tiles = new Lazy<List<TexturedTileSpec>>(() => TileCollections.SelectMany(f => f.ProduceTiles()).ToList());
        }
        
        public IEnumerable<TexturedTileSpec> ProduceTiles()
        {
            return tiles.Value;
        }
    }
}
