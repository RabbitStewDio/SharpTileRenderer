using SharpTileRenderer.Strategy.Base.Map;
using SharpTileRenderer.Strategy.Base.Model;
using SharpTileRenderer.Strategy.Base.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TerrainData = SharpTileRenderer.Strategy.Base.Model.TerrainData;

namespace SharpTileRenderer.Strategy.Base
{
    public class StrategyGameData
    {
        readonly TerrainMap terrain;

        public StrategyGameData()
        {
            Rules = new StrategyGameRules();
            TerrainHeight = 300;
            TerrainWidth = 300;
            terrain = new TerrainMap(TerrainWidth, TerrainHeight);

            Settlements = new SettlementManager(TerrainWidth, TerrainHeight);

            Players = new PlayerManager()
            {
                new Player(new PlayerId(0), "Barbarian", PlayerColor.Red, Culture.Celtic),
                new Player(new PlayerId(1), "Player A", PlayerColor.Green, Culture.Classical),
                new Player(new PlayerId(2), "Player B", PlayerColor.Blue, Culture.Celtic)
            };

            var r = new MapReader(Rules, terrain);
            r.ReadTerrain(new StringReader(MapData.TerrainData), 0, 0, TerrainWidth, TerrainHeight);
            r.ReadRoads(new StringReader(MapData.RoadData), 0, 0, TerrainWidth, TerrainHeight);
            r.ReadRivers(new StringReader(MapData.RiverData), 0, 0, TerrainWidth, TerrainHeight);
            r.ReadImprovement(new StringReader(MapData.ImprovementData), 0, 0, TerrainWidth, TerrainHeight);
            r.ReadResources(new StringReader(MapData.ResourceData), 0, 0, TerrainWidth, TerrainHeight);

            AddSettlement(new Settlement(new SettlementId(0), "Capital City", new Point(7, 13), new PlayerId(1), 4000, true));
            AddSettlement(new Settlement(new SettlementId(1), "Satellite Settlement", new Point(20, 14), new PlayerId(2), 2000, false));
        }

        public PlayerManager Players { get; }
        public SettlementManager Settlements { get; }

        public int TerrainWidth { get; }
        public int TerrainHeight { get; }

        public StrategyGameRules Rules { get; }

        public IMap2D<TerrainData> Terrain
        {
            get { return terrain; }
        }

        public int AddSettlement(Settlement s)
        {
            var idx = Settlements.AddSettlement(s);

            var t = terrain[s.Location.X, s.Location.Y];
            t = t.WithSettlement(s.DataId);
            terrain[s.Location.X, s.Location.Y] = t;
            return idx;
        }

        public class PlayerManager : IReadOnlyList<Player>
        {
            readonly List<Player> players;
            readonly Dictionary<PlayerId, Player> playersById;

            public PlayerManager()
            {
                players = new List<Player>();
                playersById = new Dictionary<PlayerId, Player>();
            }

            public void Add(Player p)
            {
                if (playersById.TryGetValue(p.Id, out _))
                {
                    throw new ArgumentException();
                }

                players.Add(p);
                playersById.Add(p.Id, p);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator<Player> IEnumerable<Player>.GetEnumerator()
            {
                return GetEnumerator();
            }

            public List<Player>.Enumerator GetEnumerator()
            {
                return players.GetEnumerator();
            }

            public int Count => players.Count;

            public Player this[int index]
            {
                get => players[index];
            }

            public Player this[PlayerId idx]
            {
                get
                {
                    if (playersById.TryGetValue(idx, out var value))
                    {
                        return value;
                    }

                    throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        ///   A holder of all settlements for all players. This is a 1-based list,
        ///   the id of zero is reserved as "no city" marker.
        /// </summary>
        public class SettlementManager: IReadOnlyList<Settlement>, IMap2D<Settlement?>
        {
            readonly List<Settlement> settlements;
            readonly Dictionary<Point, Settlement> settlementsByLocation;
            
            public SettlementManager(int width, int height)
            {
                Width = width;
                Height = height;
                settlements = new List<Settlement>();
                settlementsByLocation = new Dictionary<Point, Settlement>();
            }

            public int Width { get; }
            public int Height { get; }

            public Settlement? this[int x, int y]
            {
                get
                {
                    if (settlementsByLocation.TryGetValue(new Point(x, y), out var value))
                    {
                        return value;
                    }

                    return null;
                }
            }

            #pragma warning disable CS0067 
            public event EventHandler<MapDataChangedEventArgs>? MapDataChanged;
            #pragma warning restore CS0067

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator<Settlement> IEnumerable<Settlement>.GetEnumerator()
            {
                return GetEnumerator();
            }

            public List<Settlement>.Enumerator GetEnumerator()
            {
                return settlements.GetEnumerator();
            }

            public Settlement this[int index]
            {
                get
                {
                    return settlements[index];
                }
            }

            public int Count => settlements.Count;

            internal int AddSettlement(Settlement s)
            {
                if (settlements.Count == 254)
                {
                    throw new ArgumentException();
                }

                if (settlementsByLocation.ContainsKey(s.Location))
                {
                    throw new ArgumentException();
                }
                
                settlements.Add(s);
                settlementsByLocation[s.Location] = s;
                return settlements.Count;
            }
        }
    }
}
