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
        public readonly struct RenderFactoryData<TEntity>
        {
            readonly RenderFactoryData coreData;
            readonly string entityQuery;

            public RenderFactoryData(RenderFactoryData coreData, string entityQuery)
            {
                this.coreData = coreData;
                this.entityQuery = entityQuery ?? throw new ArgumentNullException(nameof(entityQuery));
            }

            public QuantifiedRenderFactoryData<TEntity> Counted() => new QuantifiedRenderFactoryData<TEntity>(coreData, entityQuery);

            public TagRenderFactoryData<TEntity> UsingGraphicTags()
            {
                return new TagRenderFactoryData<TEntity>(coreData, entityQuery);
            }
/*
            public ClassRenderFactoryData<TEntity, TClasses> UsingClasses<TClasses>(EntityClassificationRegistry<TClasses> registry)
                where TClasses : struct, IEntityClassification<TClasses>
            {
                return new ClassRenderFactoryData<TEntity, TClasses>(coreData, registry, entityQuery);
            }
*/
        }

        public readonly struct TagRenderFactoryData<TEntity>
        {
            readonly RenderFactoryData coreData;
            readonly string entityQuery;

            public TagRenderFactoryData(RenderFactoryData coreData, string entityQuery)
            {
                this.coreData = coreData;
                this.entityQuery = entityQuery;
            }

            public TagRenderFactoryDataWithMatcher<TEntity> WithMatcher(ISpriteMatcher<GraphicTag> spriteMatcher)
            {
                return new TagRenderFactoryDataWithMatcher<TEntity>(coreData, spriteMatcher, entityQuery);
            }

            public TagRenderFactoryDataWithMatcher<TEntity> WithMatcher<TClass>(ISelectorModel model,
                                                                                IMatcherFactory<TClass> matchFactory,
                                                                                IMatchFactoryContext<TClass> context)
                where TClass : struct, IEntityClassification<TClass>
            {
                return new TagRenderFactoryDataWithMatcher<TEntity>(coreData, matchFactory.CreateTagMatcher(model, context), entityQuery);
            }
        }

        public readonly struct TagRenderFactoryDataWithMatcher<TEntity>
        {
            readonly RenderFactoryData coreData;
            readonly ISpriteMatcher<GraphicTag> spriteMatcher;
            readonly string primaryQuery;

            public TagRenderFactoryDataWithMatcher(RenderFactoryData coreData,
                                                   ISpriteMatcher<GraphicTag> spriteMatcher,
                                                   string primaryQuery)
            {
                this.coreData = coreData;
                this.spriteMatcher = spriteMatcher;
                this.primaryQuery = primaryQuery;
            }

            public ILayer<TEntity> Build(ITileDataSetProducer<TEntity> p,
                                         ITileRenderer<(TEntity, int)> renderer,
                                         int quantity = 1)
            {
                return Build(p, new EnrichQuantityTileRenderer<TEntity, int>(renderer, quantity));
            }

            public ILayer<TEntity> Build(ITileDataSetProducer<TEntity> p,
                                         ITileRenderer<TEntity> renderer)
            {
                if (!FindDataSourceDefinition(coreData.Models, primaryQuery).TryGetValue(out var model))
                {
                    throw new ArgumentException($"Unable to find a declared model data source with id '${primaryQuery}'");
                }

                var dataSourceType = model.Kind;
                var primaryDataSet = dataSourceType switch
                {
                    DataSetType.TagMap => p.CreateGraphicDataSet(primaryQuery),
                    DataSetType.QuantifiedTagMap => p.CreateCountedGraphicDataSet(primaryQuery).Downgrade(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                ILayerTileResolver<GraphicTag, TEntity> tr = new DirectLayerTileResolver<TEntity>(spriteMatcher);
                return coreData.LayerQueryType switch
                {
                    LayerQueryType.Grid => new GridLayer<GraphicTag, TEntity>(coreData.LayerId, tr, primaryDataSet, coreData.SortOrder, renderer),
                    LayerQueryType.Sparse => new SparseLayer<GraphicTag, TEntity>(coreData.LayerId, tr, primaryDataSet, coreData.SortOrder, renderer),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
/*
        public readonly struct ClassRenderFactoryData<TEntity, TClassificationType>
            where TClassificationType : struct, IEntityClassification<TClassificationType>
        {
            readonly RenderFactoryData coreData;
            readonly EntityClassificationRegistry<TClassificationType> registry;
            readonly string entityQuery;

            public ClassRenderFactoryData(RenderFactoryData coreData, EntityClassificationRegistry<TClassificationType> registry, string entityQuery)
            {
                this.coreData = coreData;
                this.registry = registry;
                this.entityQuery = entityQuery;
            }

            public ClassRenderFactoryDataWithMatcher<TEntity, TClassificationType> WithMatcher(ISpriteMatcher<TClassificationType> spriteMatcher)
            {
                return new ClassRenderFactoryDataWithMatcher<TEntity, TClassificationType>(coreData, registry, spriteMatcher, entityQuery);
            }
        }

        public readonly struct ClassRenderFactoryDataWithMatcher<TEntity, TClassificationType>
            where TClassificationType : struct, IEntityClassification<TClassificationType>
        {
            readonly RenderFactoryData coreData;
            readonly EntityClassificationRegistry<TClassificationType> registry;
            readonly ISpriteMatcher<TClassificationType> spriteMatcher;
            readonly string primaryQuery;

            public ClassRenderFactoryDataWithMatcher(RenderFactoryData coreData, 
                                                     EntityClassificationRegistry<TClassificationType> registry,
                                                     ISpriteMatcher<TClassificationType> spriteMatcher,
                                                     string primaryQuery)
            {
                this.coreData = coreData;
                this.registry = registry;
                this.spriteMatcher = spriteMatcher;
                this.primaryQuery = primaryQuery;
            }

            public ILayer Build(ITileDataSetProducer<TEntity, TClassificationType> p,
                                ITileRenderer<(TEntity, int)> renderer,
                                int quantity = 1)
            {
                return Build(p, new EnrichQuantityTileRenderer<TEntity, int>(renderer, quantity));
            }
            
            public ILayer Build(ITileDataSetProducer<TEntity, TClassificationType> p,
                                ITileRenderer<TEntity> renderer)
            {
                if (!FindDataSourceDefinition(coreData.Models, primaryQuery).TryGetValue(out var model))
                {
                    throw new ArgumentException($"Unable to find a declared model data source with id '${primaryQuery}'");
                }

                var dataSourceType = model.Kind;
                var primaryDataSet = dataSourceType switch
                {
                    DataSetType.QuantifiedClassSet => p.CreateCountedClassDataSet(primaryQuery).Downgrade(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                var tr = new ClassificationLayerTileResolver<TClassificationType, TEntity>(spriteMatcher);
                return coreData.LayerQueryType switch
                {
                    LayerQueryType.Grid => new GridLayer<TClassificationType, TEntity>(coreData.LayerId, tr, primaryDataSet, coreData.SortOrder, renderer),
                    LayerQueryType.Sparse => new SparseLayer<TClassificationType, TEntity>(coreData.LayerId, tr, primaryDataSet, coreData.SortOrder, renderer),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        */
    }
}