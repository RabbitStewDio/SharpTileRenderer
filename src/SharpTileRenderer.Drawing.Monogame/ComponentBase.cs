using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.Drawing.Monogame
{
    public abstract class ComponentBase: IGameComponent, IUpdateable, IDrawable, INotifyPropertyChanged
    {
        bool visible;
        bool enabled;
        int drawOrder;
        int updateOrder;

        protected ComponentBase()
        {
            visible = true;
            enabled = true;
        }

        ~ComponentBase()
        {
            Dispose(false);
        }
        
        /// <summary>Shuts down the component.</summary>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>Shuts down the component.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object) this);
        }


        public virtual void Initialize()
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
            
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public int DrawOrder
        {
            get
            {
                return drawOrder;
            }
            set
            {
                if (value == drawOrder) return;
                drawOrder = value;
                OnPropertyChanged();
                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                if (value == visible) return;
                visible = value;
                OnPropertyChanged();
                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (value == enabled) return;
                enabled = value;
                OnPropertyChanged();
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int UpdateOrder
        {
            get
            {
                return updateOrder;
            }
            set
            {
                if (value == updateOrder) return;
                updateOrder = value;
                OnPropertyChanged();
                UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs>? DrawOrderChanged;
        public event EventHandler<EventArgs>? VisibleChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<EventArgs>? EnabledChanged;
        public event EventHandler<EventArgs>? UpdateOrderChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}