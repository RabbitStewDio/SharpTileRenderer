using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.TileMatching.Model.Meta;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public class XmlTileMatcherModelParser: IXmlTileMatcherModelReaderContext, IXmlTileMatcherModelReaderSettings
    {
        readonly Dictionary<XName, Func<XElement, IXmlTileMatcherModelReaderContext, ISelectorModel>> selectorHandlers;
        readonly Dictionary<XName, Func<XElement, IXmlTileMatcherModelReaderContext, IDataSetModel>> dataSetHandlers;

        public XmlTileMatcherModelParser()
        {
            selectorHandlers = new Dictionary<XName, Func<XElement, IXmlTileMatcherModelReaderContext, ISelectorModel>>();
            dataSetHandlers = new Dictionary<XName, Func<XElement, IXmlTileMatcherModelReaderContext, IDataSetModel>>();
        }

        public void RegisterSelectorHandler(XName name, Func<XElement, IXmlTileMatcherModelReaderContext, ISelectorModel> h)
        {
            selectorHandlers[name] = h ?? throw new ArgumentNullException(nameof(h));
        }
        
        public void RegisterDataSetHandler(XName name, Func<XElement, IXmlTileMatcherModelReaderContext, IDataSetModel> h)
        {
            dataSetHandlers[name] = h ?? throw new ArgumentNullException(nameof(h));
        }
        
        public TileMatcherModel Load(IContentLoader cl,
                                     ContentUri fileName)
        {
            using var textStream = cl.LoadText(fileName);
            var document = XDocument.Load(textStream);
            var root = document.Root ?? throw new ArgumentException();
            var author = (string?)root.Element(XmlTileMatcherModelTags.AuthorTag);
            var documentation = (string?)root.Element(XmlTileMatcherModelTags.DocumentationTag);
            var version = (string?)root.Element(XmlTileMatcherModelTags.VersionTag);

            var model = new TileMatcherModel()
            {
                Author = author,
                Documentation = documentation,
                Version = version
            };

            var dataSets = root.Element(XmlTileMatcherModelTags.DataSetsTag);
            if (dataSets != null)
            {
                foreach (var dataSet in dataSets.Elements())
                {
                    if (!ParseDataSets(dataSet).TryGetValue(out var dataSetDefinition))
                    {
                        continue;
                    }

                    model.DataSets.Add(dataSetDefinition);
                }
            }

            var layers = root.Element(XmlTileMatcherModelTags.LayersTag);
            if (layers != null)
            {
                foreach (var layer in layers.Elements(XmlTileMatcherModelTags.LayerTag))
                {
                    model.RenderLayers.Add(ParseLayer(layer));
                }
            }

            var tags = root.Element(XmlTileMatcherModelTags.TagsTag);
            if (tags != null)
            {
                foreach (var tag in tags.Elements(XmlTileMatcherModelTags.GraphicTagTag))
                {
                    model.Tags.Add(ParseGraphicTag(tag));
                }
            }

            return model;
        }

        GraphicTagDefinitionModel ParseGraphicTag(XElement tag)
        {
            var id = (string?)tag.Element(XmlTileMatcherModelTags.IdTag) ?? throw new XmlParseException("Missing child element 'id'", tag);
            var result = new GraphicTagDefinitionModel()
            {
                Id = id
            };

            tag.ParseClassesElement(result.Classes);
            tag.ParseProperties(result.Properties);
            result.Flags.ParseStringList(tag.Element(XmlTileMatcherModelTags.FlagsTag));
            return result;
        }

        public Optional<IDataSetModel> ParseDataSets(XElement dataSet)
        {
            if (!dataSetHandlers.TryGetValue(dataSet.Name, out var handler))
            {
                return default;
            }

            var ds = handler(dataSet, this);
            return Optional.ValueOf(ds);
        }

        EntitySourceModel? ParseEntitySource(XElement? entitySource)
        {
            if (entitySource == null)
            {
                return null;
            }
            
            var entityQuery = (string?)entitySource.Element(XmlTileMatcherModelTags.EntityQueryIdTag) ?? throw new XmlParseException($"Missing child element {XmlTileMatcherModelTags.EntityQueryIdTag}", entitySource);
            var entityQueryType = entitySource.ParseEnumElement<LayerQueryType>(XmlTileMatcherModelTags.EntityQueryTypeTag, LayerQueryType.Grid) ?? throw new XmlParseException($"Missing child element {XmlTileMatcherModelTags.EntityQueryTypeTag}", entitySource);
            return new EntitySourceModel()
            {
                EntityQueryId = entityQuery,
                LayerQueryType = entityQueryType,
            };
        }
        
        RenderLayerModel ParseLayer(XElement layer)
        {
            var id = (string?)layer.Element(XmlTileMatcherModelTags.LayerIdTag);
            var enabled = "true".Equals((string?)layer.Element(XmlTileMatcherModelTags.EnabledTag) ?? "true", StringComparison.InvariantCulture);

            var sortOrder = layer.ParseEnumElement<RenderingSortOrder>(XmlTileMatcherModelTags.EntitySortOrderTag, RenderingSortOrder.TopDownLeftRight) ?? throw new XmlParseException($"Missing child element {XmlTileMatcherModelTags.EntitySortOrderTag}", layer);
            var entitySource = ParseEntitySource(layer.Element(XmlTileMatcherModelTags.EntitySourceTag));
            var renderOrder = ((int?)layer.Element(XmlTileMatcherModelTags.RenderOrder)) ?? 0;
            var matchElements = layer.Elements();
            var renderLayerModel = new RenderLayerModel()
            {
                Id = id,
                EntitySource = entitySource,
                RenderOrder = renderOrder,
                SortingOrder = sortOrder,
                Enabled = enabled
            };
            
            layer.ParseProperties(renderLayerModel.Properties);
            layer.ParseStringListElement(XmlTileMatcherModelTags.FeatureFlagsTag, XmlTileMatcherModelTags.FeatureFlagTag, renderLayerModel.FeatureFlags);
            
            foreach (var me in matchElements)
            {
                if (ParseSelector(me).TryGetValue(out var m))
                {
                    renderLayerModel.Match = m;
                    return renderLayerModel;
                }
            }

            var subLayers = layer.Elements(XmlTileMatcherModelTags.SubLayerTag);
            foreach (var subLayer in subLayers)
            {
                renderLayerModel.SubLayers.Add(ParseLayer(subLayer));
            }

            if (renderLayerModel.SubLayers.Count == 0)
            {
                throw new XmlParseException("Unable to find valid match definition", layer);
            }

            return renderLayerModel;
        }

        public Optional<ISelectorModel> ParseSelector(XElement me)
        {
            if (selectorHandlers.TryGetValue(me.Name, out var handler))
            {
                var x = handler(me, this);
                return Optional.ValueOf(x);
            }

            return default;
        }

    }
}