using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model.DataSets
{
    [DataContract]
    public class ClassSetDataSetModel : ClassSetDataSetModelBase
    {
        [DataMember]
        public override DataSetType Kind => DataSetType.ClassSet;
    }
}