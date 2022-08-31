using SharpTileRenderer.TileMatching.Model.DataSets;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public static class BuiltInXmlDataSetHandlers
    {
        static readonly XName quantifiedTagDataTag = XmlTileMatcherModelTags.NS + "quantified-tag-data";
        static readonly XName tagDataTag = XmlTileMatcherModelTags.NS + "tag-data";
        static readonly XName quantifiedClassSetTag = XmlTileMatcherModelTags.NS + "quantified-class-set";
        static readonly XName classSetTag = XmlTileMatcherModelTags.NS + "class-set";
        static readonly XName defaultQuantityTag = XmlTileMatcherModelTags.NS + "default-quantity";
        static readonly XName defaultClassTag = XmlTileMatcherModelTags.NS + "default-class";

        
        public static TSettings ConfigureBuiltInDataSetReadHandlers<TSettings>(this TSettings c)
            where TSettings: IXmlTileMatcherModelReaderSettings
        {
            c.RegisterDataSetHandler(classSetTag, ParseClassSetModel);
            c.RegisterDataSetHandler(quantifiedClassSetTag, ParseQuantifiedClassSetModel);
            c.RegisterDataSetHandler(tagDataTag, ParseTagModel);
            c.RegisterDataSetHandler(quantifiedTagDataTag, ParseQuantifiedTagModel);
            return c;
        }

        public static TSettings ConfigureBuiltInDataSetWriteHandlers<TSettings>(this TSettings c)
            where TSettings: IXmlTileMatcherModelWriterSettings
        {
            c.RegisterDataSetHandler(DataSetType.ClassSet, WriteClassSetModel);
            c.RegisterDataSetHandler(DataSetType.TagMap, WriteTagModel);
            c.RegisterDataSetHandler(DataSetType.QuantifiedClassSet, WriteQuantifiedClassSetModel);
            c.RegisterDataSetHandler(DataSetType.QuantifiedTagMap, WriteQuantifiedTagModel);
            return c;
        }

        static XElement? WriteQuantifiedTagModel(IDataSetModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not QuantifiedTagDataSetModel m)
            {
                return null;
            }
            
            var element = new XElement(quantifiedTagDataTag);
            element.AddStringElement(XmlTileMatcherModelTags.IdTag, m.Id);
            element.AddIntElement(defaultQuantityTag, m.DefaultQuantity);
            element.Add(m.Properties.AddPropertiesList());
            return element;
        }

        static XElement? WriteQuantifiedClassSetModel(IDataSetModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not QuantifiedClassSetDataSetModel m)
            {
                return null;
            }
            
            var element = new XElement(quantifiedClassSetTag);
            element.AddStringElement(XmlTileMatcherModelTags.IdTag, model.Id);
            element.AddStringElement(defaultClassTag, m.DefaultClass);
            element.AddIntElement(defaultQuantityTag, m.DefaultQuantity);
            element.Add(m.Properties.AddPropertiesList());
            return element;
        }

        static XElement? WriteTagModel(IDataSetModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not TagDataSetModel m)
            {
                return null;
            }
            
            var element = new XElement(tagDataTag);
            element.AddStringElement(XmlTileMatcherModelTags.IdTag, m.Id);
            element.Add(m.Properties.AddPropertiesList());
            return element;
        }

        static XElement? WriteClassSetModel(IDataSetModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not ClassSetDataSetModel m)
            {
                return null;
            }
            
            var element = new XElement(classSetTag);
            element.AddStringElement(XmlTileMatcherModelTags.IdTag, m.Id);
            element.AddStringElement(defaultClassTag, m.DefaultClass);
            element.Add(m.Properties.AddPropertiesList());
            return element;
        }

        public static IDataSetModel ParseTagModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var id = (string?)element.Element(XmlTileMatcherModelTags.IdTag) ?? throw new XmlParseException("Missing 'id'", element);
            var result = new TagDataSetModel()
            {
                Id = id
            };

            element.ParseProperties(result.Properties);

            return result;
        }

        public static IDataSetModel ParseQuantifiedTagModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var id = (string?)element.Element(XmlTileMatcherModelTags.IdTag) ?? throw new XmlParseException("Missing 'id'", element);
            var defaultQuantity = (int?)element.Element(defaultQuantityTag);
            var result = new QuantifiedTagDataSetModel()
            {
                Id = id,
                DefaultQuantity = defaultQuantity ?? 0
            };

            element.ParseProperties(result.Properties);

            return result;
        }

        public static IDataSetModel ParseClassSetModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var id = (string?)element.Element(XmlTileMatcherModelTags.IdTag) ?? throw new XmlParseException("Missing 'id'", element);
            var defaultClass = (string?)element.Element(defaultClassTag);

            var result = new ClassSetDataSetModel()
            {
                Id = id,
                DefaultClass = defaultClass
            };

            element.ParseClassesElement(result.Classes);
            element.ParseProperties(result.Properties);

            return result;
        }

        public static IDataSetModel ParseQuantifiedClassSetModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var id = (string?)element.Element(XmlTileMatcherModelTags.IdTag) ?? throw new XmlParseException("Missing 'id'", element);
            var defaultClass = (string?)element.Element(defaultClassTag);
            var defaultQuantity = (int?)element.Element(defaultQuantityTag);

            var result = new QuantifiedClassSetDataSetModel()
            {
                Id = id,
                DefaultClass = defaultClass,
                DefaultQuantity = defaultQuantity ?? 0
            };

            element.ParseClassesElement(result.Classes);
            element.ParseProperties(result.Properties);

            return result;
        }
    }
}