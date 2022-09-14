using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.Drawing.Monogame
{
    public class RenderComponent : DrawableGameComponent, INotifyPropertyChanged
    {
        readonly ObservableCollection<ILayer> layers;
        ViewportRendering? renderData;
        Optional<ILayer[]> layersCached;
        ViewPort? viewPort;
        ScreenInsets renderBounds;

        public RenderComponent(Game game) : base(game)
        {
            this.layers = new ObservableCollection<ILayer>();
            this.layers.CollectionChanged += OnLayersChanged;
            this.layersCached = default;
            
        }

        public ScreenInsets RenderBounds
        {
            get
            {
                return renderBounds;
            }
            set
            {
                if (value.Equals(renderBounds)) return;
                renderBounds = value;
                OnPropertyChanged();
            }
        }

        public ViewPort? ViewPort
        {
            get
            {
                return viewPort;
            }
            set
            {
                if (Equals(value, viewPort)) return;
                
                viewPort = value;
                if (viewPort != null)
                {
                    renderData = new ViewportRendering(viewPort);
                }
                else
                {
                    renderData = null;
                }
                OnPropertyChanged();
            }
        }

        void OnLayersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.layersCached = default;
        }

        public ObservableCollection<ILayer> Layers
        {
            get { return layers; }
        }

        public void SetLayers(IReadOnlyList<ILayer> layers)
        {
            this.layers.Clear();
            for (var index = 0; index < layers.Count; index++)
            {
                var l = layers[index];
                this.layers.Add(l);
            }
        }

        public void SetLayers(params ILayer[] layers)
        {
            this.layers.Clear();
            for (var index = 0; index < layers.Length; index++)
            {
                var l = layers[index];
                this.layers.Add(l);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (viewPort == null) return;

            var vp = Game.GraphicsDevice.Viewport;
            viewPort.PixelBounds = new ScreenBounds(renderBounds.Left, renderBounds.Top, 
                                                    Math.Max(0, vp.Width - renderBounds.Left - renderBounds.Right), 
                                                    Math.Max(0, vp.Height - renderBounds.Top - renderBounds.Bottom));
            
            PerformRendering();
        }

        protected virtual void PerformRendering()
        {
            var localRenderData = this.renderData;
            if (localRenderData == null)
            {
                return;
            }
            
            if (!layersCached.TryGetValue(out var layerArray))
            {
                layerArray = this.layers.ToArray();
                layersCached = layerArray;
            }
            localRenderData.Render(layerArray);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}