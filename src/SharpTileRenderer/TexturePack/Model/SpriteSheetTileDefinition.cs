using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using System;
using System.Collections.ObjectModel;

namespace SharpTileRenderer.TexturePack.Model
{
    public class SpriteSheetTileDefinition : ModelBase
    {
        int? anchorX;
        int? anchorY;
        string? name;
        int gridX;
        int gridY;

        public SpriteSheetTileDefinition()
        {
            Tags = new ObservableCollection<SpriteTag>();
            RegisterObservableList(nameof(Tags), Tags);
        }

        public SpriteSheetTileDefinition(int gridX,
                                         int gridY,
                                         params SpriteTag[]? tags) :
            this(FormatDefaultName(gridX, gridY, tags), gridX, gridY, null, null, tags)
        {
        }

        static string FormatDefaultName(int gridX,
                                        int gridY,
                                        params SpriteTag[]? tags)
        {
            return tags == null ? $"{gridX}-{gridY}" : $"{gridX}-{gridY}-{tags}";
        }

        public SpriteSheetTileDefinition(string? name,
                                         int gridX,
                                         int gridY,
                                         int? anchorX,
                                         int? anchorY,
                                         params SpriteTag[]? tags)
        {
            Name = name ?? FormatDefaultName(gridX, gridY, tags);
            GridX = gridX;
            GridY = gridY;
            AnchorX = anchorX;
            AnchorY = anchorY;
            Tags = new ObservableCollection<SpriteTag>(tags ?? Array.Empty<SpriteTag>());
            RegisterObservableList(nameof(Tags), Tags);
        }

        public SpriteSheetTileDefinition(SpriteTag tag) : this(tag.ToString(), 0, 0, null, null, tag)
        {
        }

        public ObservableCollection<SpriteTag> Tags { get; }

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

        public int GridX
        {
            get
            {
                return gridX;
            }
            set
            {
                if (value == gridX) return;
                gridX = value;
                OnPropertyChanged();
            }
        }

        public int GridY
        {
            get
            {
                return gridY;
            }
            set
            {
                if (value == gridY) return;
                gridY = value;
                OnPropertyChanged();
            }
        }

        public int? AnchorX
        {
            get
            {
                return anchorX;
            }
            set
            {
                if (value == anchorX) return;
                anchorX = value;
                OnPropertyChanged();
            }
        }

        public int? AnchorY
        {
            get
            {
                return anchorY;
            }
            set
            {
                if (value == anchorY) return;
                anchorY = value;
                OnPropertyChanged();
            }
        }
    }
}