using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.Selectors
{
    [DataContract]
    public class CellGroupSelectorModel : ModelBase, ISelectorModel, IEquatable<CellGroupSelectorModel>
    {
        [DataMember(Order = 0)]
        readonly string kind = BuiltInSelectors.CellGroup;

        [IgnoreDataMember]
        public string Kind => kind;

        [IgnoreDataMember]
        public bool IsQuantifiedSelector => false;


        [DataMember(Order = 1)]
        string? prefix;

        [DataMember(Order = 2)]
        string? contextDataSet;

        [DataMember(Order = 3)]
        public ObservableCollection<string> Matches { get; }

        [DataMember(Order = 4)]
        string? defaultClass;

        [DataMember(Order = 5)]
        CellGroupNavigationDirection direction;

        public CellGroupSelectorModel()
        {
            Matches = new ObservableCollection<string>();
            RegisterObservableList(nameof(Matches), Matches);
        }

        public string? ContextDataSet
        {
            get
            {
                return contextDataSet;
            }
            set
            {
                if (value == contextDataSet) return;
                contextDataSet = value;
                OnPropertyChanged();
            }
        }

        public string? Prefix
        {
            get
            {
                return prefix;
            }
            set
            {
                if (value == prefix) return;
                prefix = value;
                OnPropertyChanged();
            }
        }

        public string? DefaultClass
        {
            get { return defaultClass; }
            set
            {
                if (value == defaultClass) return;
                defaultClass = value;
                OnPropertyChanged();
            }
        }

        public CellGroupNavigationDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                if (value == direction) return;
                direction = value;
                OnPropertyChanged();
            }
        }

        public bool Equals(ISelectorModel other)
        {
            return other is CellGroupSelectorModel m && Equals(m);
        }

        public bool Equals(CellGroupSelectorModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return prefix == other.prefix && contextDataSet == other.contextDataSet && direction == other.direction && Matches.SequenceEqual(other.Matches);
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

            return Equals((CellGroupSelectorModel)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (prefix != null ? prefix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (contextDataSet != null ? contextDataSet.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)direction;
                hashCode = (hashCode * 397) ^ Matches.GetContentsHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(CellGroupSelectorModel? left, CellGroupSelectorModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CellGroupSelectorModel? left, CellGroupSelectorModel? right)
        {
            return !Equals(left, right);
        }
    }
}