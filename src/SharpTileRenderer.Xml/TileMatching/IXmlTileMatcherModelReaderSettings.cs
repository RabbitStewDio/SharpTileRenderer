using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public interface IXmlTileMatcherModelReaderSettings
    {
        void RegisterSelectorHandler(XName name, Func<XElement, IXmlTileMatcherModelReaderContext, ISelectorModel> h);
        void RegisterDataSetHandler(XName name, Func<XElement, IXmlTileMatcherModelReaderContext, IDataSetModel> h);
    }
}