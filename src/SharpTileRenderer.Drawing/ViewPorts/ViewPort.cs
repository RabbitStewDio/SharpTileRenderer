using JetBrains.Annotations;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    public sealed class ViewPort : IViewPort, IMapNavigation, INotifyPropertyChanged
    {
        readonly ScreenSpaceNavigator navigator;
        readonly IMapNavigator<GridDirection> mapNavigator;
        readonly IMapNavigator<GridDirection> screenNavigator;
        ScreenBounds pixelBounds;
        VirtualMapCoordinate focus;
        int zLayer;
        int rotation;

        public ViewPort(NavigatorMetaData navigatorConfig,
                        TileShape tileShape,
                        IntDimension tileSize)
        {
            this.TileSize = tileSize;
            this.TileShape = tileShape;

            this.GridType = AssertValidGridType(navigatorConfig.GridType, tileShape);

            this.navigator = new ScreenSpaceNavigator(GridType, tileSize);
            this.mapNavigator = navigatorConfig.BuildNavigator();
            this.screenNavigator = navigatorConfig.WithRotation(RotationForGridType(GridType)).BuildNavigator();
        }

        static int RotationForGridType(GridType gridType) => gridType == GridType.Grid ? 0 : 1;
        
        public IMapNavigation Navigation => this;

        public ScreenInsets PixelOverdraw { get; set; }

        public IMapNavigator<GridDirection> this[MapNavigationType type] =>
            type switch
            {
                MapNavigationType.Map => mapNavigator,
                MapNavigationType.Screen => screenNavigator,
                _ => throw new ArgumentException()
            };

        GridType AssertValidGridType(GridType t, TileShape s)
        {
            if (s == TileShape.Hex)
            {
                throw new ArgumentException("Hex tiles are not yet supported");
            }

            switch (t)
            {
                case GridType.Grid:
                case GridType.IsoDiamond:
                {
                    // Data is organised as a continuous 2D array
                    // Depending on the tile shape, this is either rendered 
                    // as a normal grid or as an isometric diamond shaped grid.
                    if (s == TileShape.Isometric)
                    {
                        return GridType.IsoDiamond;
                    }

                    return GridType.Grid;
                }
                case GridType.IsoStaggered:
                {
                    // iso-staggered game data can only be used with isometric tilesets. 
                    if (s != TileShape.Isometric)
                    {
                        throw new ArgumentException("An iso-staggered map backend always requires isometric tiles");
                    }

                    return GridType.IsoStaggered;
                }
                case GridType.Hex:
                case GridType.HexDiamond:
                    throw new ArgumentException("hex is not supported yet");
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }

        public GridType GridType { get; }

        public TileShape TileShape { get; }
        
        public int Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                if (value == rotation) return;
                rotation = value;
                OnPropertyChanged();
                Refresh();
            }
        }

        public ScreenBounds PixelBounds
        {
            get
            {
                return pixelBounds;
            }
            set
            {
                if (value.Equals(pixelBounds)) return;
                pixelBounds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TileBounds));
                Refresh();
            }
        }

        public TileBounds TileBounds
        {
            get
            {
                var activeArea = pixelBounds + PixelOverdraw;
                var tb = new TileBounds(activeArea.X / TileSize.Width, activeArea.Y / TileSize.Height, 
                                        activeArea.Width / TileSize.Width, activeArea.Height / TileSize.Height);
                return tb;
            }
        }

        public VirtualMapCoordinate Focus
        {
            get
            {
                return focus;
            }
            set
            {
                if (value.Equals(focus)) return;
                focus = value;
                OnPropertyChanged();
            }
        }

        public int ZLayer
        {
            get
            {
                return zLayer;
            }
            set
            {
                if (value == zLayer) return;
                zLayer = value;
                OnPropertyChanged();
                Refresh();
            }
        }

        public IntDimension TileSize { get; }

        public IScreenSpaceNavigator ScreenSpaceNavigator => navigator;

        void Refresh()
        {
            navigator.Refresh(this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}