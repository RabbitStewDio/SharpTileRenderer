using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.TileMatching.Model.Meta;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.Util;
using SharpYaml.Model;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using YamlStream = SharpYaml.Model.YamlStream;

namespace SharpTileRenderer.Yaml.TileMatching
{
    public class YamlTileMatcherModelParser : IYamlTileMatcherModelReaderSettings, IYamlTileMatcherModelParserContext
    {
        static readonly IMemberNamingConvention namingConvention = YamlSerializerSettings.NamingConvention;
        readonly Dictionary<string, Func<YamlMapping, YamlParserContext, IDataSetModel>> dataSetFactories;
        readonly Dictionary<string, Func<YamlMapping, YamlParserContext, ISelectorModel>> selectorFactories;
        readonly Lazy<SerializerSettings> serializerSettings;

        public YamlTileMatcherModelParser()
        {
            dataSetFactories = new Dictionary<string, Func<YamlMapping, YamlParserContext, IDataSetModel>>();
            selectorFactories = new Dictionary<string, Func<YamlMapping, YamlParserContext, ISelectorModel>>();
            serializerSettings = new Lazy<SerializerSettings>(CreateSerializerSettings);
        }

        public void RegisterDataSetHandler(string id, Func<YamlMapping, YamlParserContext, IDataSetModel> fn)
        {
            dataSetFactories[id] = fn ?? throw new ArgumentNullException(nameof(fn));
        }

        public void RegisterSelectorHandler(string id, Func<YamlMapping, YamlParserContext, ISelectorModel> fn)
        {
            selectorFactories[id] = fn ?? throw new ArgumentException(nameof(fn));
        }

        public SerializerSettings SerializerSettings => serializerSettings.Value;

        protected virtual SerializerSettings CreateSerializerSettings()
        {
            return YamlSerializerSettings.CreateDefaultSerializerSettings();
        }

        public TileMatcherModel Load(IContentLoader cl,
                                     ContentUri fileName)
        {
            using var textStream = cl.LoadText(fileName);
            var stream = YamlStream.Load(textStream);
            if (stream.Count == 0)
            {
                throw new YamlParsingException("A tile matcher model file must not be empty.");
            }

            var doc = stream[0];
            var root = (doc.Contents as YamlMapping) ?? throw new YamlParsingException("Invalid content in yaml file");
            return Parse(root);
        }

        public TileMatcherModel Read(string txt)
        {
            var stream = YamlStream.Load(new StringReader(txt));
            if (stream.Count == 0)
            {
                throw new YamlParsingException("A tile matcher model file must not be empty.");
            }

            var doc = stream[0];
            var root = (doc.Contents as YamlMapping) ?? throw new YamlParsingException("Invalid content in yaml file");
            return Parse(root);
        }

        TileMatcherModel Parse(YamlMapping doc)
        {
            var author = doc[namingConvention.Convert(nameof(TileMatcherModel.Author))].AsString();
            var version = doc[namingConvention.Convert(nameof(TileMatcherModel.Version))].AsString();
            var documentation = doc[namingConvention.Convert(nameof(TileMatcherModel.Documentation))].AsString();
            var retval = new TileMatcherModel()
            {
                Author = author,
                Documentation = documentation,
                Version = version
            };

            ParseDataSets(doc, retval);
            ParseLayers(doc, retval);
            ParseGraphicTags(doc, retval);
            return retval;
        }

        void ParseGraphicTags(YamlMapping doc, TileMatcherModel retval)
        {
            if (doc[namingConvention.Convert(nameof(TileMatcherModel.Tags))] is not YamlSequence tags)
            {
                return;
            }

            foreach (var tagNode in tags)
            {
                if (tagNode is not YamlMapping element)
                {
                    continue;
                }

                var id = element[namingConvention.Convert(nameof(GraphicTagDefinitionModel.Id))].AsString();
                var model = new GraphicTagDefinitionModel()
                {
                    Id = id
                };

                var classes = element[namingConvention.Convert(nameof(GraphicTagDefinitionModel.Classes))] as YamlSequence;
                ParseStringList(classes, model.Classes);
                var flags = element[namingConvention.Convert(nameof(GraphicTagDefinitionModel.Flags))] as YamlSequence;
                ParseStringList(flags, model.Flags);

                if (element[namingConvention.Convert(nameof(GraphicTagDefinitionModel.Properties))] is YamlMapping propertiesNode)
                {
                    var dict = propertiesNode.ToObject<Dictionary<string, string>>();
                    foreach (var v in dict)
                    {
                        model.Properties[v.Key] = v.Value;
                    }
                }

                retval.Tags.Add(model);
            }
        }

        void ParseStringList(YamlSequence? sequence, ObservableCollection<string> target)
        {
            if (sequence == null) return;
            var values = sequence.ToObject<List<string>>();
            foreach (var v in values)
            {
                if (!string.IsNullOrEmpty(v))
                {
                    target.Add(v);
                }
            }
        }

        void ParseLayers(YamlMapping doc, TileMatcherModel retval)
        {
            if (doc[namingConvention.Convert(nameof(TileMatcherModel.RenderLayers))] is not YamlSequence layers)
            {
                return;
            }

            foreach (var layerNode in layers)
            {
                if (layerNode is not YamlMapping element)
                {
                    continue;
                }

                var renderLayerModel = ParseLayer(element);
                retval.RenderLayers.Add(renderLayerModel);
            }
        }

        RenderLayerModel ParseLayer(YamlMapping element)
        {
            var id = element.ParseProperty(namingConvention.Convert(nameof(RenderLayerModel.Id))) ?? throw new ArgumentException();
            var enabled = "true".Equals(element.ParseProperty(namingConvention.Convert(nameof(RenderLayerModel.Enabled))) ?? "true");
            var renderOrderText = element.ParseProperty(namingConvention.Convert(nameof(RenderLayerModel.RenderOrder)));
            int? renderOrder;
            if (!string.IsNullOrEmpty(renderOrderText))
            {
                if (!int.TryParse(renderOrderText, out var ro))
                {
                    throw new ArgumentException();
                }

                renderOrder = ro;
            }
            else
            {
                renderOrder = null;
            }

            var entitySourceNode = element[namingConvention.Convert(nameof(RenderLayerModel.EntitySource))] as YamlMapping;
            var entitySource = entitySourceNode?.ToObject<EntitySourceModel>(SerializerSettings);
            var matchNode = element[namingConvention.Convert(nameof(RenderLayerModel.Match))] as YamlMapping ?? throw new ArgumentException();

            var renderLayerModel = new RenderLayerModel()
            {
                Id = id,
                EntitySource = entitySource,
                RenderOrder = renderOrder,
                Enabled = enabled
            };

            if (element[namingConvention.Convert(nameof(RenderLayerModel.Properties))] is YamlMapping propertiesNode)
            {
                var dict = propertiesNode.ToObject<Dictionary<string, string>>();
                foreach (var v in dict)
                {
                    renderLayerModel.Properties[v.Key] = v.Value;
                }
            }

            var classes = element[namingConvention.Convert(nameof(RenderLayerModel.FeatureFlags))] as YamlSequence;
            ParseStringList(classes, renderLayerModel.FeatureFlags);
            if (TryParseSelector(matchNode).TryGetValue(out var match))
            {
                renderLayerModel.Match = match;
            }
            else
            {
                var subLayers = element[namingConvention.Convert(nameof(RenderLayerModel.SubLayers))] as YamlSequence;
                if (subLayers == null) throw new YamlParsingException($"Layer '{renderLayerModel.Id}' must contain either a valid matcher or a non-empty sub-layer list");
                foreach (var subLayer in subLayers)
                {
                    if (subLayer is YamlMapping subLayerMapping)
                    {
                        renderLayerModel.SubLayers.Add(ParseLayer(subLayerMapping));
                    } 
                }
            }
            
            return renderLayerModel;
        }

        public Optional<ISelectorModel> TryParseSelector(YamlMapping matchNodeYamlMapping)
        {
            var matchType = matchNodeYamlMapping.ParseProperty(namingConvention.Convert(nameof(ISelectorModel.Kind)));
            if (matchType == null) return default;
            if (selectorFactories.TryGetValue(matchType, out var factoryFunction))
            {
                var selector = factoryFunction(matchNodeYamlMapping, CreateContext());
                return Optional.OfNullable(selector);
            }

            throw new ArgumentException($"Matcher type {matchType} does not exist");
        }
        
        public ISelectorModel ParseSelector(YamlMapping matchNodeYamlMapping)
        {
            var matchType = matchNodeYamlMapping.ParseProperty(namingConvention.Convert(nameof(ISelectorModel.Kind))) ?? throw new ArgumentException("Matcher node is missing match-type property");
            if (selectorFactories.TryGetValue(matchType, out var factoryFunction))
            {
                return factoryFunction(matchNodeYamlMapping, CreateContext());
            }

            throw new ArgumentException($"Matcher type {matchType} does not exist");
        }

        public void ParseDataSets(YamlMapping doc, TileMatcherModel retval)
        {
            if (doc[namingConvention.Convert(nameof(TileMatcherModel.DataSets))] is not YamlSequence dataSets)
            {
                return;
            }

            foreach (var dsNode in dataSets)
            {
                if (dsNode is not YamlMapping element)
                {
                    continue;
                }

                var result = ParseDataSet(element);
                retval.DataSets.Add(result);
            }
        }

        public IDataSetModel ParseDataSet(YamlMapping element)
        {
            var type = element[namingConvention.Convert(nameof(IDataSetModel.Kind))].AsString() ?? throw new ArgumentException();
            if (!dataSetFactories.TryGetValue(type, out var producerFn))
            {
                throw new ArgumentException();
            }

            return producerFn(element, CreateContext());
        }

        YamlParserContext CreateContext() => new YamlParserContext(this, SerializerSettings);
    }
}