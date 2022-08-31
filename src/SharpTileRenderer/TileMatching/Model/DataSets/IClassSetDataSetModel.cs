using System.Collections.ObjectModel;

namespace SharpTileRenderer.TileMatching.Model.DataSets
{
    public interface IClassSetDataSetModel : IDataSetModel
    {
        public ObservableCollection<string> Classes { get; }
        public string? DefaultClass { get; set; }
    }
}