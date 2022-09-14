using SharpTileRenderer.Navigation;
using SharpTileRenderer.RPG.Base.Map;
using SharpTileRenderer.RPG.Base.Model;
using SharpTileRenderer.RPG.Base.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SharpTileRenderer.RPG.Base
{
    public class DungeonGameData
    {
        readonly DungeonGameRules rules;
        public int TerrainWidth { get; }
        public int TerrainHeight { get; }
        public DefaultMap<TerrainElement> Terrain { get; }
        public DefaultMap<ItemElement> Items { get; }
        public List<Actor> Actors { get; }

        public DungeonGameData(DungeonGameRules rules)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.TerrainHeight = 100;
            this.TerrainWidth = 100;

            var r = new Random(5000);

            this.Terrain = new DefaultMap<TerrainElement>(TerrainWidth, TerrainHeight, rules.Terrains.Grass);
            this.Items = new DefaultMap<ItemElement>(TerrainWidth, TerrainHeight, rules.Items.None);

            var mr = new MapReader(rules, this);
            mr.ReadTerrain(new StringReader(DungeonGameMaps.TerrainData), 0, 0, TerrainWidth, TerrainHeight);
            mr.ReadItems(new StringReader(DungeonGameMaps.ItemData), 0, 0, TerrainWidth, TerrainHeight);

            this.Actors = new List<Actor>();
            for (int i = 0; i < 1000; i += 1)
            {
                var position = new Vector2((float)(r.NextDouble() * TerrainWidth),
                                                 (float)(r.NextDouble() * TerrainHeight));
                Actors.Add(new Actor(rules.Actors.ChooseRandomElement(r),
                                     position, r));
            }
        }
    }

    public class DungeonGame
    {
        public DungeonGameData GameData { get; }
        public DungeonGameRules GameRules { get; }

        public DungeonGame()
        {
            GameRules = new DungeonGameRules();
            GameData = new DungeonGameData(GameRules);
        }

        public NavigatorMetaData NavigatorConfig => NavigatorMetaData.FromGridType(GridType.Grid)
                                                                     // .WithHorizontalLimit(GameData.TerrainWidth)
                                                                     // .WithVerticalLimit(GameData.TerrainHeight);
                                                                     .WithHorizontalWrap(GameData.TerrainWidth)
                                                                     .WithVerticalWrap(GameData.TerrainHeight);

        public void Update(DungeonGameTime time)
        {
            foreach (var a in GameData.Actors)
            {
                a.Update(time, GameData);
            }
        }
    }
}