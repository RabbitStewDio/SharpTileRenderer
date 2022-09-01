using SharpTileRenderer.Util;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.Meta
{
    [DataContract]
    public class GraphicTagDefinitionModel : ModelBase, IEquatable<GraphicTagDefinitionModel>
    {
        string? id;

        public GraphicTagDefinitionModel()
        {
            Properties = new ObservableDictionary<string, string>();
            Classes = new ObservableCollection<string>();
            Flags = new ObservableCollection<string>();
            RegisterObservableList(nameof(Properties), Properties);
            RegisterObservableList(nameof(Classes), Classes);
            RegisterObservableList(nameof(Flags), Flags);
        }

        [DataMember(Order = 1)]
        public ObservableCollection<string> Classes { get;  }

        [DataMember(Order = 2)]
        public ObservableDictionary<string, string> Properties { get; }

        [DataMember(Order = 3)]
        public ObservableCollection<string> Flags { get; }

        [DataMember(Order = 0)]
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

        public bool Equals(GraphicTagDefinitionModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return id == other.id && 
                   Classes.SequenceEqual(other.Classes) && 
                   Properties.SequenceEqual(other.Properties) && 
                   Flags.SequenceEqual(other.Flags);
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

            return Equals((GraphicTagDefinitionModel)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (id != null ? id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Classes.GetHashCode();
                hashCode = (hashCode * 397) ^ Properties.GetHashCode();
                hashCode = (hashCode * 397) ^ Flags.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(GraphicTagDefinitionModel? left, GraphicTagDefinitionModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GraphicTagDefinitionModel? left, GraphicTagDefinitionModel? right)
        {
            return !Equals(left, right);
        }
    }
}