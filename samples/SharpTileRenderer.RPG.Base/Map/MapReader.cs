using SharpTileRenderer.RPG.Base.Model;
using SharpTileRenderer.RPG.Base.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpTileRenderer.RPG.Base.Map
{
    public class MapReader
    {
        readonly DungeonGameData game;
        readonly Dictionary<char, TerrainElement> terrainsByCharId;
        readonly Dictionary<char, ItemElement> itemsByCharId;

        public MapReader(DungeonGameRules rules, DungeonGameData game)
        {
            this.game = game;
            this.terrainsByCharId = rules.Terrains.ToDict(t => t.AsciiId);
            this.itemsByCharId = rules.Items.ToDict(r => r.AsciiId);
        }
        
        void Read<T>(DefaultMap<T> map, TextReader r, ReadLine<T> handler, int x = 0, int y = 0, int w = 0, int h = 0)
        {
            var line = r.ReadLine();
            var targetY = y;


            var maxH = Math.Max(0, h > 0 ? h : map.Height - y) + y;
            while (line != null && targetY < maxH)
            {
                var maxX = CalculateMaxLength(line, w);
                for (var idx = 0; idx < maxX; idx += 1)
                {
                    handler(map, line[idx], x + idx, targetY);
                }

                targetY += 1;
                line = r.ReadLine();
            }
        }

        delegate void ReadLine<T>(DefaultMap<T> map, char c, int x, int targetY);

        static int CalculateMaxLength(string line, int width)
        {
            var maxX = line.Length;
            if (width > 0)
            {
                maxX = Math.Min(maxX, width);
            }

            return maxX;
        }
        
        void ReadTerrainHandler(DefaultMap<TerrainElement> map, char c, int x, int targetY)
        {
            if (terrainsByCharId.TryGetValue(c, out var t))
            {
                map[x, targetY] = t;
            }
            else
            {
                throw new ArgumentException($"There is no handler for symbol '{c}' at position {x},{targetY}");
            }
        }

        public void ReadTerrain(TextReader r, int ox = 0, int y = 0, int w = 0, int h = 0)
        {
            Read(game.Terrain, r, ReadTerrainHandler, ox, y, w, h);
        }

        void ReadItemHandler(DefaultMap<ItemElement> map, char c, int x, int targetY)
        {
            if (itemsByCharId.TryGetValue(c, out var t))
            {
                map[x, targetY] = t;
            }
            else
            {
                throw new ArgumentException($"There is no handler for symbol '{c}' at position {x},{targetY}");
            }
        }

        public void ReadItems(TextReader r, int ox = 0, int y = 0, int w = 0, int h = 0)
        {
            Read(game.Items, r, ReadItemHandler, ox, y, w, h);
        }

    }
}