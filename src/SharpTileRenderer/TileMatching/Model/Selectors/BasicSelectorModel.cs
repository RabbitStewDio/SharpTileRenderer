using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.Selectors
{
    [DataContract]
    public class BasicSelectorModel : ModelBase, ISelectorModel, IEquatable<BasicSelectorModel>
    {
        [DataMember(Order = 1)]
        string? prefix;

        [DataMember(Order = 2)]
        string? suffix;

        [DataMember(Order = 0)]
        readonly string kind = BuiltInSelectors.Basic;

        [IgnoreDataMember]
        public bool IsQuantifiedSelector => false;

        [IgnoreDataMember]
        public string Kind => kind;

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
        public string? Suffix
        {
            get
            {
                return suffix;
            }
            set
            {
                if (value == suffix) return;
                suffix = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        public IReadOnlyList<ISelectorModel> ChildSelectors => Array.Empty<ISelectorModel>();

        public bool Equals(ISelectorModel other)
        {
            return other is BasicSelectorModel m && Equals(m);
        }
        
        public bool Equals(BasicSelectorModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return prefix == other.prefix;
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

            return Equals((BasicSelectorModel)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return (prefix != null ? prefix.GetHashCode() : 0);
        }

        public static bool operator ==(BasicSelectorModel? left, BasicSelectorModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BasicSelectorModel? left, BasicSelectorModel? right)
        {
            return !Equals(left, right);
        }

    }
}