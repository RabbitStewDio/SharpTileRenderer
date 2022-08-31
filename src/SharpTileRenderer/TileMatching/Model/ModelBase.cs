using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.TileMatching.Model
{
    public class ModelBase: INotifyPropertyChanged, IDisposable
    {
        readonly Lazy<Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler>> listHandlers;

        public ModelBase()
        {
            this.listHandlers = new Lazy<Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler>>();
        }

        ~ModelBase()
        {
            Dispose(false);
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RegisterObservableList(string propertyName, INotifyPropertyChanged np)
        {
            void OnLayerCollectionChanged(object? sender, PropertyChangedEventArgs e)
            {
                OnPropertyChanged(propertyName);
            }

            PropertyChangedEventHandler eventHandler = OnLayerCollectionChanged;
            listHandlers.Value[np] = eventHandler; 
            np.PropertyChanged += eventHandler;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (!listHandlers.IsValueCreated)
            {
                return;
            }

            foreach (var kvp in listHandlers.Value)
            {
                kvp.Key.PropertyChanged -= kvp.Value;
            }
            listHandlers.Value.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}