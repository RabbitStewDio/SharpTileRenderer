using Microsoft.Xna.Framework;
using SharpTileRenderer.Drawing;
using SharpTileRenderer.Drawing.Monogame;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Strategy.Base;
using SharpTileRenderer.Strategy.Base.Map;
using SharpTileRenderer.Strategy.Base.Model;
using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileBlending;
using SharpTileRenderer.TileBlending.Xml;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.Xml.TexturePack;
using System;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Xml.TileMatching;
using System.Collections.Generic;
using Point = Microsoft.Xna.Framework.Point;

namespace SharpTileRenderer.Strategy.MonoGame
{
    public class SimpleGame : Game
    {
        // default resolution is small enough for even the most crappy display in 2022 
        static readonly Point resolution = new Point(1200, 720);
        readonly FrameRateCalculator frameRate;
        readonly StrategyGame game;
        readonly XnaContentLoader contentLoader;
        readonly XnaTextureOperations textureOperations;
        readonly RenderComponent renderComponent;

        public SimpleGame()
        {
            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = resolution.X,
                PreferredBackBufferHeight = resolution.Y,
                SynchronizeWithVerticalRetrace = false
            };

            Window.AllowUserResizing = true;
            IsFixedTimeStep = false;
            frameRate = new FrameRateCalculator(this);
            renderComponent = new RenderComponent(this);
            game = new StrategyGame();

            contentLoader = new XnaContentLoader(Content, Graphics);
            textureOperations = new XnaTextureOperations(Graphics);

            Components.Add(frameRate);
            Components.Add(renderComponent);
        }

        public GraphicsDeviceManager Graphics { get; }

        protected override void Initialize()
        {
            var texturePack = new XmlTexturePackLoader().Load(contentLoader, ContentUri.MakeRelative("tiles.xml"));
            var tileProducer = new TileProducer<XnaTexture>(textureOperations, textureOperations.CreateAtlasBuilder());
            var tileSet = new SpriteTagTileResolver<TexturedTile<XnaTexture>>(texturePack.TileSize).Populate(tileProducer, contentLoader, texturePack);

            Console.WriteLine($"Loaded {tileSet.Count} tiles.");

            // var renderConfig = new YamlTileMatcherModelParser().Load(contentLoader, ContentUri.MakeRelative("renderer.yaml"));
            var renderConfig = new XmlTileMatcherModelParser().ConfigureBuiltInSelectorReadHandlers()
                                                              .ConfigureBuiltInDataSetReadHandlers()
                                                              .ConfigureBlendHandlers()
                                                              .Load(contentLoader, ContentUri.MakeRelative("renderer.xml"));
            var navConfig = game.NavigatorConfig;

            var layers = RenderLayerFactory.DefineFactoryForMap(navConfig)
                                           .WithClassification<EntityClassification32>()
                                           .WithDefaultMatchers()
                                           //.WithFeature(XnaTextureTileModule.For(Graphics).WithTileSet(tileSet).WithDebugRenderer(Content.Load<SpriteFont>("Fonts/DialogTiny")))
                                           .WithFeature(XnaTextureTileModule.For(Graphics).WithTileSet(tileSet))
                                           .WithFeature(new TextureBlendingTileModule())
                                           .PrepareForData()
                                           .WithDataSets(CreateDataSetHandler(renderConfig))
                                           .WithDataSets(CreateSettlementDataSet(renderConfig))
                                           .ProduceLayers(renderConfig);

            renderComponent.ViewPort = new ViewPort(navConfig, texturePack.TileShape, texturePack.TileSize);
            renderComponent.SetLayers(layers);
            renderComponent.ViewPort.Focus = new VirtualMapCoordinate(0, 2);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            game.Update(0, 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        protected ITileDataSetProducer<Settlement?> CreateSettlementDataSet(TileMatcherModel tileMatcherModel)
        {
            var gameDataSets = new StrategyGameDataSets(game, tileMatcherModel);

            var ds = new DefaultTileDataSetProducer<Settlement?>();
            ds.WithQuantifiedDataSet("cities", gameDataSets.CreateCitiesDataSet);
            return ds;
        }

        protected ITileDataSetProducer<Unit> CreateDataSetHandler(TileMatcherModel tileMatcherModel)
        {
            var gameDataSets = new StrategyGameDataSets(game, tileMatcherModel);

            var ds = new DefaultTileDataSetProducer<Unit>();
            ds.WithDataSet("terrain", gameDataSets.CreateTerrainDataSet);
            ds.WithDataSet("rivers", gameDataSets.CreateRiversDataSet);
            ds.WithDataSet("improvements", gameDataSets.CreateImprovementsDataSet);
            ds.WithDataSet("resources", gameDataSets.CreateResourceDataSet);
            ds.WithDataSet("roads", gameDataSets.CreateRoadDataSet);
            ds.WithDataSet("fog", gameDataSets.CreateFogDataSet);
            return ds;
        }
    }

    public class StrategyGameDataSets
    {
        readonly StrategyGame game;
        readonly Dictionary<TerrainId, GraphicTag> terrainGraphicMap;

        public StrategyGameDataSets(StrategyGame game, TileMatcherModel tileMatcherModel)
        {
            if (tileMatcherModel == null)
            {
                throw new ArgumentNullException(nameof(tileMatcherModel));
            }

            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.terrainGraphicMap = new Dictionary<TerrainId, GraphicTag>();

            var tagMap = new HashSet<string>();
            foreach (var tag in tileMatcherModel.Tags)
            {
                if (!string.IsNullOrEmpty(tag.Id))
                {
                    tagMap.Add(tag.Id);
                }
            }
            
            foreach (var terrainId in game.GameData.Rules.TerrainTypes)
            {
                if (!game.GameData.Rules.TerrainTypes.TryGetValue(terrainId, out var terrain))
                {
                    continue;
                }

                if (TryFindFirstValidGraphicTag(terrain, tagMap, out var tag))
                {
                    terrainGraphicMap[terrainId] = tag;
                }
            }
        }

        bool TryFindFirstValidGraphicTag(IRuleElement re, ICollection<string> validTags, out GraphicTag tag)
        {
            foreach (var graphicTag in re.AllGraphicTags())
            {
                if (!string.IsNullOrEmpty(graphicTag) && validTags.Contains(graphicTag))
                {
                    tag = GraphicTag.From(graphicTag);
                    return true;
                }
            }

            tag = default;
            return false;
        }
        
        public ITileDataSet<GraphicTag, Unit> CreateTerrainDataSet()
        {
            GraphicTag TerrainToGraphicTag(TerrainData f)
            {
                var resource = f.TerrainIdx;
                if (terrainGraphicMap.TryGetValue(resource, out var result))
                {
                    return result;
                }

                return GraphicTag.Empty;
            }

            return new DefaultMapTileDataSet<TerrainData, Unit>(game.GameData.Terrain, TerrainToGraphicTag,
                                                                _ => new Unit());
        }

        public ITileDataSet<GraphicTag, Unit> CreateRiversDataSet()
        {
            GraphicTag RiverToGraphicTag(TerrainData f)
            {
                if ((f.RoadsAndRiver & RoadTypeId.River) == RoadTypeId.River) return GraphicTag.From("river");
                if (game.GameData.Rules.TerrainTypes.TryGetValue(f.TerrainIdx, out var terrain))
                {
                    if (terrain.Class == TerrainClass.Water)
                    {
                        return GraphicTag.From("ocean");
                    }
                }

                return GraphicTag.From("land");
            }

            return new DefaultMapTileDataSet<TerrainData, Unit>(game.GameData.Terrain, RiverToGraphicTag,
                                                                _ => new Unit());
        }

        public ITileDataSet<GraphicTag, Unit> CreateResourceDataSet()
        {
            GraphicTag ResourceToGraphicTag(TerrainData f)
            {
                var resource = f.Resources;
                if (game.GameData.Rules.TerrainResourceTypes[resource].TryGetValue(out var value) &&
                    !string.IsNullOrEmpty(value.GraphicTag))
                {
                    return GraphicTag.From(value.GraphicTag);
                }

                return GraphicTag.Empty;
            }

            return new DefaultMapTileDataSet<TerrainData, Unit>(game.GameData.Terrain, ResourceToGraphicTag,
                                                                _ => new Unit());
        }

        public ITileDataSet<GraphicTag, Unit> CreateImprovementsDataSet()
        {
            GraphicTag ImprovementToGraphicTag(TerrainData f)
            {
                var resource = f.Improvement;
                if (game.GameData.Rules.TerrainImprovementTypes[resource].TryGetValue(out var value))
                {
                    return GraphicTag.From(value.GraphicTag);
                }

                return GraphicTag.Empty;
            }

            return new DefaultMapTileDataSet<TerrainData, Unit>(game.GameData.Terrain, ImprovementToGraphicTag,
                                                                _ => new Unit());
        }

        public ITileDataSet<GraphicTag, Unit> CreateRoadDataSet()
        {
            static GraphicTag RoadToGraphicTag(TerrainData f)
            {
                if ((f.RoadsAndRiver & RoadTypeId.Railroad) == RoadTypeId.Railroad) return GraphicTag.From("roads.railroad");
                if ((f.RoadsAndRiver & RoadTypeId.Road) == RoadTypeId.Road) return GraphicTag.From("roads.road");
                return GraphicTag.Empty;
            }

            return new DefaultMapTileDataSet<TerrainData, Unit>(game.GameData.Terrain, RoadToGraphicTag,
                                                                _ => new Unit());
        }

        public ITileDataSet<GraphicTag, Unit> CreateFogDataSet()
        {
            static GraphicTag FogToGraphicTag(FogState f)
            {
                if ((f & FogState.Visible) == FogState.Visible) return GraphicTag.From("fog.known");
                if ((f & FogState.Explored) == FogState.Explored) return GraphicTag.From("fog.fog");
                return GraphicTag.From("fog.unknown");
            }

            return new DefaultMapTileDataSet<FogState, Unit>(game.FogData.Fog, FogToGraphicTag,
                                                             _ => new Unit());
        }

        public IQuantifiedTagTileDataSet<GraphicTag, Settlement?, int> CreateCitiesDataSet()
        {
            GraphicTag ConvertSettlement(ISettlement? s)
            {
                if (s == null) return GraphicTag.Empty;
                var culture = game.GameData.Players[s.Owner].Culture;
                return s.Walled switch
                {
                    true => culture switch
                    {
                        Culture.Asian => GraphicTag.From("city.asian_wall"),
                        Culture.Tropical => GraphicTag.From("city.tropical_wall"),
                        Culture.Celtic => GraphicTag.From("city.celtic_wall"),
                        Culture.Classical => GraphicTag.From("city.classical_wall"),
                        Culture.Babylonian => GraphicTag.From("city.babylonian_wall"),
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    false => culture switch
                    {
                        Culture.Asian => GraphicTag.From("city.asian_city"),
                        Culture.Tropical => GraphicTag.From("city.tropical_city"),
                        Culture.Celtic => GraphicTag.From("city.celtic_city"),
                        Culture.Classical => GraphicTag.From("city.classical_city"),
                        Culture.Babylonian => GraphicTag.From("city.babylonian_city"),
                        _ => throw new ArgumentOutOfRangeException()
                    }
                };
            }

            static (Settlement?, int) ClassifyPopulation(Settlement? s)
            {
                if (s == null)
                {
                    return (null, 0);
                }

                return s.Population switch
                {
                    <= 1000 => (s, 1),
                    <= 5000 => (s, 2),
                    <= 50000 => (s, 3),
                    _ => (s, 4)
                };
            }

            return new DefaultQuantifiedMapTileDataSet<Settlement?, Settlement?>(game.GameData.Settlements,
                                                                                 ConvertSettlement,
                                                                                 ClassifyPopulation);
        }
    }
}