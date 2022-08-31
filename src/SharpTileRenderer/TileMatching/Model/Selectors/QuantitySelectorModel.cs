using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.Selectors
{
    [DataContract]
    public class QuantitySelectorDefinition: ModelBase, IEquatable<QuantitySelectorDefinition>
    {
        int matchedQuantity;

        [DataMember(Order = 1)]
        public int MatchedQuantity
        {
            get
            {
                return matchedQuantity;
            }
            set
            {
                if (value == matchedQuantity) return;
                matchedQuantity = value;
                OnPropertyChanged();
            }
        }

        [DataMember(Order = 2)]
        public ISelectorModel? Selector { get; set; }

        public bool Equals(QuantitySelectorDefinition? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return MatchedQuantity.Equals(other.MatchedQuantity) && Equals(Selector, other.Selector);
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

            return Equals((ChoiceDefinition)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                return (MatchedQuantity * 397) ^ (Selector != null ? Selector.GetHashCode() : 0);
            }
        }

        public static bool operator ==(QuantitySelectorDefinition? left, QuantitySelectorDefinition? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(QuantitySelectorDefinition? left, QuantitySelectorDefinition? right)
        {
            return !Equals(left, right);
        }
    }

    [DataContract]
    public class QuantitySelectorModel : ModelBase, ISelectorModel, IEquatable<QuantitySelectorModel>
    {
        [DataMember(Order = 0)]
        public string Kind => BuiltInSelectors.QuantityChoice;
        
        [DataMember(Order = 1)]
        public ObservableCollection<QuantitySelectorDefinition> Choices { get; }

        [IgnoreDataMember]
        public bool IsQuantifiedSelector
        {
            get
            {
                return true;
            }
        }


        public QuantitySelectorModel()
        {
            Choices = new ObservableCollection<QuantitySelectorDefinition>();
            RegisterObservableList(nameof(Choices), Choices);
        }

        public bool Equals(ISelectorModel other)
        {
            return other is QuantitySelectorModel m && Equals(m);
        }

        public bool Equals(QuantitySelectorModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Choices.SequenceEqual(other.Choices);
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

            return Equals((QuantitySelectorModel)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            for (var i = 0; i < Choices.Count; i++)
            {
                hashCode = hashCode * 397 + Choices[i].GetHashCode();
            }
            return hashCode;
        }

        public static bool operator ==(QuantitySelectorModel? left, QuantitySelectorModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(QuantitySelectorModel? left, QuantitySelectorModel? right)
        {
            return !Equals(left, right);
        }
    }
}