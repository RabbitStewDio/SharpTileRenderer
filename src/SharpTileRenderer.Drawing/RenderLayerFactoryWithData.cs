using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Drawing
{
    /// <summary>
    ///   Third stage.
    /// </summary>
    /// <typeparam name="TClassification"></typeparam>
    public class RenderLayerFactoryWithData<TClassification> : IRenderLayerProducerConfig<TClassification>, IRenderLayerTypeLift
        where TClassification : struct, IEntityClassification<TClassification>
    {
        readonly IFeatureModule[] tileRendererFeatures;
        readonly Dictionary<Type, IDataContextHolder> dataContexts;
        public NavigatorMetaData MapNavigator { get; }
        public EntityClassificationRegistry<TClassification> Registry { get; }
        public GraphicTagMetaDataRegistry<TClassification> TagMetaData { get; }
        public MatcherFactory<TClassification> MatcherFactory { get; }

        public RenderLayerFactoryWithData(NavigatorMetaData md,
                                          EntityClassificationRegistry<TClassification> registry,
                                          GraphicTagMetaDataRegistry<TClassification> tagMetaData,
                                          MatcherFactory<TClassification> matcherFactory,
                                          IFeatureModule[] tileRendererFeatures)
        {
            this.tileRendererFeatures = tileRendererFeatures ?? throw new ArgumentNullException(nameof(tileRendererFeatures));
            Array.Sort(this.tileRendererFeatures, (a, b) => -a.PreferenceWeight.CompareTo(b.PreferenceWeight));
            this.dataContexts = new Dictionary<Type, IDataContextHolder>();

            MapNavigator = md;
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            TagMetaData = tagMetaData ?? throw new ArgumentNullException(nameof(tagMetaData));
            MatcherFactory = matcherFactory ?? throw new ArgumentNullException(nameof(matcherFactory));
        }

        public bool TryGetFeature<TFeature>([MaybeNullWhen(false)] out TFeature f)
        {
            foreach (var feat in tileRendererFeatures)
            {
                if (feat is TFeature maybeFeature)
                {
                    f = maybeFeature;
                    return true;
                }
            }

            f = default;
            return false;
        }

        public bool TryGetFeature<TFeature>([MaybeNullWhen(false)] out TFeature f, Func<TFeature, bool>? featureFilter)
        {
            if (featureFilter == null)
            {
                f = default;
                return false;
            }
            
            foreach (var feat in tileRendererFeatures)
            {
                if (feat is TFeature maybeFeature && featureFilter(maybeFeature))
                {
                    f = maybeFeature;
                    return true;
                }
            }

            f = default;
            return false;
        }

        public RenderLayerFactoryWithData<TClassification> WithDataSets<TEntity>(ITileDataSetProducer<TEntity> dataSets)
        {
            if (!dataContexts.TryGetValue(typeof(TEntity), out var ds))
            {
                var dsx = new DataContextHolder<TEntity>();
                dsx.Add(dataSets);
                dataContexts[typeof(TEntity)] = dsx;
            }
            else
            {
                var dsx = (DataContextHolder<TEntity>)ds;
                dsx.Add(dataSets);
            }

            return this;
        }

        public List<ILayer> ProduceLayers(TileMatcherModel renderConfig)
        {
            RegisterTagMetaData(renderConfig);
            InitializeFeatures();
            return ProduceLayersInternal(renderConfig);
        }

        void InitializeFeatures()
        {
            foreach (var feature in tileRendererFeatures)
            {
                feature.Initialize(this);
            }
        }

        void RegisterTagMetaData(TileMatcherModel renderConfig)
        {
            foreach (var tagDef in renderConfig.Tags)
            {
                if (string.IsNullOrEmpty(tagDef.Id))
                {
                    continue;
                }

                TagMetaData.Register(tagDef);
            }
        }

        List<ILayer> ProduceLayersInternal(TileMatcherModel renderConfig)
        {
            var result = new List<ILayer>();
            foreach (var renderLayer in renderConfig.RenderLayers)
            {
                if (!renderLayer.Enabled)
                {
                    continue;
                }

                var layer = ProduceLayer(renderConfig, renderLayer);
                if (layer.TryGetValue(out var l))
                {
                    result.Add(l);
                }
            }

            return result;
        }


        Optional<ILayer> ProduceLayer(TileMatcherModel renderConfig,
                                      RenderLayerModel renderLayer)
        {
            foreach (var ds in dataContexts.Values)
            {
                if (ds.CanHandleFully(renderLayer))
                {
                    var layer = ds.Apply(renderConfig, renderLayer, this);
                    if (layer.HasValue)
                    {
                        return layer;
                    }
                }
            }

            return default;
        }

        Optional<ILayer> IRenderLayerTypeLift.Apply<TEntity>(TileMatcherModel model,
                                                             RenderLayerModel layer,
                                                             DataContextHolder<TEntity> ctx)
        {
            if (layer.IsQuantifiedLayer())
            {
                if (ProduceLayerForCountedEntity(model, layer, ctx).TryGetValue(out var l))
                {
                    ILayer ll = l;
                    return Optional.OfNullable(ll);
                }
            }
            else
            {
                if (ProduceLayerForEntity(model, layer, ctx).TryGetValue(out var l))
                {
                    ILayer ll = l;
                    return Optional.OfNullable(ll);
                }
            }

            return Optional.Empty();
        }

        Optional<ILayer<TEntity>> ProduceLayerForEntity<TEntity>(TileMatcherModel model,
                                                                 RenderLayerModel layer,
                                                                 DataContextHolder<TEntity> ctx)
        {
            if (layer.SubLayers.Count > 0)
            {
                return ProduceCombinedLayer(model, layer, ctx);
            }

            return CreateInternal(model, layer, ctx);
        }

        Optional<ILayer<(TEntity, int)>> ProduceLayerForCountedEntity<TEntity>(TileMatcherModel model,
                                                                               RenderLayerModel layer,
                                                                               DataContextHolder<TEntity> ctx)
        {
            if (layer.SubLayers.Count > 0)
            {
                return ProduceCountedCombinedLayer(model, layer, ctx);
            }

            return CreateCountedInternal(model, layer, ctx);
        }

        Optional<ILayer<TEntity>> ProduceCombinedLayer<TEntity>(TileMatcherModel model,
                                                                RenderLayerModel layer,
                                                                DataContextHolder<TEntity> ctx)
        {
            var layers = new List<ILayer<TEntity>>();
            if (layer.EntitySource != null || layer.Match != null)
            {
                var syntheticLayer = new RenderLayerModel()
                {
                    Enabled = true,
                    Id = "Synthetic-" + layer.Id,
                    EntitySource = layer.EntitySource,
                    Match = layer.Match,
                    RenderOrder = layer.RenderOrder,
                    SortingOrder = layer.SortingOrder
                };
                if (ProduceLayerForEntity(model, syntheticLayer, ctx).TryGetValue(out var l))
                {
                    layers.Add(l);
                }
            }

            foreach (var subLayer in layer.SubLayers)
            {
                if (ProduceLayerForEntity(model, subLayer, ctx).TryGetValue(out var l))
                {
                    layers.Add(l);
                }
            }

            if (layers.Count == 0)
            {
                return Optional.Empty();
            }

            if (ProduceRenderer<TEntity>(model, layer).TryGetValue(out var renderer))
            {
                return RenderLayerFactory.CreateCombinedLayer(layer, renderer, layers.ToArray());
            }

            throw new ArgumentException($"Unable to produce renderer for layer {layer.Id}");
        }

        Optional<ILayer<(TEntity, int)>> ProduceCountedCombinedLayer<TEntity>(TileMatcherModel model,
                                                                              RenderLayerModel layer,
                                                                              DataContextHolder<TEntity> ctx)
        {
            var layers = new List<ILayer<(TEntity, int)>>();
            if (layer.EntitySource != null || layer.Match != null)
            {
                var syntheticLayer = new RenderLayerModel()
                {
                    Enabled = true,
                    Id = "Synthetic-" + layer.Id,
                    EntitySource = layer.EntitySource,
                    Match = layer.Match,
                    RenderOrder = layer.RenderOrder,
                    SortingOrder = layer.SortingOrder
                };
                if (ProduceLayerForCountedEntity(model, syntheticLayer, ctx).TryGetValue(out var l))
                {
                    layers.Add(l);
                }
            }

            foreach (var subLayer in layer.SubLayers)
            {
                if (ProduceLayerForCountedEntity(model, subLayer, ctx).TryGetValue(out var l))
                {
                    layers.Add(l);
                }
            }

            if (layers.Count == 0)
            {
                return Optional.Empty();
            }

            if (ProduceRenderer<(TEntity, int)>(model, layer).TryGetValue(out var renderer))
            {
                return RenderLayerFactory.CreateCombinedLayer(layer, renderer, layers.ToArray());
            }

            throw new ArgumentException($"Unable to produce renderer for layer {layer.Id}");
        }

        Optional<ILayer<TEntity>> CreateInternal<TEntity>(TileMatcherModel model,
                                                          RenderLayerModel layer,
                                                          ITileDataSetProducer<TEntity> dataSet)
        {
            if (layer.Enabled == false)
            {
                return default;
            }

            if (layer.Match == null || layer.EntitySource == null)
            {
                return default;
            }

            var ctx = DefaultMatchFactoryContext.From(dataSet, MapNavigator.BuildNavigator(), Registry, TagMetaData);
            if (!ProduceRenderer<TEntity>(model, layer).TryGetValue(out var renderer))
            {
                throw new ArgumentException($"Unable to produce renderer for layer {layer.Id}");
            }

            var producedLayer = RenderLayerFactory.CreateLayer<TEntity>(layer, model.DataSets)
                                                  .UsingGraphicTags()
                                                  .WithMatcher(layer.Match, MatcherFactory, ctx)
                                                  .Build(dataSet, renderer);
            return Optional.OfNullable(producedLayer);
        }

        Optional<ILayer<(TEntity, int)>> CreateCountedInternal<TEntity>(TileMatcherModel model,
                                                                        RenderLayerModel layer,
                                                                        ITileDataSetProducer<TEntity> dataSet)
        {
            if (layer.Enabled == false)
            {
                return Optional.Empty();
            }

            if (layer.Match == null || layer.EntitySource == null)
            {
                return Optional.Empty();
            }

            var ctx = DefaultMatchFactoryContext.From(dataSet, MapNavigator.BuildNavigator(), Registry, TagMetaData);
            if (!ProduceRenderer<TEntity>(model, layer).TryGetValue(out var renderer))
            {
                throw new ArgumentException($"Unable to produce renderer for layer {layer.Id}");
            }

            var l = RenderLayerFactory.CreateLayer<TEntity>(layer, model.DataSets)
                                      .Counted()
                                      .UsingGraphicTags()
                                      .WithMatcher(layer.Match, MatcherFactory, ctx)
                                      .Build(dataSet, renderer);
            return Optional.OfNullable(l);
        }

        Optional<ITileRenderer<TEntity>> ProduceRenderer<TEntity>(TileMatcherModel model, RenderLayerModel layer)
        {
            foreach (var module in tileRendererFeatures)
            {
                if (module is not IDrawingFeatureModule drawModule)
                {
                    continue;
                }

                if (drawModule.CreateRendererForData<TEntity, TClassification>(this, model, layer).TryGetValue(out var v))
                {
                    return Optional.OfNullable(v);
                }
            }

            return Optional.Empty();
        }
    }
}