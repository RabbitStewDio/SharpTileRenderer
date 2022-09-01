using SharpTileRenderer.Strategy.Base.Model;
using SharpTileRenderer.Strategy.Base.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpTileRenderer.Strategy.Base.Map
{
    public class MapReader
    {
        readonly Dictionary<char, ITerrain> terrainsByCharId;
        readonly Dictionary<char, ITerrainResource> resourcesByCharId;
        readonly Dictionary<char, IRoadType> roadsByCharId;
        readonly Dictionary<char, TerrainImprovement> improvementByCharId;

        public MapReader(StrategyGameRules rules, TerrainMap map)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            this.Map = map ?? throw new ArgumentNullException(nameof(map));
            this.terrainsByCharId = rules.TerrainTypes.Contents.Select(e => e.value).ToDict(t => t.AsciiId);
            this.resourcesByCharId = rules.TerrainResourceTypes.Contents.Select(e => e.value).ToDict(r => r.AsciiId);
            this.roadsByCharId = rules.RoadTypes.Contents.Select(e => e.value).Where(r => !r.River).ToDict(r => r.AsciiId);
            this.improvementByCharId = rules.TerrainImprovementTypes.Contents.Select(e => e.value).ToDict(r => r.AsciiId);
        }

        public TerrainMap Map { get; }

        public void ReadTerrain(TextReader r, int ox = 0, int y = 0, int w = 0, int h = 0)
        {
            void ReadTerrainLine(char c, int x, int targetY)
            {
                if (terrainsByCharId.TryGetValue(c, out var t))
                {
                    var v = Map[x, targetY];
                    v = v.WithTerrainIdx(t.TerrainId);
                    Map[x, targetY] = v;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            Read(r, ReadTerrainLine, ox, y, w, h);
        }

        void ReadImprovementHandler(char c, int x, int targetY)
        {
            if (improvementByCharId.TryGetValue(c, out var t))
            {
                var v = Map[x, targetY];
                v = v.WithImprovement(t.DataId);
                Map[x, targetY] = v;
            }
        }

        void ReadRoadHandler(char c, int x, int targetY)
        {
            if (roadsByCharId.TryGetValue(c, out var t))
            {
                var v = Map[x, targetY];
                var riverState = v.RoadsAndRiver & RoadTypeId.River;
                v = v.WithRoad(t.DataId | riverState);
                Map[x, targetY] = v;
            }
        }

        void ReadRiversHandler(char c, int x, int targetY)
        {
            if (roadsByCharId.TryGetValue(c, out var t))
            {
                var v = Map[x, targetY];
                var roadState = v.RoadsAndRiver & ~RoadTypeId.River;
                v = v.WithRoad(t.DataId | roadState);
                Map[x, targetY] = v;
            }
        }

        public void ReadRoads(TextReader r, int ox = 0, int y = 0, int w = 0, int h = 0)
        {
            Read(r, ReadRoadHandler, ox, y, w, h);
        }

        public void ReadImprovement(TextReader r, int ox = 0, int y = 0, int w = 0, int h = 0)
        {
            Read(r, ReadImprovementHandler, ox, y, w, h);
        }

        public void ReadRivers(TextReader r, int ox = 0, int y = 0, int w = 0, int h = 0)
        {
            Read(r, ReadRiversHandler, ox, y, w, h);
        }

        public void ReadResources(TextReader r, int ox = 0, int y = 0, int w = 0, int h = 0)
        {
            void ReadResource(char c, int x, int targetY)
            {
                if (resourcesByCharId.TryGetValue(c, out var t))
                {
                    var v = Map[x, targetY];
                    v = v.WithResource(t.ResourceId);
                    Map[x, targetY] = v;
                }
            }

            Read(r, ReadResource, ox, y, w, h);
        }

        void Read(TextReader r, ReadLine handler, int x = 0, int y = 0, int w = 0, int h = 0)
        {
            var line = r.ReadLine();
            var targetY = y;


            var maxH = Math.Max(0, h > 0 ? h : Map.Height - y) + y;
            while (line != null && targetY < maxH)
            {
                var maxX = CalculateMaxLength(line, w);
                for (var idx = 0; idx < maxX; idx += 1)
                {
                    handler(line[idx], x + idx, targetY);
                }

                targetY += 1;
                line = r.ReadLine();
            }
        }

        delegate void ReadLine(char c, int x, int targetY);

        static int CalculateMaxLength(string line, int width)
        {
            var maxX = line.Length;
            if (width > 0)
            {
                maxX = Math.Min(maxX, width);
            }

            return maxX;
        }
    }

    public static class MapReaderExtensions
    {
        public static string Strip(this string text, char barrier = '|')
        {
            var nl = Environment.NewLine;
            var sr = new StringReader(text);
            var line = sr.ReadLine();
            var result = new StringBuilder();
            var firstLine = true;
            while (line != null)
            {
                if (!firstLine)
                {
                    result.Append(nl);
                }

                int idx = line.IndexOf(barrier);
                if (idx >= 0)
                {
                    result.Append(line.Substring(idx + 1));
                    firstLine = false;
                }
                else if (result.Length == 0 && line.Length == 0)
                {
                    // skip leading empty lines.
                }
                else
                {
                    result.Append(line);
                    firstLine = false;
                }

                line = sr.ReadLine();
            }

            return result.ToString();
        }
    }
}