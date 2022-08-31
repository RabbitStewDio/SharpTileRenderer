using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.DataSets
{
    [DataContract]
    public class QuantifiedClassSetDataSetModel : ClassSetDataSetModelBase, IQuantifiedDataSetModel
    {
        [DataMember]
        int defaultQuantity;

        [DataMember]
        public override DataSetType Kind => DataSetType.QuantifiedClassSet;

        public int DefaultQuantity
        {
            get
            {
                return defaultQuantity;
            }
            set
            {
                if (value == defaultQuantity) return;
                defaultQuantity = value;
                OnPropertyChanged();
            }
        }
    }
}