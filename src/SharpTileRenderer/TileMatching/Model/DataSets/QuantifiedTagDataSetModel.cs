using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.DataSets
{
    public class QuantifiedTagDataSetModel : DataSetModelBase, IQuantifiedDataSetModel
    {
        [DataMember]
        public DataSetType Kind => DataSetType.QuantifiedTagMap;

        [DataMember]
        int defaultQuantity;

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