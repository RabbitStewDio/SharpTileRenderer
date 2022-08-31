using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.Selectors
{
    [DataContract]
    public class ListSelectorModel : ModelBase, ISelectorModel, IEquatable<ListSelectorModel>
    {
        [DataMember(Order = 0)]
        readonly string kind = BuiltInSelectors.List;

        [IgnoreDataMember]
        public string Kind => kind;

        [DataMember(Order = 1)]
        public ObservableCollection<ISelectorModel> Selectors { get; }

        public ListSelectorModel()
        {
            Selectors = new ObservableCollection<ISelectorModel>();
            RegisterObservableList(nameof(Selectors), Selectors);
        }
        
        public bool Equals(ISelectorModel other)
        {
            return other is ListSelectorModel m && Equals(m);
        }

        [IgnoreDataMember]
        public bool IsQuantifiedSelector
        {
            get
            {
                for (var index = 0; index < Selectors.Count; index++)
                {
                    var choiceDefinition = Selectors[index];
                    if (choiceDefinition?.IsQuantifiedSelector == true) return true;
                }

                return false;
            }
        }

        public bool Equals(ListSelectorModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Selectors.SequenceEqual(other.Selectors);
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

            return Equals((ListSelectorModel)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = Selectors.GetContentsHashCode();
            return hashCode;
        }

        public static bool operator ==(ListSelectorModel? left, ListSelectorModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ListSelectorModel? left, ListSelectorModel? right)
        {
            return !Equals(left, right);
        }
    }
}