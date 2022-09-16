using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.Selectors
{
    [DataContract]
    public class RoadCornerSelectorModel : ModelBase, ISelectorModel, IEquatable<RoadCornerSelectorModel>
    {
        [DataMember(Order = 0)]
        readonly string kind = BuiltInSelectors.RoadCorner;

        [IgnoreDataMember]
        public string Kind => kind;

        [DataMember(Order = 1)]
        string? prefix;

        [DataMember(Order = 2)]
        string? contextDataSet;

        [IgnoreDataMember]
        public bool IsQuantifiedSelector => false;

        public RoadCornerSelectorModel()
        {
            MatchSelf = new ObservableCollection<string>();
            MatchWith = new ObservableCollection<string>();
            RegisterObservableList(nameof(MatchSelf), MatchSelf);
            RegisterObservableList(nameof(MatchWith), MatchWith);
        }

        [IgnoreDataMember]
        public IReadOnlyList<ISelectorModel> ChildSelectors => Array.Empty<ISelectorModel>();
        
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

        [DataMember(Order = 3)]
        public ObservableCollection<string> MatchSelf { get; }

        [DataMember(Order = 4)]
        public ObservableCollection<string> MatchWith { get; }

        public bool Equals(ISelectorModel other)
        {
            return other is RoadCornerSelectorModel m && Equals(m);
        }

        public bool Equals(RoadCornerSelectorModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return prefix == other.prefix &&
                   contextDataSet == other.contextDataSet &&
                   MatchSelf.SequenceEqual(other.MatchSelf) &&
                   MatchWith.SequenceEqual(other.MatchWith);
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

            return Equals((RoadCornerSelectorModel)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (prefix != null ? prefix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (contextDataSet != null ? contextDataSet.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MatchSelf.GetContentsHashCode();
                hashCode = (hashCode * 397) ^ MatchWith.GetContentsHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(RoadCornerSelectorModel? left, RoadCornerSelectorModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RoadCornerSelectorModel? left, RoadCornerSelectorModel? right)
        {
            return !Equals(left, right);
        }
    }
}