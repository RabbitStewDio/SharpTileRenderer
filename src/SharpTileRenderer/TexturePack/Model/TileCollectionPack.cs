using SharpTileRenderer.TileMatching.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpTileRenderer.TexturePack.Model
{
    /// <summary>
    ///   An aggregate of sprite sheet metadata. This collection does not yet constitute a valid
    ///   set of tiles. 
    /// </summary>
    public class TileCollectionPack : ModelBase, ITileCollection
    {
        IntDimension tileSize;
        TileShape tileShape;
        string? name;
        string? author;
        string? documentation;
        string? version;

        public TileCollectionPack()
        {
            TileSize = new IntDimension(32, 32);
            TileShape = TileShape.Grid;
            TileCollections = new ObservableCollection<SpriteSheetTileCollection>();
            RegisterObservableList(nameof(TileCollections), TileCollections);
            Name = name ?? $"TileCollection-{tileShape}-{tileSize}";
        }

        public TileCollectionPack(string? name,
                              IntDimension tileSize,
                              TileShape tileShape,
                              params SpriteSheetTileCollection[]? textureFiles): this()
        {
            TileSize = tileSize;
            TileShape = tileShape;
            if (textureFiles == null)
            {
                throw new ArgumentNullException(nameof(textureFiles));
            }

            Name = name ?? $"TileCollection-{tileShape}-{tileSize}";
            foreach (var t in textureFiles)
            {
                TileCollections.Add(t);
            }
        }

        public ObservableCollection<SpriteSheetTileCollection> TileCollections { get; }

        public string? Author
        {
            get { return author; }
            set
            {
                if (value == author) return;
                author = value;
                OnPropertyChanged();
            }
        }

        public string? Documentation
        {
            get { return documentation; }
            set
            {
                if (value == documentation) return;
                documentation = value;
                OnPropertyChanged();
            }
        }

        public string? Version
        {
            get { return version; }
            set
            {
                if (value == version) return;
                version = value;
                OnPropertyChanged();
            }
        }

        public string? Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value == name) return;
                name = value;
                OnPropertyChanged();
            }
        }

        public IntDimension TileSize
        {
            get
            {
                return tileSize;
            }
            set
            {
                if (value.Equals(tileSize)) return;
                tileSize = value;
                OnPropertyChanged();
            }
        }

        public TileShape TileShape
        {
            get
            {
                return tileShape;
            }
            set
            {
                if (value == tileShape) return;
                tileShape = value;
                OnPropertyChanged();
            }
        }

        public IReadOnlyList<TexturedTileSpec> ProduceTiles()
        {
            return TileCollections.SelectMany(f => f.ProduceTiles()).ToList();
        }
    }
}