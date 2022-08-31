using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.Util;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public interface IXmlTileMatcherModelReaderContext
    {
        Optional<IDataSetModel> ParseDataSets(XElement dataSet);
        Optional<ISelectorModel> ParseSelector(XElement me);
    }
}