using SharpTileRenderer.TileMatching.Model;
using System.Collections.ObjectModel;

namespace SharpTileRenderer.TexturePack.Model
{
    /// <summary>
    ///   A grid of tiles defined for a given texture.
    /// </summary>
    public class SpriteSheetTileGrid : ModelBase
    {
        string? name;
        int cellWidth;
        int cellHeight;
        int offsetX;
        int offsetY;
        int anchorX;
        int anchorY;
        int cellPaddingX;
        int cellPaddingY;

        public SpriteSheetTileGrid()
        {
            Tiles = new ObservableCollection<SpriteSheetTileDefinition>();
            RegisterObservableList(nameof(Tiles), Tiles);
        }

        public SpriteSheetTileGrid(int cellWidth,
                                   int cellHeight,
                                   int offsetX,
                                   int offsetY,
                                   int anchorX,
                                   int anchorY,
                                   params SpriteSheetTileDefinition[] tiles) :
            this(cellWidth, cellHeight, offsetX, offsetY, anchorX, anchorY, 0, 0, tiles)
        {
        }

        public SpriteSheetTileGrid(int cellWidth,
                                   int cellHeight,
                                   int offsetX,
                                   int offsetY,
                                   int anchorX,
                                   int anchorY,
                                   int cellPaddingX,
                                   int cellPaddingY,
                                   params SpriteSheetTileDefinition[] tiles)
        {
            Tiles = new ObservableCollection<SpriteSheetTileDefinition>(tiles);
            RegisterObservableList(nameof(Tiles), Tiles);
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            OffsetX = offsetX;
            OffsetY = offsetY;
            AnchorX = anchorX;
            AnchorY = anchorY;
            CellPaddingX = cellPaddingX;
            CellPaddingY = cellPaddingY;
        }

        public string? Name
        {
            get { return name; }
            set
            {
                if (value == name) return;
                name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SpriteSheetTileDefinition> Tiles { get; }

        public int CellWidth
        {
            get
            {
                return cellWidth;
            }
            set
            {
                if (value == cellWidth) return;
                cellWidth = value;
                OnPropertyChanged();
            }
        }

        public int CellHeight
        {
            get
            {
                return cellHeight;
            }
            set
            {
                if (value == cellHeight) return;
                cellHeight = value;
                OnPropertyChanged();
            }
        }

        public int OffsetX
        {
            get
            {
                return offsetX;
            }
            set
            {
                if (value == offsetX) return;
                offsetX = value;
                OnPropertyChanged();
            }
        }

        public int OffsetY
        {
            get
            {
                return offsetY;
            }
            set
            {
                if (value == offsetY) return;
                offsetY = value;
                OnPropertyChanged();
            }
        }

        public int AnchorX
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

        public int AnchorY
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

        public int CellPaddingX
        {
            get
            {
                return cellPaddingX;
            }
            set
            {
                if (value == cellPaddingX) return;
                cellPaddingX = value;
                OnPropertyChanged();
            }
        }

        public int CellPaddingY
        {
            get
            {
                return cellPaddingY;
            }
            set
            {
                if (value == cellPaddingY) return;
                cellPaddingY = value;
                OnPropertyChanged();
            }
        }
    }
}