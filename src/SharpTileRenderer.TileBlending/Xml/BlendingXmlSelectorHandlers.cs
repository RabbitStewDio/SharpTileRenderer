using SharpTileRenderer.TileBlending.Matcher;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.Xml.TileMatching;
using System.Xml.Linq;

namespace SharpTileRenderer.TileBlending.Xml
{
    public static class BlendingXmlSelectorHandlers
    {
        public static TSettings ConfigureBlendHandlers<TSettings>(this TSettings c)
            where TSettings: IXmlTileMatcherModelReaderSettings
        {
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BlendingSelectorModel.SelectorName, ParseBlendingSelectorModel);
            return c;
        }
        
        public static TSettings ConfigureBlendWriteHandlers<TSettings>(this TSettings c)
            where TSettings : IXmlTileMatcherModelWriterSettings
        {
            c.RegisterSelectorHandler(BlendingSelectorModel.SelectorName, WriteBlendingSelectorModel);
            return c;
        }

        static ISelectorModel ParseBlendingSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.contextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var retval = new BlendingSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            retval.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-self"));
            retval.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-with"));
            return retval;
        }
        
        static XElement? WriteBlendingSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not BlendingSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.NS + BlendingSelectorModel.SelectorName);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.contextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.NS + "match-self", XmlTileSelectorModelTags.matchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.NS + "match-with", XmlTileSelectorModelTags.matchClassTag));
            return element;
        }
    }
}