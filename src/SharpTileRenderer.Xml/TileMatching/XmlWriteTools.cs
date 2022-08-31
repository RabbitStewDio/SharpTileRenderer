using System.Collections.Generic;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public static class XmlWriteTools
    {
        public static XElement AddStringElement(this XElement e, XName name, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                e.Add(new XElement(name, value));
            }

            return e;
        }

        public static XElement AddIntElement(this XElement e, XName name, int value)
        {
            if (value == 0)
            {
                return e;
            }

            e.Add(new XElement(name, $"{value}"));
            return e;
        }

        public static XElement? AddStringList(this IReadOnlyList<string> value, XName listTag, XName tag)
        {
            if (value.Count == 0)
            {
                return null;
            }
            var retval = new XElement(listTag);
            foreach (var v in value)
            {
                retval.AddStringElement(tag, v);
            }

            return retval;
        }

        public static XElement? AddPropertiesList(this IReadOnlyDictionary<string, string> value)
        {
            if (value.Count == 0)
            {
                return null;
            }
            var retval = new XElement(XmlTileMatcherModelTags.PropertiesTag);
            foreach (var v in value)
            {
                var propertyElement = new XElement(XmlTileMatcherModelTags.PropertyTag);
                propertyElement.AddStringElement(XmlTileMatcherModelTags.NameTag, v.Key);
                propertyElement.AddStringElement(XmlTileMatcherModelTags.ValueTag, v.Value);
                retval.Add(propertyElement);
            }

            return retval;
        }
    }
}