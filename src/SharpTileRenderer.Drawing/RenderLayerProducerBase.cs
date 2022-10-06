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
using System.Linq;

namespace SharpTileRenderer.Drawing
{
    public abstract class RenderLayerProducerBase<TEntity, TClassification> : IRenderLayerProducer<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        readonly ITileDataSetProducer<TEntity> dataSets;

        protected RenderLayerProducerBase(ITileDataSetProducer<TEntity> dataSets, Optional<string> featureFlag)
        {
            this.dataSets = dataSets;
            this.FeatureFlag = featureFlag;
        }

        public bool HandlesLayer(RenderLayerModel layer)
        {
            var handledLayer = true;
            var checkedLayer = false;
            foreach (var l in layer.SubLayers)
            {
                checkedLayer = true;
                if (!HandlesLayer(l))
                {
                    handledLayer = false;
                }
            }

            var queryId = layer.EntitySource?.EntityQueryId;
            if (queryId != null)
            {
                checkedLayer = true;
                if (!ContainsDataSet(queryId))
                {
                    handledLayer = false;
                }
            }

            if (!checkedLayer)
            {
                return false;
            }

            return handledLayer;
        }

        bool ContainsDataSet(string id)
        {
            return this.dataSets.ContainsDataSet(id);
        }

        public Optional<string> FeatureFlag { get; }

        protected virtual ITileRenderer<(TEntity, int)> CreateQuantifiedRenderer(RenderLayerModel layer, IRenderLayerProducerConfig<TClassification> parameters)
        {
            return new StripQuantityTileRenderer<TEntity, int>(CreateRenderer(layer, parameters));
        }
        
        protected abstract ITileRenderer<TEntity> CreateRenderer(RenderLayerModel layer, IRenderLayerProducerConfig<TClassification> parameters);

        public ILayer Create(TileMatcherModel tileMatcherModel,
                             RenderLayerModel layer,
                             IRenderLayerProducerConfig<TClassification> parameters)
        {
            if (layer.IsQuantifiedLayer())
            {
                return CreateInternalQuantified(tileMatcherModel, layer, parameters);
            }

            return CreateInternal(tileMatcherModel, layer, parameters);
        }

        protected ILayer<TEntity> CreateInternal(TileMatcherModel tileMatcherModel,
                                                 RenderLayerModel layer,
                                                 IRenderLayerProducerConfig<TClassification> parameters)
        {
            if (layer.SubLayers.Count > 0)
            {
                var layers = new List<ILayer<TEntity>>();
                foreach (var subLayer in layer.SubLayers)
                {
                    layers.Add(CreateInternal(tileMatcherModel, subLayer, parameters));
                }

                return RenderLayerFactory.CreateCombinedLayer(layer, CreateRenderer(layer, parameters), layers.ToArray());
            }

            if (layer.Match == null) throw new ArgumentNullException();
            var ctx = DefaultMatchFactoryContext.From(dataSets, parameters.MapNavigator.BuildNavigator(), parameters.Registry, parameters.TagMetaData);

            if (layer.SubLayers.Count > 0)
            {
                // create a combined layer.
                var layers = layer.SubLayers.Select(l => CreateInternal(tileMatcherModel, l, parameters)).ToArray();
                return RenderLayerFactory.CreateCombinedLayer(layer, CreateRenderer(layer, parameters), layers);
            }

            var l = RenderLayerFactory.CreateLayer<TEntity>(layer, tileMatcherModel.DataSets);
            return l.UsingGraphicTags()
                    .WithMatcher(layer.Match, parameters.MatcherFactory, ctx)
                    .Build(dataSets, CreateRenderer(layer, parameters));
        }

        protected ILayer<(TEntity, int)> CreateInternalQuantified(TileMatcherModel tileMatcherModel,
                                                                  RenderLayerModel layer,
                                                                  IRenderLayerProducerConfig<TClassification> parameters)
        {
            if (layer.SubLayers.Count > 0)
            {
                var layers = new List<ILayer<(TEntity, int)>>();
                foreach (var subLayer in layer.SubLayers)
                {
                    layers.Add(CreateInternalQuantified(tileMatcherModel, subLayer, parameters));
                }

                return RenderLayerFactory.CreateCombinedLayer(layer, CreateQuantifiedRenderer(layer, parameters), layers.ToArray());
            }
            
            if (layer.Match == null) throw new ArgumentNullException();
            var ctx = DefaultMatchFactoryContext.From(dataSets, parameters.MapNavigator.BuildNavigator(), parameters.Registry, parameters.TagMetaData);

            if (layer.SubLayers.Count > 0)
            {
                // create a combined layer.
                var layers = layer.SubLayers.Select(l => CreateInternalQuantified(tileMatcherModel, l, parameters)).ToArray();
                return RenderLayerFactory.CreateCombinedLayer(layer, CreateQuantifiedRenderer(layer, parameters), layers);
            }

            var l = RenderLayerFactory.CreateLayer<TEntity>(layer, tileMatcherModel.DataSets);
            return l.Counted()
                    .UsingGraphicTags()
                    .WithMatcher(layer.Match, parameters.MatcherFactory, ctx)
                    .Build(dataSets, CreateQuantifiedRenderer(layer, parameters));
        }
    }
}