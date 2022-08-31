using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model
{
    [DataContract]
    public class RenderLayerModel : ModelBase, IEquatable<RenderLayerModel>
    {
        [DataMember(Order = 0)]
        string? id;

        [DataMember(Order = 1)]
        bool enabled;
        
        [DataMember(Order = 2)]
        int? renderOrder;

        [DataMember(Order = 3)]
        EntitySourceModel? entitySource;

        [DataMember(Order = 4)]
        ISelectorModel? match;

        [DataMember(Order = 5)]
        public ObservableCollection<string> FeatureFlags { get; }

        [DataMember(Order = 6)]
        public ObservableDictionary<string, string> Properties { get; }

        [IgnoreDataMember]
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

        [IgnoreDataMember]
        public int? RenderOrder
        {
            get
            {
                return renderOrder;
            }
            set
            {
                if (value == renderOrder) return;
                renderOrder = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        public ISelectorModel? Match
        {
            get
            {
                return match;
            }
            set
            {
                if (Equals(value, match)) return;
                match = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        public EntitySourceModel? EntitySource
        {
            get
            {
                return entitySource;
            }
            set
            {
                if (Equals(value, entitySource)) return;
                entitySource = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value == enabled) return;
                enabled = value;
                OnPropertyChanged();
            }
        }

        public RenderLayerModel()
        {
            Enabled = true;
            FeatureFlags = new ObservableCollection<string>();
            Properties = new ObservableDictionary<string, string>();
            this.RegisterObservableList(nameof(FeatureFlags), FeatureFlags);
            this.RegisterObservableList(nameof(Properties), Properties);
        }

        public bool Equals(RenderLayerModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(entitySource, other.entitySource) && id == other.id && Equals(match, other.match) && enabled == other.enabled &&
                   Properties.DictionaryEqual(other.Properties) &&
                   FeatureFlags.SequenceEqual(other.FeatureFlags);
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

            return Equals((RenderLayerModel)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (entitySource != null ? entitySource.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (id != null ? id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (enabled ? 1 : 0);
                hashCode = (hashCode * 397) ^ (match != null ? match.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FeatureFlags.GetContentsHashCode());
                hashCode = (hashCode * 397) ^ (Properties.GetContentsHashCode());
                return hashCode;
            }
        }

        public static bool operator ==(RenderLayerModel? left, RenderLayerModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RenderLayerModel? left, RenderLayerModel? right)
        {
            return !Equals(left, right);
        }
    }
}