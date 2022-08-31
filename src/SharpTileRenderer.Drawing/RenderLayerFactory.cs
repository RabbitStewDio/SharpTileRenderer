using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.TileMatching.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Drawing
{
    
    public class RenderLayerFactoryPart<TClassification>: IFeatureInitializer<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        public RenderLayerFactoryPart(NavigatorMetaData md, EntityClassificationRegistry<TClassification> registry)
        {
            this.features = new List<IFeatureModule>();
            this.MapNavigator = md;
            this.Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.TagMetaData = new GraphicTagMetaDataRegistry<TClassification>(registry);
            this.MatcherFactory = new MatcherFactory<TClassification>();
        }

        readonly List<IFeatureModule> features;
        
        public NavigatorMetaData MapNavigator { get; }

        public EntityClassificationRegistry<TClassification> Registry { get; }

        public GraphicTagMetaDataRegistry<TClassification> TagMetaData { get; }

        public MatcherFactory<TClassification> MatcherFactory { get; }

        public RenderLayerFactoryPart<TClassification> WithFeature(IFeatureModule f)
        {
            features.Add(f);
            f.Initialize(this);
            return this;
        }
        
        public RenderLayerFactoryPart<TClassification> WithDefaultMatchers()
        {
            this.MatcherFactory.WithDefaultMatchers();
            return this;
        }

        public RenderLayerFactoryPart<TClassification> RegisterTagSelector(string id, MatcherFactory<TClassification>.MatcherFactoryDelegate<GraphicTag> f)
        {
            this.MatcherFactory.RegisterTagSelector(id, f);
            return this;
        }

        public RenderLayerFactoryPart<TClassification> RegisterQuantifiedTagSelector(string id, MatcherFactory<TClassification>.MatcherFactoryDelegate<(GraphicTag, int)> f)
        {
            this.MatcherFactory.RegisterQuantifiedTagSelector(id, f);
            return this;
        }

        public RenderLayerFactoryPart<TClassification> Register(Action<MatcherFactory<TClassification>> action)
        {
            action(MatcherFactory);
            return this;
        }

        public RenderLayerProducerData<TClassification> PrepareForData()
        {
            return new RenderLayerProducerData<TClassification>(MapNavigator, Registry, TagMetaData, MatcherFactory, features.ToArray());
        }
    }

    public class RenderLayerProducerData<TClassification> : IRenderLayerProducerData<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        readonly IFeatureModule[] tileRendererFeatures;
        readonly List<IRenderLayerProducer<TClassification>> dataContexts;
        public NavigatorMetaData MapNavigator { get; }
        public EntityClassificationRegistry<TClassification> Registry { get; }
        public GraphicTagMetaDataRegistry<TClassification> TagMetaData { get; }
        public MatcherFactory<TClassification> MatcherFactory { get; }

        public RenderLayerProducerData(NavigatorMetaData md,
                                       EntityClassificationRegistry<TClassification> registry,
                                       GraphicTagMetaDataRegistry<TClassification> tagMetaData,
                                       MatcherFactory<TClassification> matcherFactory,
                                       IFeatureModule[] tileRendererFeatures)
        {
            this.tileRendererFeatures = tileRendererFeatures ?? throw new ArgumentNullException(nameof(tileRendererFeatures));
            MapNavigator = md;
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            TagMetaData = tagMetaData ?? throw new ArgumentNullException(nameof(tagMetaData));
            MatcherFactory = matcherFactory ?? throw new ArgumentNullException(nameof(matcherFactory));
            dataContexts = new List<IRenderLayerProducer<TClassification>>();
        }

        RenderLayerProducerData(EntityClassificationRegistry<TClassification> registry,
                                GraphicTagMetaDataRegistry<TClassification> tagMetaData,
                                MatcherFactory<TClassification> matcherFactory,
                                List<IRenderLayerProducer<TClassification>> dataContexts,
                                IFeatureModule[] tileRendererFeatures)
        {
            this.dataContexts = dataContexts ?? throw new ArgumentNullException(nameof(dataContexts));
            this.tileRendererFeatures = tileRendererFeatures ?? throw new ArgumentNullException(nameof(tileRendererFeatures));
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
        
        public RenderLayerProducerData<TClassification> WithDataSets<TEntity>(ITileDataSetProducer<TEntity> dataSets)
        {
            var dataContext = new List<IRenderLayerProducer<TClassification>>(this.dataContexts);
            foreach (var feature in tileRendererFeatures)
            {
                if (feature is IDrawingFeatureModule drawingFeature)
                {
                    if (drawingFeature.CreateRendererForData(this, dataSets, out var lp))
                    {
                        dataContext.Add(lp);
                    }
                }
            }
            return new RenderLayerProducerData<TClassification>(Registry, TagMetaData, MatcherFactory, dataContext, tileRendererFeatures);
        }

        public RenderLayerProducerData<TClassification> WithAvailableDataSets<TEntity>(ITileDataSetProducer<TEntity> dataSets,
                                                                                       IRenderLayerProducer<TClassification> layerProducer)
        {
            var dataContext = new List<IRenderLayerProducer<TClassification>>(this.dataContexts);
            dataContext.Add(layerProducer);
            return new RenderLayerProducerData<TClassification>(Registry, TagMetaData, MatcherFactory, dataContext, tileRendererFeatures);
        }

        public List<ILayer> ProduceLayers(TileMatcherModel renderConfig)
        {
            foreach (var tagDef in renderConfig.Tags)
            {
                if (string.IsNullOrEmpty(tagDef.Id)) continue;
                TagMetaData.Register(tagDef);
            }
            
            var result = new List<ILayer>();
            foreach (var renderLayer in renderConfig.RenderLayers)
            {
                if (!renderLayer.Enabled) continue;
                
                var layer = ProduceLayer(renderConfig, renderLayer);
                if (layer.TryGetValue(out var l))
                {
                    result.Add(l);
                }
            }

            return result;
        }

        Optional<ILayer> ProduceLayer(TileMatcherModel renderConfig, RenderLayerModel renderLayer)
        {
            var renderLayerEntitySource = renderLayer.EntitySource ?? throw new ArgumentException();
            var qid = renderLayerEntitySource.EntityQueryId ?? throw new ArgumentException();

            // First attempt to build feature-flag specific layers 
            if (renderLayer.FeatureFlags.Count > 0)
            {
                foreach (var ds in dataContexts)
                {
                    if (!ds.ContainsDataSet(qid))
                    {
                        continue;
                    }

                    if (!ds.FeatureFlag.TryGetValue(out var ff))
                    {
                        continue;
                    }

                    if (renderLayer.FeatureFlags.Contains(ff))
                    {
                        ILayer l = ds.Create(renderConfig, renderLayer, this);
                        return Optional.OfNullable(l);
                    }
                }
                
                return Optional.Empty<ILayer>();
            }

            // .. and only if there are no feature-flag layers defined, build a generic layer 
            foreach (var ds in dataContexts)
            {
                if (!ds.ContainsDataSet(qid))
                {
                    continue;
                }

                if (ds.FeatureFlag.TryGetValue(out _))
                {
                    continue;
                }

                ILayer l = ds.Create(renderConfig, renderLayer, this);
                return Optional.OfNullable(l);
            }

            return Optional.Empty<ILayer>();
        }
    }

    public abstract class RenderLayerProducerBase<TEntity, TClassification> : IRenderLayerProducer<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        readonly ITileDataSetProducer<TEntity> dataSets;

        protected RenderLayerProducerBase(ITileDataSetProducer<TEntity> dataSets, Optional<string> featureFlag)
        {
            this.dataSets = dataSets;
            this.FeatureFlag = featureFlag;
        }

        public bool ContainsDataSet(string id)
        {
            return this.dataSets.ContainsDataSet(id);
        }

        public Optional<string> FeatureFlag { get; }

        protected abstract ITileRenderer<TEntity> CreateRenderer(RenderLayerModel layer, IRenderLayerProducerData<TClassification> parameters);

        public ILayer Create(TileMatcherModel tileMatcherModel,
                             RenderLayerModel layer,
                             IRenderLayerProducerData<TClassification> parameters)
        {
            if (layer.Match == null) throw new ArgumentNullException();
            var ctx = DefaultMatchFactoryContext.From(dataSets, parameters.MapNavigator.BuildNavigator(), parameters.Registry, parameters.TagMetaData);

            var l = RenderLayerFactory.CreateLayer<TEntity>(layer, tileMatcherModel.DataSets);
            if (layer.Match.IsQuantifiedSelector)
            {
                return l.Counted()
                        .UsingGraphicTags()
                        .WithMatcher(layer.Match, parameters.MatcherFactory, ctx)
                        .Build(dataSets, CreateRenderer(layer, parameters));
            }

            return l.UsingGraphicTags()
                    .WithMatcher(layer.Match, parameters.MatcherFactory, ctx)
                    .Build(dataSets, CreateRenderer(layer, parameters));
        }
    }

    public interface IRenderLayerProducerData<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        public NavigatorMetaData MapNavigator { get; }
        public EntityClassificationRegistry<TClassification> Registry { get; }
        public GraphicTagMetaDataRegistry<TClassification> TagMetaData { get; }
        public MatcherFactory<TClassification> MatcherFactory { get; }
    }

    public interface IRenderLayerProducer<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        bool ContainsDataSet(string id);
        Optional<string> FeatureFlag { get; }
        ILayer Create(TileMatcherModel tileMatcherModel, RenderLayerModel layer, IRenderLayerProducerData<TClassification> parameters);
    }

    public class RenderLayerFactoryPart
    {
        readonly NavigatorMetaData md;

        public RenderLayerFactoryPart(NavigatorMetaData md)
        {
            this.md = md;
        }

        public RenderLayerFactoryPart<TClassification> WithClassification<TClassification>()
            where TClassification : struct, IEntityClassification<TClassification>
        {
            return new RenderLayerFactoryPart<TClassification>(md, new EntityClassificationRegistry<TClassification>());
        }
    }

    public static partial class RenderLayerFactory
    {
        public static RenderLayerFactoryPart DefineFactoryForMap(NavigatorMetaData md) => new RenderLayerFactoryPart(md);

        public static RenderFactoryData CreateLayer(string layerId)
        {
            return new RenderFactoryData(layerId, RenderingSortOrder.TopDownLeftRight, LayerQueryType.Grid, Array.Empty<IDataSetModel>());
        }

        public static RenderFactoryData<TEntityKey> CreateLayer<TEntityKey>(RenderLayerModel layerModel, IReadOnlyList<IDataSetModel> models)
        {
            if (layerModel.EntitySource == null) throw new ArgumentException();
            if (layerModel.Id == null) throw new ArgumentException();
            if (layerModel.EntitySource.EntityQueryId == null) throw new ArgumentException();
            return new RenderFactoryData(layerModel.Id,
                                         layerModel.EntitySource.SortingOrder,
                                         layerModel.EntitySource.LayerQueryType,
                                         models)
                .WithEntityData<TEntityKey>(layerModel.EntitySource.EntityQueryId);
        }

        public static TagRenderFactoryDataWithMatcher<TEntityKey> CreateGraphicTagLayer<TEntityKey, TClassification>(RenderLayerModel layerModel,
                                                                                                                     IReadOnlyList<IDataSetModel> models,
                                                                                                                     MatcherFactory<TClassification> matchFactory,
                                                                                                                     IMatchFactoryContext<TClassification> matchContext)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (layerModel.EntitySource == null) throw new ArgumentException();
            if (layerModel.Id == null) throw new ArgumentException();
            if (layerModel.EntitySource.EntityQueryId == null) throw new ArgumentException();
            if (layerModel.Match == null) throw new ArgumentException();
            var d = new RenderFactoryData(layerModel.Id,
                                          layerModel.EntitySource.SortingOrder,
                                          layerModel.EntitySource.LayerQueryType,
                                          models)
                .WithEntityData<TEntityKey>(layerModel.EntitySource.EntityQueryId);
            return d.UsingGraphicTags().WithMatcher(matchFactory.CreateTagMatcher(layerModel.Match, matchContext));
        }

        public readonly struct RenderFactoryData
        {
            internal readonly string LayerId;
            internal readonly RenderingSortOrder SortOrder;
            internal readonly LayerQueryType LayerQueryType;
            internal readonly IReadOnlyList<IDataSetModel> Models;


            internal RenderFactoryData(string layerId, RenderingSortOrder sortOrder, LayerQueryType layerQueryType, IReadOnlyList<IDataSetModel> models)
            {
                this.LayerId = layerId ?? throw new ArgumentNullException(nameof(layerId));
                this.SortOrder = sortOrder;
                this.LayerQueryType = layerQueryType;
                this.Models = models ?? throw new ArgumentNullException(nameof(models));
            }

            public RenderFactoryData ForSparseData()
            {
                return new RenderFactoryData(LayerId, SortOrder, LayerQueryType.Sparse, Models);
            }

            public RenderFactoryData ForGridData()
            {
                return new RenderFactoryData(LayerId, SortOrder, LayerQueryType.Grid, Models);
            }

            public RenderFactoryData WithQueryType(LayerQueryType t) => new RenderFactoryData(LayerId, SortOrder, t, Models);

            public RenderFactoryData WithRenderOrder(RenderingSortOrder sortOrder)
            {
                return new RenderFactoryData(LayerId, sortOrder, LayerQueryType, Models);
            }

            public RenderFactoryData WithAvailableDataSets(IReadOnlyList<IDataSetModel> models)
            {
                return new RenderFactoryData(LayerId, SortOrder, LayerQueryType, models);
            }

            public RenderFactoryData<TEntity> WithEntityData<TEntity>(string entityQuery)
            {
                return new RenderFactoryData<TEntity>(this, entityQuery);
            }
        }

        static Optional<IDataSetModel> FindDataSourceDefinition(IReadOnlyList<IDataSetModel> models, string primaryQueryId)
        {
            for (var index = 0; index < models.Count; index++)
            {
                var m = models[index];
                if (m.Id == primaryQueryId)
                {
                    return Optional.ValueOf(m);
                }
            }

            return Optional.Empty();
        }
    }
}