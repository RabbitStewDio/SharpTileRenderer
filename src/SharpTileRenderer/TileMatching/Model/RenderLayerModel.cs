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
        
        [DataMember(Order=2)]
        RenderingSortOrder sortingOrder;

        [DataMember(Order = 3)]
        int? renderOrder;

        [DataMember(Order = 4)]
        public ObservableCollection<string> FeatureFlags { get; }

        [DataMember(Order = 5)]
        public ObservableDictionary<string, string> Properties { get; }

        [DataMember(Order = 10)]
        EntitySourceModel? entitySource;

        [DataMember(Order = 11)]
        ISelectorModel? match;

        [DataMember(Order = 20)]
        public ObservableCollection<RenderLayerModel> SubLayers { get; }

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
        public RenderingSortOrder SortingOrder
        {
            get
            {
                return sortingOrder;
            }
            set
            {
                if (value == sortingOrder) return;
                sortingOrder = value;
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
            SubLayers = new ObservableCollection<RenderLayerModel>();
            FeatureFlags = new ObservableCollection<string>();
            Properties = new ObservableDictionary<string, string>();
            this.RegisterObservableList(nameof(FeatureFlags), FeatureFlags);
            this.RegisterObservableList(nameof(Properties), Properties);
            this.RegisterObservableList(nameof(SubLayers), SubLayers);
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

            return Equals(entitySource, other.entitySource) && id == other.id && Equals(match, other.match) && enabled == other.enabled && sortingOrder == other.sortingOrder &&
                   SubLayers.SequenceEqual(other.SubLayers) &&
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
                hashCode = (hashCode * 397) ^ (renderOrder != null ? renderOrder.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ((int) sortingOrder);
                hashCode = (hashCode * 397) ^ (match != null ? match.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SubLayers.GetContentsHashCode());
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

        public bool IsQuantifiedLayer()
        {
            var quantified = false;
            if (Match != null) quantified = Match.IsQuantifiedSelector;
            foreach (var s in SubLayers)
            {
                quantified &= s.IsQuantifiedLayer();
            }

            return quantified;
        }
    }
}