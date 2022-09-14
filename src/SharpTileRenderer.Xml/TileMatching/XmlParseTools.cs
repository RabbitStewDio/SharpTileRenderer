using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public static class XmlParseTools
    {
        public static void ParseStringList(this ObservableCollection<string> matchedTags,
                                           XElement? tags)
        {
            if (tags == null)
            {
                return;
            }

            var childElements = tags.Elements();
            foreach (var tag in childElements)
            {
                var t = (string?)tag;
                if (string.IsNullOrEmpty(t))
                {
                    throw new XmlParseException("Empty tag found", tag);
                }

                matchedTags.Add(t);
            }
        }

        public static void ParseProperties(this XElement element, ObservableDictionary<string, string> result)
        {
            var propertiesNode = element.Element(XmlTileMatcherModelTags.PropertiesTag);
            if (propertiesNode == null)
            {
                return;
            }

            foreach (var property in propertiesNode.Elements(XmlTileMatcherModelTags.PropertyTag))
            {
                var name = (string?)property.Element(XmlTileMatcherModelTags.NameTag) ?? throw new XmlParseException("required node 'name'", property);
                var value = (string?)property.Element(XmlTileMatcherModelTags.ValueTag) ?? throw new XmlParseException("required node 'value'", property);
                result.Add(new KeyValuePair<string, string>(name, value));
            }
        }

        public static void ParseStringListElement(this XElement element, XName listTag, XName elementTag, ObservableCollection<string> result)
        {
            var classesNode = element.Element(listTag);
            if (classesNode == null)
            {
                return;
            }
            
            foreach (var cls in classesNode.Elements(elementTag))
            {
                var clsName = (string?)cls ?? throw new XmlParseException("Invalid content", cls);
                result.Add(clsName);
            }
        }

        public static void ParseClassesElement(this XElement element, ObservableCollection<string> result)
        {
            element.ParseStringListElement(XmlTileMatcherModelTags.ClassesTag, XmlTileMatcherModelTags.ClassTag, result);
        }

        public static T? ParseEnumElement<T>(this XElement e, XName name, T? defaultValue = null)
            where T : struct
        {
            var childElement = e.Element(name);
            if (childElement == null)
            {
                return defaultValue;
            }

            var content = (string?)childElement;
            return content.ParseEnumStrict(childElement, defaultValue);
        }

        public static T? ParseEnumStrict<T>(this string? t, IXmlLineInfo lineInfo, T? defaultValue = null)
            where T : struct
        {
            if (string.IsNullOrEmpty(t))
            {
                return defaultValue;
            }

            if (!Enum.TryParse(t, out T result))
            {
                throw new XmlParseException($"Attribute value '{t}' for enum {typeof(T)} is invalid.", lineInfo);
            }

            return result;
        }

        public static Optional<ISelectorModel> ParseAnySelector(this XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var matchElements = element.Elements();
            foreach (var me in matchElements)
            {
                var r = context.ParseSelector(me);
                if (r.HasValue)
                {
                    return r;
                }
            }

            return Optional.Empty<ISelectorModel>();
        }
    }
}