using SharpTileRenderer.TileMatching.Model.DataSets;

namespace SharpTileRenderer.TileMatching.DataSets
{
    public class DefaultTileDataSetMetaData : ITileDataSetMetaData
    {
        public DefaultTileDataSetMetaData(DataSetType dataSetType, bool isThreadSafe)
        {
            DataSetType = dataSetType;
            IsThreadSafe = isThreadSafe;
        }

        public DataSetType DataSetType { get; }
        public bool IsThreadSafe { get; }
    }
}