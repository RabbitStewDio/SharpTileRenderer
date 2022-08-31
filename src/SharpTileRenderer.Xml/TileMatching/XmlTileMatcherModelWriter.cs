using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.TileMatching.Model.Meta;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public interface IXmlTileMatcherModelWriterContext
    {
        XElement? WriteDataSet(IDataSetModel? model);
        XElement? WriteSelector(ISelectorModel? model);
    }

    public interface IXmlTileMatcherModelWriterSettings
    {
        void RegisterSelectorHandler(string kind, SelectorWriteHandler handler);
        void RegisterDataSetHandler(DataSetType kind, DataSetWriteHandler handler);
    }

    public delegate XElement? DataSetWriteHandler(IDataSetModel model, IXmlTileMatcherModelWriterContext context);

    public delegate XElement? SelectorWriteHandler(ISelectorModel model, IXmlTileMatcherModelWriterContext context);

    public class XmlTileMatcherModelWriter : IXmlTileMatcherModelWriterContext, IXmlTileMatcherModelWriterSettings
    {
        readonly Dictionary<DataSetType, DataSetWriteHandler> dataSetWriteHandlers;
        readonly Dictionary<string, SelectorWriteHandler> selectorWriteHandlers;

        public XmlTileMatcherModelWriter()
        {
            dataSetWriteHandlers = new Dictionary<DataSetType, DataSetWriteHandler>();
            selectorWriteHandlers = new Dictionary<string, SelectorWriteHandler>();
        }

        public void RegisterSelectorHandler(string kind, SelectorWriteHandler handler)
        {
            if (kind == null)
            {
                throw new ArgumentNullException(nameof(kind));
            }

            selectorWriteHandlers[kind] = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void RegisterDataSetHandler(DataSetType kind, DataSetWriteHandler handler)
        {
            dataSetWriteHandlers[kind] = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public string Write(TileMatcherModel model)
        {
            var docNode = new XElement(XmlTileMatcherModelTags.TileSpecTag);
            docNode.Add(new XAttribute(XNamespace.Xmlns + "ts", XmlTileSelectorModelTags.NS));
            docNode.AddStringElement(XmlTileMatcherModelTags.AuthorTag, model.Author);
            docNode.AddStringElement(XmlTileMatcherModelTags.VersionTag, model.Version);
            docNode.AddStringElement(XmlTileMatcherModelTags.DocumentationTag, model.Documentation);
            docNode.Add(AddTagsElement(model.Tags));
            docNode.Add(AddDataSetsElement(model.DataSets));
            docNode.Add(AddRenderLayersElement(model.RenderLayers));
            return docNode.ToString(SaveOptions.OmitDuplicateNamespaces);
        }

        XElement? AddRenderLayersElement(IReadOnlyList<RenderLayerModel> layers)
        {
            if (layers.Count == 0) return null;
            var renderLayersElement = new XElement(XmlTileMatcherModelTags.LayersTag);
            foreach (var l in layers)
            {
                var renderLayerElement = new XElement(XmlTileMatcherModelTags.LayerTag);
                renderLayerElement.AddStringElement(XmlTileMatcherModelTags.IdTag, l.Id);
                if (l.RenderOrder != null)
                {
                    renderLayerElement.AddStringElement(XmlTileMatcherModelTags.RenderOrder, $"{l.RenderOrder}");
                }

                renderLayerElement.AddStringElement(XmlTileMatcherModelTags.EnabledTag, l.Enabled ? "true" : "false");
                renderLayerElement.Add(CreateEntitySourceElement(l.EntitySource));
                renderLayerElement.Add(WriteSelector(l.Match));
                renderLayerElement.Add(l.FeatureFlags.AddStringList(XmlTileMatcherModelTags.FeatureFlagsTag, XmlTileMatcherModelTags.FeatureFlagTag));
                renderLayerElement.Add(l.Properties.AddPropertiesList());
                renderLayersElement.Add(renderLayerElement);
            }

            return renderLayersElement;
        }

        XElement? CreateEntitySourceElement(EntitySourceModel? entitySource)
        {
            if (entitySource == null) return null;
            
            var retval = new XElement(XmlTileMatcherModelTags.EntitySourceTag);
            retval.AddStringElement(XmlTileMatcherModelTags.EntityQueryIdTag, entitySource.EntityQueryId);
            retval.AddStringElement(XmlTileMatcherModelTags.EntitySortOrderTag, $"{entitySource.SortingOrder}");
            retval.AddStringElement(XmlTileMatcherModelTags.EntityQueryTypeTag, $"{entitySource.LayerQueryType}");
            return retval;
        }

        XElement? AddDataSetsElement(IReadOnlyList<IDataSetModel> dataSets)
        {
            if (dataSets.Count == 0) return null;
            var dataSetsElement = new XElement(XmlTileMatcherModelTags.DataSetsTag);
            foreach (var ds in dataSets)
            {
                dataSetsElement.Add(WriteDataSet(ds));
            }

            return dataSetsElement;
        }

        public XElement? WriteDataSet(IDataSetModel? model)
        {
            if (model == null) return null;
            
            if (dataSetWriteHandlers.TryGetValue(model.Kind, out var handler))
            {
                return handler(model, this);
            }

            return null;
        }

        public XElement? WriteSelector(ISelectorModel? model)
        {
            if (model == null) return null;
            
            if (selectorWriteHandlers.TryGetValue(model.Kind, out var handler))
            {
                return handler(model, this);
            }

            return null;
        }

        XElement? AddTagsElement(IReadOnlyList<GraphicTagDefinitionModel> tags)
        {
            if (tags.Count == 0) return null;

            var tagsElement = new XElement(XmlTileMatcherModelTags.TagsTag);
            foreach (var tag in tags)
            {
                tagsElement.Add(CreateTagElement(tag));
            }

            return tagsElement;
        }

        XElement CreateTagElement(GraphicTagDefinitionModel tag)
        {
            var tagElement = new XElement(XmlTileMatcherModelTags.GraphicTagTag);
            tagElement.AddStringElement(XmlTileMatcherModelTags.IdTag, tag.Id);
            tagElement.Add(tag.Flags.AddStringList(XmlTileMatcherModelTags.FlagsTag, XmlTileMatcherModelTags.FlagTag));
            tagElement.Add(tag.Classes.AddStringList(XmlTileMatcherModelTags.ClassesTag, XmlTileMatcherModelTags.ClassTag));
            tagElement.Add(tag.Properties.AddPropertiesList());
            return tagElement;
        }
    }
}