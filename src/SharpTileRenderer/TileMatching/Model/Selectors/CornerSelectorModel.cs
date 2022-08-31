using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.Selectors
{
    [DataContract]
    public class CornerSelectorModel : ModelBase, ISelectorModel, IEquatable<CornerSelectorModel>
    {
        [DataMember(Order = 2)]
        string? contextDataSet;

        [DataMember(Order = 1)]
        string? prefix;

        [DataMember(Order = 0)]
        readonly string kind = BuiltInSelectors.Corner;

        [IgnoreDataMember]
        public string Kind => kind;

        [IgnoreDataMember]
        public bool IsQuantifiedSelector => false;

        string? defaultClass;
        
        public CornerSelectorModel()
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

        [DataMember(Order = 3)]
        public ObservableCollection<string> Matches { get; }

        [DataMember(Order = 4)]
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

        public bool Equals(ISelectorModel other)
        {
            return other is CornerSelectorModel m && Equals(m);
        }

        public bool Equals(CornerSelectorModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return contextDataSet == other.contextDataSet && prefix == other.prefix && Matches.SequenceEqual(other.Matches);
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

            return Equals((CornerSelectorModel)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (contextDataSet != null ? contextDataSet.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (prefix != null ? prefix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Matches.GetContentsHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(CornerSelectorModel? left, CornerSelectorModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CornerSelectorModel? left, CornerSelectorModel? right)
        {
            return !Equals(left, right);
        }
    }
}