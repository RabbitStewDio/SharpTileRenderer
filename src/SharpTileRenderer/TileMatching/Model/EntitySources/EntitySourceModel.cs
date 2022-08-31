using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.EntitySources
{
    [DataContract]
    public class EntitySourceModel : ModelBase, IEquatable<EntitySourceModel>
    {
        [DataMember]
        string? entityQueryId;
        [DataMember]
        RenderingSortOrder sortingOrder;
        [DataMember]
        LayerQueryType layerQueryType;

        [IgnoreDataMember]
        public string? EntityQueryId
        {
            get
            {
                return entityQueryId;
            }
            set
            {
                if (value == entityQueryId) return;
                entityQueryId = value;
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
        public LayerQueryType LayerQueryType
        {
            get
            {
                return layerQueryType;
            }
            set
            {
                if (value == layerQueryType) return;
                layerQueryType = value;
                OnPropertyChanged();
            }
        }

        public bool Equals(EntitySourceModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return entityQueryId == other.entityQueryId && sortingOrder == other.sortingOrder && layerQueryType == other.layerQueryType;
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

            return Equals((EntitySourceModel)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (entityQueryId != null ? entityQueryId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)sortingOrder;
                hashCode = (hashCode * 397) ^ (int)layerQueryType;
                return hashCode;
            }
        }

        public static bool operator ==(EntitySourceModel? left, EntitySourceModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntitySourceModel? left, EntitySourceModel? right)
        {
            return !Equals(left, right);
        }
    }
}