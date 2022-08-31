using SharpTileRenderer.TileMatching.Model.DataSets;

namespace SharpTileRenderer.TileMatching.DataSets
{
    public interface ITileDataSetMetaData
    {
        DataSetType DataSetType { get; }
        bool IsThreadSafe { get; }
    }
}