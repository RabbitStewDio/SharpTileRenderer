using JetBrains.Annotations;
using SharpTileRenderer.Util;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.DataSets
{
    public abstract class DataSetModelBase : IEquatable<DataSetModelBase>
    {
        string? id;

        public string? Id
        {
            get
            {
                return id;
            }
            set
            {
                if (value == id) return;
                id = value;
                OnPropertyChanged();
            }
        }

        protected DataSetModelBase()
        {
            Properties = new ObservableDictionary<string, string>();
            var np = Properties as INotifyPropertyChanged;
            np.PropertyChanged += OnDataSetChanged;
        }

        ~DataSetModelBase()
        {
            var np = Properties as INotifyPropertyChanged;
            np.PropertyChanged -= OnDataSetChanged;
        }

        void OnDataSetChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Properties));
        }

        [DataMember]
        public ObservableDictionary<string, string> Properties { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(DataSetModelBase? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return id == other.id && Properties.DictionaryEqual(other.Properties);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((DataSetModelBase)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((id != null ? id.GetHashCode() : 0) * 397) ^ Properties.GetContentsHashCode();
            }
        }

        public static bool operator ==(DataSetModelBase? left, DataSetModelBase? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataSetModelBase? left, DataSetModelBase? right)
        {
            return !Equals(left, right);
        }
    }

    public static class DataSetModelExtensions
    {
        public static TDataSetModel WithProperty<TDataSetModel>(this TDataSetModel m, string key, string value)
            where TDataSetModel: IDataSetModel
        {
            m.Properties[key] = value;
            return m;
        }
    }
}