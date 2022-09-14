using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Drawing.TileResolvers;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.TileMatching.Selectors;
using System;

namespace SharpTileRenderer.Drawing
{
    public static partial class RenderLayerFactory
    {
        public readonly struct QuantifiedRenderFactoryData<TEntity>
        {
            readonly RenderFactoryData coreData;
            readonly string entityQuery;

            public QuantifiedRenderFactoryData(RenderFactoryData coreData, string entityQuery)
            {
                this.coreData = coreData;
                this.entityQuery = entityQuery ?? throw new ArgumentNullException(nameof(entityQuery));
            }

            public QuantifiedTagRenderFactoryData<TEntity> UsingGraphicTags()
            {
                return new QuantifiedTagRenderFactoryData<TEntity>(coreData, entityQuery);
            }
/*
            public QuantifiedClassRenderFactoryData<TEntity, TClasses> UsingClasses<TClasses>(EntityClassificationRegistry<TClasses> registry)
                where TClasses : struct, IEntityClassification<TClasses>
            {
                return new QuantifiedClassRenderFactoryData<TEntity, TClasses>(coreData, registry, entityQuery);
            }
            */
        }


        public readonly struct QuantifiedTagRenderFactoryData<TEntity>
        {
            readonly RenderFactoryData coreData;
            readonly string entityQuery;

            public QuantifiedTagRenderFactoryData(RenderFactoryData coreData,
                                                  string entityQuery)
            {
                this.coreData = coreData;
                this.entityQuery = entityQuery;
            }

            public QuantifiedTagRenderFactoryDataWithMatcher<TEntity> WithMatcher(ISpriteMatcher<(GraphicTag, int)> spriteMatcher)
            {
                return new QuantifiedTagRenderFactoryDataWithMatcher<TEntity>(coreData, spriteMatcher, entityQuery);
            }

            public QuantifiedTagRenderFactoryDataWithMatcher<TEntity> WithMatcher<TClass>(ISelectorModel model,
                                                                                          IMatcherFactory<TClass> matchFactory,
                                                                                          IMatchFactoryContext<TClass> context)
                where TClass : struct, IEntityClassification<TClass>
            {
                return new QuantifiedTagRenderFactoryDataWithMatcher<TEntity>(coreData, matchFactory.CreateQuantifiedTagMatcher(model, context), entityQuery);
            }
        }

        public readonly struct QuantifiedTagRenderFactoryDataWithMatcher<TEntity>
        {
            readonly RenderFactoryData coreData;
            readonly ISpriteMatcher<(GraphicTag, int)> spriteMatcher;
            readonly string entityQuery;

            public QuantifiedTagRenderFactoryDataWithMatcher(RenderFactoryData coreData,
                                                             ISpriteMatcher<(GraphicTag, int)> spriteMatcher,
                                                             string entityQuery)
            {
                this.coreData = coreData;
                this.spriteMatcher = spriteMatcher;
                this.entityQuery = entityQuery;
            }

            public ILayer<(TEntity, int)> Build(ITileDataSetProducer<TEntity> p,
                                                ITileRenderer<TEntity> renderer)
            {
                return Build(p, new StripQuantityTileRenderer<TEntity, int>(renderer));
            }

            public ILayer<(TEntity, int)> Build(ITileDataSetProducer<TEntity> p,
                                                ITileRenderer<(TEntity, int)> renderer)
            {
                if (!FindDataSourceDefinition(coreData.Models, entityQuery).TryGetValue(out var model))
                {
                    throw new ArgumentException($"Unable to find a declared model data source with id '${entityQuery}'");
                }

                var dataSourceType = model.Kind;
                var primaryDataSet = dataSourceType switch
                {
                    DataSetType.QuantifiedTagMap => p.CreateCountedGraphicDataSet(entityQuery),
                    _ => throw new ArgumentOutOfRangeException()
                };

                ILayerTileResolver<GraphicTag, (TEntity, int)> tr = new QuantifiedDirectLayerTileResolver<TEntity, int>(spriteMatcher);
                return coreData.LayerQueryType switch
                {
                    LayerQueryType.Grid => new GridLayer<GraphicTag, (TEntity, int)>(coreData.LayerId, tr, primaryDataSet, coreData.SortOrder, renderer),
                    LayerQueryType.Sparse => new SparseLayer<GraphicTag, (TEntity, int)>(coreData.LayerId, tr, primaryDataSet, coreData.SortOrder, renderer),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
/*
        public readonly struct QuantifiedClassRenderFactoryData<TEntity, TClassificationType>
            where TClassificationType : struct, IEntityClassification<TClassificationType>
        {
            readonly RenderFactoryData coreData;
            readonly EntityClassificationRegistry<TClassificationType> registry;
            readonly string primaryQuery;

            public QuantifiedClassRenderFactoryData(RenderFactoryData coreData,
                                                    EntityClassificationRegistry<TClassificationType> registry,
                                                    string primaryQuery)
            {
                this.coreData = coreData;
                this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
                this.primaryQuery = primaryQuery ?? throw new ArgumentNullException(nameof(primaryQuery));
            }


            public QuantifiedClassRenderFactoryDataWithMatcher<TEntity, TClassificationType> WithMatcher(ISpriteMatcher<(TClassificationType, int)> spriteMatcher)
            {
                return new QuantifiedClassRenderFactoryDataWithMatcher<TEntity, TClassificationType>(coreData, registry, primaryQuery, spriteMatcher);
            }
        }

        public readonly struct QuantifiedClassRenderFactoryDataWithMatcher<TEntity, TClassificationType>
            where TClassificationType : struct, IEntityClassification<TClassificationType>
        {
            readonly RenderFactoryData coreData;
            readonly EntityClassificationRegistry<TClassificationType> registry;
            readonly string primaryQuery;
            readonly ISpriteMatcher<(TClassificationType, int)> spriteMatcher;

            public QuantifiedClassRenderFactoryDataWithMatcher(RenderFactoryData coreData,
                                                               EntityClassificationRegistry<TClassificationType> registry,
                                                               string primaryQuery,
                                                               ISpriteMatcher<(TClassificationType, int)> spriteMatcher)
            {
                this.coreData = coreData;
                this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
                this.primaryQuery = primaryQuery ?? throw new ArgumentNullException(nameof(primaryQuery));
                this.spriteMatcher = spriteMatcher;
            }

            public ILayer Build(ITileDataSetProducer<TEntity, TClassificationType> p,
                                ITileRenderer<TEntity> renderer)
            {
                return Build(p, new StripQuantityTileRenderer<TEntity, int>(renderer));
            }

            public ILayer Build(ITileDataSetProducer<TEntity, TClassificationType> p,
                                ITileRenderer<(TEntity, int)> renderer)
            {
                if (!FindDataSourceDefinition(coreData.Models, primaryQuery).TryGetValue(out var model))
                {
                    throw new ArgumentException($"Unable to find a declared model data source with id '${primaryQuery}'");
                }

                var dataSourceType = model.Kind;
                var primaryDataSet = dataSourceType switch
                {
                    DataSetType.QuantifiedClassSet => p.CreateCountedClassDataSet(primaryQuery),
                    _ => throw new ArgumentOutOfRangeException()
                };

                var tr = new QuantifiedClassificationLayerTileResolver<TClassificationType, TEntity, int>(spriteMatcher);
                return coreData.LayerQueryType switch
                {
                    LayerQueryType.Grid => new GridLayer<TClassificationType, (TEntity, int)>(coreData.LayerId, tr, primaryDataSet, coreData.SortOrder, renderer),
                    LayerQueryType.Sparse => new SparseLayer<TClassificationType, (TEntity, int)>(coreData.LayerId, tr, primaryDataSet, coreData.SortOrder, renderer),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        */
    }
}