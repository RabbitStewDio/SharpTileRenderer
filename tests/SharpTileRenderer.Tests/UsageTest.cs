using SharpTileRenderer.Drawing;
using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.Tests.Fixtures;
using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TexturePack.Model;
using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.EntitySources;
using SharpTileRenderer.TileMatching.Selectors;
using SharpTileRenderer.Yaml.TexturePack;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Tests
{
    public class UsageTest
    {
        public readonly struct LayerFactoryParameters<TClassification, TEntity>
            where TClassification : struct, IEntityClassification<TClassification>
        {
            public readonly IReadOnlyList<IDataSetModel> DataSetModels;
            public readonly IMatcherFactory<TClassification> MatcherFactory;
            public readonly IMatchFactoryContext<TClassification> MatcherFactoryContext;
            public readonly ITileRenderer<TEntity> Renderer;
            public readonly ITileDataSetProducer<TEntity> DataSets;

            public LayerFactoryParameters(IReadOnlyList<IDataSetModel> dataSetModels, 
                                          IMatcherFactory<TClassification> matcherFactory, 
                                          IMatchFactoryContext<TClassification> matcherFactoryContext, 
                                          ITileRenderer<TEntity> renderer, 
                                          ITileDataSetProducer<TEntity> dataSets)
            {
                DataSetModels = dataSetModels;
                MatcherFactory = matcherFactory;
                MatcherFactoryContext = matcherFactoryContext;
                Renderer = renderer;
                DataSets = dataSets;
            }
        }

        public ILayer CreateLayerInGenericFashion<TClassification, TEntity>(RenderLayerModel layer,
                                                                            LayerFactoryParameters<TClassification, TEntity> parameters)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (layer.Match == null) throw new ArgumentNullException();

            var l = RenderLayerFactory.CreateLayer<TEntity>(layer, parameters.DataSetModels);
            if (layer.Match.IsQuantifiedSelector)
            {
                return l.Counted()
                        .UsingGraphicTags()
                        .WithMatcher(layer.Match, parameters.MatcherFactory, parameters.MatcherFactoryContext)
                        .Build(parameters.DataSets, parameters.Renderer);
            }

            return l.UsingGraphicTags()
                    .WithMatcher(layer.Match, parameters.MatcherFactory, parameters.MatcherFactoryContext)
                    .Build(parameters.DataSets, parameters.Renderer);
        }

        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void SampleUsage()
        {
            // provided via the global infrastructure
            var contentLoader = new TestContentLoader();

            // tiles are what is rendered. This is somewhat independent from the rest of the library
            // and builds a repository of renderable elements. 
            var tiles = LoadTileSet(contentLoader);
            var tileProducer = new TileProducer<Texture>(null!);
            var spriteTiles = new SpriteTagTileResolver<TexturedTile<Texture>>(default);
            spriteTiles.Populate(tileProducer, contentLoader, tiles);
            var renderer = new TestRenderer(spriteTiles);

            
            // external data needed to produce the tile matcher
            // - connects into the underlying data structures
            var navConfig = NavigatorMetaData.FromGridType(GridType.Grid).WithHorizontalLimit(100).WithVerticalLimit(100);
            var dataSets = CreateDataSetProducer();
            var classReg = new EntityClassificationRegistry<EntityClassification32>();
            var tagMetaData = new GraphicTagMetaDataRegistry<EntityClassification32>(classReg);

            // Loaded from YAML or XML            
            var model = CreateTileMatcherModel();

            // Boilerplate
            var matcherFactory = new MatcherFactory<EntityClassification32>().WithDefaultMatchers();
            var matcherFactoryContext = DefaultMatchFactoryContext.From(dataSets, navConfig.BuildNavigator(), classReg, tagMetaData);

            var parameters = new LayerFactoryParameters<EntityClassification32, TestEntityKey>(model.DataSets, matcherFactory,
                                                                                               matcherFactoryContext, renderer, dataSets);
            model.RenderLayers.Select(l => CreateLayerInGenericFashion(l, parameters));
            var (layer, layer2) = CreateLayersManually(parameters, model.RenderLayers);

            var viewPort = new ViewPort(navConfig, tiles.TileShape, tiles.TileSize)
            {
                PixelBounds = new ScreenBounds(0, 0, 640, 480),
                Focus = new VirtualMapCoordinate(10, 10)
            };

            var viewPortRendering = new ViewportRendering(viewPort);
            viewPortRendering.Render(layer, layer2);
        }

        static ITileCollection LoadTileSet(IContentLoader l)
        {
            var fileName = "texture-pack.yaml";
            TileCollectionPack texturePackMeta = new YamlTexturePackModelParser().Load(l, ContentUri.MakeRelative(fileName));
            return texturePackMeta;
        }
        
        static (ILayer layer, ILayer layer2) CreateLayersManually(in LayerFactoryParameters<EntityClassification32, TestEntityKey> parameters, 
                                                                  IReadOnlyList<RenderLayerModel> renderLayers)
        {
            IMatcherFactory<EntityClassification32> matcherFactory = parameters.MatcherFactory;
            IMatchFactoryContext<EntityClassification32> matcherFactoryContext = parameters.MatcherFactoryContext;
            ITileDataSetProducer<TestEntityKey> dataSets = parameters.DataSets;
            IReadOnlyList<IDataSetModel> dataSetModels = parameters.DataSetModels;
            ITileRenderer<TestEntityKey> renderer = parameters.Renderer;
                
            var layerZero = renderLayers[0];
            var layer = RenderLayerFactory.CreateLayer<TestEntityKey>(layerZero, dataSetModels)
                                          .UsingGraphicTags()
                                          .WithMatcher(layerZero.Match, matcherFactory, matcherFactoryContext)
                                          .Build(dataSets, renderer);
            // manual mode
            var layerOne = renderLayers[1];
            var layer2 = RenderLayerFactory.CreateLayer(layerOne.Id)
                                           .WithAvailableDataSets(dataSetModels)
                                           .WithQueryType(layerOne.EntitySource.LayerQueryType)
                                           .WithRenderOrder(layerOne.EntitySource.SortingOrder)
                                           .WithEntityData<TestEntityKey>(layerOne.EntitySource.EntityQueryId)
                                           .Counted()
                                           .UsingGraphicTags()
                                           .WithMatcher(layerOne.Match, matcherFactory, matcherFactoryContext)
                                           .Build(dataSets, renderer);
            return (layer, layer2);
        }

        static TileMatcherModel CreateTileMatcherModel()
        {
            var model = new TileMatcherModel()
                        {
                            Author = "Me",
                            Documentation = "Test data",
                            Version = "1.0.0"
                        }
                        .WithDataSet(new TagDataSetModel()
                                         {
                                             Id = "SomeTagData"
                                         }
                                         .WithProperty("key", "value")
                        )
                        .WithDataSet(new ClassSetDataSetModel()
                                         {
                                             Id = "SomeClassData"
                                         }
                                         .WithProperty("key", "value")
                        )
                        .WithDataSet(new QuantifiedClassSetDataSetModel()
                                         {
                                             Id = "SomeClassData",
                                             Classes = { "road", "railway" },
                                             DefaultQuantity = 1
                                         }
                                         .WithProperty("key", "value")
                        )
                        .WithRenderLayer(new RenderLayerModel()
                            {
                                Id = "SomeLayer",
                                EntitySource = new EntitySourceModel()
                                {
                                    EntityQueryId = "SomeTagData",
                                    LayerQueryType = LayerQueryType.Grid,
                                    SortingOrder = RenderingSortOrder.TopDownLeftRight
                                },
                                Match = null
                            }
                        )
                        .WithRenderLayer(new RenderLayerModel()
                            {
                                Id = "SomeCountedLayer",
                                EntitySource = new EntitySourceModel()
                                {
                                    EntityQueryId = "SomeTagData",
                                    LayerQueryType = LayerQueryType.Grid,
                                    SortingOrder = RenderingSortOrder.TopDownLeftRight
                                },
                                Match = null
                            }
                        );
            return model;
        }

        ITileDataSetProducer<TestEntityKey> CreateDataSetProducer()
        {
            return new DefaultTileDataSetProducer<TestEntityKey>()
                   .WithDataSet("SomeTagData", ArrayDataSet.CreateBasicTagDataSet<TestEntityKey>(10, 10))
                   .WithDataSet("SomeClassData", ArrayDataSet.CreateBasicTagDataSet<TestEntityKey>(10, 10));
        }
    }

    public class TestRenderer : ITileRenderer<TestEntityKey>
    {
        readonly SpriteTagTileResolver<TexturedTile<Texture>> spriteTiles;

        public TestRenderer(SpriteTagTileResolver<TexturedTile<Texture>> spriteTiles)
        {
            this.spriteTiles = spriteTiles;
        }

        public ValueTask RenderBatchAsync(IViewPort vp,
                                          List<ScreenRenderInstruction<TestEntityKey>> renderInstructionBuffer,
                                          CancellationToken cancellationToken)
        {
            RenderBatch(vp, renderInstructionBuffer);
            return new ValueTask(Task.CompletedTask);
        }

        public void RenderBatch(IViewPort vp, List<ScreenRenderInstruction<TestEntityKey>> renderInstructionBuffer)
        {
        }
    }

    class TestContentLoader : FileContentLoader, IContentLoader<Texture>
    {
        public Texture LoadTexture(ContentUri name)
        {
            return new Texture()
            {
                Name = name.ToString(),
                Bounds = new TextureCoordinateRect(0, 0, 2048, 2048)
            };
        }
    }

    public class Texture : ITexture<Texture>
    {
        public string Name { get; set; }
        public TextureCoordinateRect Bounds { get; set; }
        public bool Valid { get; } = true;
        
        public Texture CreateSubTexture(string name, TextureCoordinateRect bounds)
        {
            throw new NotImplementedException();
        }
    }
}