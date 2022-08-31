using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.DataSets
{
    public class TagDataSetModel : DataSetModelBase, IDataSetModel
    {
        [DataMember]
        public DataSetType Kind => DataSetType.TagMap;
    }
}