using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.Selectors
{
    [DataContract]
    public class ChoiceDefinition: ModelBase, IEquatable<ChoiceDefinition>
    {
        [DataMember(Order = 1)]
        public ObservableCollection<string> MatchedTags { get; }
        
        [DataMember(Order = 2)]
        public ISelectorModel? Selector { get; set; }

        public ChoiceDefinition()
        {
            MatchedTags = new ObservableCollection<string>();
            RegisterObservableList(nameof(MatchedTags), MatchedTags);
        }

        public bool Equals(ChoiceDefinition? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return MatchedTags.SequenceEqual(other.MatchedTags) && Equals(Selector, other.Selector);
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
                return (MatchedTags.GetContentsHashCode() * 397) ^ (Selector != null ? Selector.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ChoiceDefinition? left, ChoiceDefinition? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ChoiceDefinition? left, ChoiceDefinition? right)
        {
            return !Equals(left, right);
        }
    }
    
    [DataContract]
    public class ChoiceSelectorModel : ModelBase, ISelectorModel, IEquatable<ChoiceSelectorModel>
    {
        [DataMember(Order = 0)]
        readonly string kind = BuiltInSelectors.Choice;

        [IgnoreDataMember]
        public string Kind => kind;
        
        [DataMember(Order = 1)]
        public ObservableCollection<ChoiceDefinition> Choices { get; }

        [IgnoreDataMember]
        public bool IsQuantifiedSelector
        {
            get
            {
                for (var index = 0; index < Choices.Count; index++)
                {
                    var choiceDefinition = Choices[index];
                    if (choiceDefinition.Selector?.IsQuantifiedSelector == true) return true;
                }

                return false;
            }
        }


        public ChoiceSelectorModel()
        {
            Choices = new ObservableCollection<ChoiceDefinition>();
            RegisterObservableList(nameof(Choices), Choices);
        }

        public bool Equals(ISelectorModel other)
        {
            return other is ChoiceSelectorModel m && Equals(m);
        }

        public bool Equals(ChoiceSelectorModel? other)
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

            return Equals((ChoiceSelectorModel)obj);
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

        public static bool operator ==(ChoiceSelectorModel? left, ChoiceSelectorModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ChoiceSelectorModel? left, ChoiceSelectorModel? right)
        {
            return !Equals(left, right);
        }
    }
}