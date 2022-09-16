using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.TileMatching.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing
{
    public interface IRenderLayerProducer<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        bool HandlesLayer(RenderLayerModel layer);
        Optional<string> FeatureFlag { get; }
        ILayer Create(TileMatcherModel tileMatcherModel, RenderLayerModel layer, IRenderLayerProducerConfig<TClassification> parameters);
    }

    public static partial class RenderLayerFactory
    {
        public static RenderLayerFactoryWithNavigator DefineFactoryForMap(NavigatorMetaData md) => new RenderLayerFactoryWithNavigator(md);

        public static RenderFactoryData CreateLayer(string layerId)
        {
            return new RenderFactoryData(layerId, RenderingSortOrder.TopDownLeftRight, LayerQueryType.Grid, Array.Empty<IDataSetModel>());
        }

        public static CombinedLayer<TEntityKey> CreateCombinedLayer<TEntityKey>(RenderLayerModel layerModel,
                                                                                ITileRenderer<TEntityKey> renderer,
                                                                                params ILayer<TEntityKey>[]? layers)
        {
            if (layerModel == null)
            {
                throw new ArgumentNullException(nameof(layerModel));
            }

            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            if (layerModel.Id == null) throw new ArgumentException();
            if (layerModel.SubLayers.Count == 0)
            {
                throw new ArgumentException();
            }

            layers ??= Array.Empty<ILayer<TEntityKey>>();
            return new CombinedLayer<TEntityKey>(layerModel.Id,
                                                 layerModel.SortingOrder, renderer, layers);
        }

        public static RenderFactoryData<TEntityKey> CreateLayer<TEntityKey>(RenderLayerModel layerModel, 
                                                                            IReadOnlyList<IDataSetModel> models)
        {
            if (layerModel == null)
            {
                throw new ArgumentNullException(nameof(layerModel));
            }

            if (models == null)
            {
                throw new ArgumentNullException(nameof(models));
            }

            if (layerModel.Id == null) throw new ArgumentException();
            if (layerModel.SubLayers.Count > 0)
            {
                throw new ArgumentException();
            }

            if (layerModel.EntitySource == null) throw new ArgumentException();
            if (layerModel.EntitySource.EntityQueryId == null) throw new ArgumentException();
            return new RenderFactoryData(layerModel.Id,
                                         layerModel.SortingOrder,
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
                                          layerModel.SortingOrder,
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