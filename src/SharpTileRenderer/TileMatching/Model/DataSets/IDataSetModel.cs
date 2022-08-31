using SharpTileRenderer.Util;
using System.ComponentModel;

namespace SharpTileRenderer.TileMatching.Model.DataSets
{
    public interface IDataSetModel: INotifyPropertyChanged
    {
        public string? Id { get; set; }
        public DataSetType Kind { get; }
        public ObservableDictionary<string, string> Properties { get; }
    }
}