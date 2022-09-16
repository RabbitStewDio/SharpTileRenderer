using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileBlending.Matcher
{
    [DataContract]
    public class BlendingSelectorModel : ModelBase, ISelectorModel, IEquatable<BlendingSelectorModel>
    {
        public const string SelectorName = "blend";

        [DataMember(Order = 0)]
        readonly string kind = SelectorName;
        [IgnoreDataMember]
        public string Kind => kind;

        [DataMember(Order = 1)]
        string? prefix;

        [DataMember(Order = 2)]
        string? sourcePrefix;

        [DataMember(Order = 3)]
        string? sourceSuffix;

        [DataMember(Order = 4)]
        string? contextDataSet;

        [IgnoreDataMember]
        public bool IsQuantifiedSelector => false;

        public BlendingSelectorModel()
        {
            MatchSelf = new ObservableCollection<string>();
            MatchWith = new ObservableCollection<string>();
            RegisterObservableList(nameof(MatchSelf), MatchSelf);
            RegisterObservableList(nameof(MatchWith), MatchWith);
        }

        [IgnoreDataMember]
        public IReadOnlyList<ISelectorModel> ChildSelectors => Array.Empty<ISelectorModel>();

        [IgnoreDataMember]
        public string? SourcePrefix
        {
            get { return sourcePrefix; }
            set
            {
                if (value == sourcePrefix) return;
                sourcePrefix = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        public string? SourceSuffix
        {
            get { return sourceSuffix; }
            set
            {
                if (value == sourceSuffix) return;
                sourceSuffix = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
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

        [IgnoreDataMember]
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

        [DataMember(Order = 5)]
        public ObservableCollection<string> MatchSelf { get; }

        [DataMember(Order = 6)]
        public ObservableCollection<string> MatchWith { get; }

        public bool Equals(ISelectorModel? other)
        {
            return other is BlendingSelectorModel m && Equals(m);
        }

        public bool Equals(BlendingSelectorModel? other)
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

            return Equals((BlendingSelectorModel)obj);
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

        public static bool operator ==(BlendingSelectorModel? left, BlendingSelectorModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BlendingSelectorModel? left, BlendingSelectorModel? right)
        {
            return !Equals(left, right);
        }
    }
}