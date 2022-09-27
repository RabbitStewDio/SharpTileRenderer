using Microsoft.Xna.Framework;
using SharpTileRenderer.Drawing;
using SharpTileRenderer.Drawing.Monogame;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.RPG.Base;
using SharpTileRenderer.RPG.Base.Map;
using SharpTileRenderer.RPG.Base.Model;
using SharpTileRenderer.RPG.Base.Util;
using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileBlending.Xml;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Xml.TexturePack;
using SharpTileRenderer.Xml.TileMatching;
using System;
using System.Collections.Generic;
using Point = Microsoft.Xna.Framework.Point;

namespace SharpTileRenderer.RPG.MonoGame
{
    public class SimpleGame : Game
    {
        // default resolution is small enough for even the most crappy display in 2022 
        static readonly Point resolution = new Point(1200, 720);
        readonly FrameRateCalculator frameRate;
        readonly DungeonGame game;
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

            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            IsFixedTimeStep = false;
            frameRate = new FrameRateCalculator(this);
            renderComponent = new RenderComponent(this);
            game = new DungeonGame();

            contentLoader = new XnaContentLoader(Content, Graphics);
            textureOperations = new XnaTextureOperations(Graphics);

            Components.Add(frameRate);
            Components.Add(renderComponent);
        }

        public GraphicsDeviceManager Graphics { get; }

        protected override void Initialize()
        {
            var texturePack = new XmlTexturePackLoader().Load(contentLoader, ContentUri.MakeRelative("Tiles/Rpg/tileset.xml"));
            var tileProducer = new TileProducer<XnaTexture>(textureOperations, textureOperations.CreateAtlasBuilder());
            var tileSet = new SpriteTagTileResolver<TexturedTile<XnaTexture>>(texturePack.TileSize).Populate(tileProducer, contentLoader, texturePack);

            Console.WriteLine($"Loaded {tileSet.Count} tiles.");

            // var renderConfig = new YamlTileMatcherModelParser().Load(contentLoader, ContentUri.MakeRelative("renderer.yaml"));
            var renderConfig = new XmlTileMatcherModelParser().ConfigureBuiltInSelectorReadHandlers()
                                                              .ConfigureBuiltInDataSetReadHandlers()
                                                              .ConfigureBlendHandlers()
                                                              .Load(contentLoader, ContentUri.MakeRelative("renderer.xml"));
            var navConfig = game.NavigatorConfig;
            var dataSets = new DungeonGameDataSets(game, renderConfig);

            var layers = RenderLayerFactory.DefineFactoryForMap(navConfig)
                                           .WithClassification<EntityClassification32>()
                                           .WithDefaultMatchers()
                                           .WithFeature(XnaRendererFeatureModule.For(Graphics).WithTileSet(tileSet))
                                           .PrepareForData()
                                           .WithDataSets(dataSets.CreateTerrainDataSet())
                                           .WithDataSets(dataSets.CreateItemDataSet())
                                           .ProduceLayers(renderConfig);

            renderComponent.ViewPort = new ViewPort(navConfig, texturePack.TileShape, texturePack.TileSize);
            renderComponent.SetLayers(layers);
            renderComponent.ViewPort.Focus = new VirtualMapCoordinate(0, 2);

            Components.Add(new GameUI(this, renderComponent));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            game.Update(new DungeonGameTime((float)gameTime.TotalGameTime.TotalSeconds, (float)gameTime.ElapsedGameTime.TotalSeconds));
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }

    public class DungeonGameDataSets
    {
        readonly DungeonGame game;
        readonly Dictionary<NPCRuleElement, GraphicTag> npcGraphics;
        readonly Dictionary<ItemElement, GraphicTag> itemGraphics;
        readonly Dictionary<TerrainElement, GraphicTag> terrainGraphics;

        public DungeonGameDataSets(DungeonGame game, TileMatcherModel tileMatcherModel)
        {
            this.game = game;
            var rules = game.GameRules;
            this.npcGraphics = CreateGraphicMap(tileMatcherModel, rules.Actors);
            this.itemGraphics = CreateGraphicMap(tileMatcherModel, rules.Items);
            this.terrainGraphics = CreateGraphicMap(tileMatcherModel, rules.Terrains);
        }

        public ITileDataSetProducer<TerrainElement?> CreateTerrainDataSet()
        {
            var ds = new DefaultTileDataSetProducer<TerrainElement?>();
            ds.WithDataSet("terrain", CreateTerrainDataSetInternal);
            return ds;
        }

        public ITileDataSetProducer<Unit> CreateItemDataSet()
        {
            var ds = new DefaultTileDataSetProducer<Unit>();
            ds.WithDataSet("items", CreateItemDataSetInternal);
            ds.WithDataSet("actors", CreateActorDataSetInternal);
            return ds;
        }

        ITileDataSet<GraphicTag, TerrainElement?> CreateTerrainDataSetInternal()
        {
            return new DefaultMapTileDataSet<TerrainElement, TerrainElement?>(game.GameData.Terrain, TerrainTagMapping, t => t);
        }

        GraphicTag TerrainTagMapping(TerrainElement t)
        {
            if (terrainGraphics.TryGetValue(t, out var v))
            {
                return v;
            }

            return default;
        }

        ITileDataSet<GraphicTag, Unit> CreateItemDataSetInternal()
        {
            return new DefaultMapTileDataSet<ItemElement, Unit>(game.GameData.Items, t => itemGraphics[t], _ => default);
        }

        ITileDataSet<GraphicTag, Unit> CreateActorDataSetInternal()
        {
            return new DefaultSparseMapDataSet<Actor, Unit>(game.GameData.Actors, t => t.Position, t => npcGraphics[t.RuleData], _ => default);
        }

        Dictionary<TRuleElement, GraphicTag> CreateGraphicMap<TRuleElement>(TileMatcherModel tileMatcherModel, IEnumerable<TRuleElement> r)
            where TRuleElement : IRuleElement
        {
            var tagMap = new HashSet<string>();
            foreach (var tag in tileMatcherModel.Tags)
            {
                if (!string.IsNullOrEmpty(tag.Id))
                {
                    tagMap.Add(tag.Id);
                }
            }

            var terrainGraphicMap = new Dictionary<TRuleElement, GraphicTag>();
            foreach (var terrain in r)
            {
                if (TryFindFirstValidGraphicTag(terrain, tagMap, out var tag))
                {
                    terrainGraphicMap[terrain] = tag;
                }
                else
                {
                    terrainGraphicMap[terrain] = GraphicTag.From(terrain.GraphicTag);
                }
            }

            return terrainGraphicMap;
        }

        bool TryFindFirstValidGraphicTag<TRuleElement>(TRuleElement re, ICollection<string> validTags, out GraphicTag tag)
            where TRuleElement : IRuleElement
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
    }
}