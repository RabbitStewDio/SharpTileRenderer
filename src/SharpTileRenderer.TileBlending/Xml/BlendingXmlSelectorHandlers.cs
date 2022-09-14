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
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BlendingSelectorModel.SelectorName, ParseBlendingSelectorModel);
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
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.ContextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var retval = new BlendingSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            retval.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-self"));
            retval.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-with"));
            return retval;
        }
        
        static XElement? WriteBlendingSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not BlendingSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BlendingSelectorModel.SelectorName);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.ContextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.Ns + "match-self", XmlTileSelectorModelTags.MatchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.Ns + "match-with", XmlTileSelectorModelTags.MatchClassTag));
            return element;
        }
    }
}