using SharpTileRenderer.Strategy.Base.Model;
using SharpTileRenderer.Strategy.Base.Util;
using System;
using static SharpTileRenderer.Strategy.Base.Model.ResourcesExtensions;

namespace SharpTileRenderer.Strategy.Base
{
    public class StrategyGameRules
    {
        public StrategyGameRules()
        {
            Terrains = new DefinedTerrains();
            TerrainTypes = TypeRegistry.CreateFrom(Terrains, Terrains.Nothing, t => t.TerrainId);

            TerrainResources = new DefinedResources();
            TerrainResourceTypes = TypeRegistry.CreateFrom(TerrainResources, TerrainResources.None, r => r.ResourceId);

            Roads = new DefinedRoads();
            RoadTypes = TypeRegistry.CreateFromInstances(r => r.DataId,  Roads.Nothing, Roads.River, Roads.Road, Roads.Railroad);

            TerrainImprovements = new DefinedImprovements();
            TerrainImprovementTypes = TypeRegistry.CreateFrom(TerrainImprovements, TerrainImprovements.None, i => i.DataId);

            MoveFragments = 3;
        }


        public DefinedImprovements TerrainImprovements { get; }

        public TypeRegistry<TerrainResourceId, ITerrainResource> TerrainResourceTypes { get; }

        public DefinedResources TerrainResources { get; }

        public int MoveFragments { get; }

        public DefinedRoads Roads { get; }

        public DefinedTerrains Terrains { get; }

        public ITypeRegistry<TerrainId, ITerrain> TerrainTypes { get; }
        public ITypeRegistry<TerrainImprovementId, TerrainImprovement> TerrainImprovementTypes { get; }
        public ITypeRegistry<RoadTypeId, IRoadType> RoadTypes { get; }

        public class DefinedImprovements
        {
            public DefinedImprovements()
            {
                None = new TerrainImprovement(new TerrainImprovementId(0), "none", ' ', "None", default, "");
                Farms = new TerrainImprovement(new TerrainImprovementId(1), "farms", 'f', "Farms", default, "farmland");
                Mining = new TerrainImprovement(new TerrainImprovementId(2), "mine", 'm', "Mine", default, "mine");
            }

            public TerrainImprovement None { get; }
            public TerrainImprovement Farms { get; }
            public TerrainImprovement Mining { get; }
        }

        public class DefinedResources
        {
            public DefinedResources()
            {
                None = new TerrainResource(new TerrainResourceId(0), "", ' ', "", new Resources(), "");
                Oasis = new TerrainResource(new TerrainResourceId(1), "oasis", 'o', "Oasis", new Resources(3, 0, 0), "oasis");
                GrasslandBonus = new TerrainResource(new TerrainResourceId(2), "bonus", 'b', "Resources", new Resources(0, 1, 0),
                                                     "grassland_resources");
                Coal = new TerrainResource(new TerrainResourceId(3), "coal", 'c', "Coal", new Resources(0, 2, 0), "coal");
                Furs = new TerrainResource(new TerrainResourceId(4), "furs", 'f', "Furs", new Resources(1, 0, 3), "furs");
                Pheasant = new TerrainResource(new TerrainResourceId(5), "pheasant", 'p', "Pheasant", new Resources(2, 0, 0), "pheasant");
            }

            public ITerrainResource None { get; }
            public ITerrainResource Oasis { get; }
            public ITerrainResource GrasslandBonus { get; }
            public ITerrainResource Coal { get; }
            public ITerrainResource Furs { get; }
            public ITerrainResource Pheasant { get; }
        }

        public class DefinedRoads
        {
            internal DefinedRoads()
            {
                Nothing = new RoadType(RoadTypeId.None, "None", ' ', "", false, -1, "");
                Road = new RoadType(RoadTypeId.Road, "road", '+', "Road", false, 2, "road");
                Railroad = new RoadType(RoadTypeId.Railroad, "Railroad", '#', "Railroad", false, 1, "railroad");
                River = new RoadType(RoadTypeId.River, "River", 'x', "River", true, -1, "river");
            }

            public IRoadType Nothing { get; }
            public IRoadType Road { get; }
            public IRoadType Railroad { get; }
            public IRoadType River { get; }
        }

        public class DefinedTerrains
        {

            internal DefinedTerrains()
            {
                Nothing = new Terrain(new TerrainId(0), 'i', "inaccessible")
                          .WithClass(TerrainClass.Water)
                          .WithGraphic("inaccessible", "arctic");

                Ocean = new Terrain(new TerrainId(1), ' ', "ocean")
                        .WithClass(TerrainClass.Water)
                        .WithGraphic("coast")
                        .WithMoveCost(1)
                        .WithBaseProduction(Resource(1, 0, 2));

                DeepOcean = new Terrain(new TerrainId(2), ':', "deep_ocean")
                            .WithClass(TerrainClass.Water)
                            .WithGraphic("floor", "coast")
                            .WithMoveCost(1)
                            .WithBaseProduction(Resource(1, 0, 2));

                Arctic = new Terrain(new TerrainId(3), 'a', "glacier")
                         .WithClass(TerrainClass.Land)
                         .WithGraphic("arctic")
                         .WithMoveCost(2)
                         .WithRoads(4)
                         .WithMining(10, Prod(1));

                Desert = new Terrain(new TerrainId(4), 'd', "desert")
                         .WithClass(TerrainClass.Land)
                         .WithGraphic("desert")
                         .WithMoveCost(1)
                         .WithRoads(2, TradeBoost(1f))
                         .WithBaseProduction(Prod(1))
                         .WithMining(5, Prod(1))
                         .WithIrrigation(5, Food(1));

                Forest = new Terrain(new TerrainId(5), 'f', "forest")
                         .WithClass(TerrainClass.Land)
                         .WithGraphic("forest")
                         .WithMoveCost(2)
                         .WithRoads(4)
                         .WithBaseProduction(Resource(1, 2, 0));

                Gras = new Terrain(new TerrainId(6), 'g', "gras")
                       .WithClass(TerrainClass.Land)
                       .WithGraphic("forest")
                       .WithMoveCost(1)
                       .WithRoads(2, TradeBoost(1f))
                       .WithBaseProduction(Resource(1, 0, 2))
                       .WithIrrigation(5, Food(1));

                Hills = new Terrain(new TerrainId(7), 'h', "hills")
                        .WithClass(TerrainClass.Land)
                        .WithGraphic("hills")
                        .WithMoveCost(2)
                        .WithRoads(4)
                        .WithBaseProduction(Resource(1, 0, 0))
                        .WithIrrigation(10, Food(1))
                        .WithMining(10, Prod(3));

                Mountains = new Terrain(new TerrainId(8), 'm', "mountains")
                            .WithClass(TerrainClass.Land)
                            .WithGraphic("mountains")
                            .WithMoveCost(3)
                            .WithRoads(6)
                            .WithBaseProduction(Prod(1))
                            .WithMining(10, Prod(1));

                Plains = new Terrain(new TerrainId(9), 'p', "plains")
                         .WithClass(TerrainClass.Land)
                         .WithGraphic("plains")
                         .WithMoveCost(1)
                         .WithRoads(2, TradeBoost(1))
                         .WithBaseProduction(Resource(1, 1, 0))
                         .WithIrrigation(5, Food(1));

                Swamp = new Terrain(new TerrainId(10), 's', "swamp")
                        .WithClass(TerrainClass.Land)
                        .WithGraphic("swamp")
                        .WithMoveCost(1)
                        .WithRoads(4)
                        .WithBaseProduction(Food(1));

                Tundra = new Terrain(new TerrainId(11), 't', "tundra")
                         .WithClass(TerrainClass.Land)
                         .WithGraphic("tundra")
                         .WithMoveCost(1)
                         .WithRoads(4)
                         .WithBaseProduction(Food(1))
                         .WithIrrigation(5, Food(1));
            }

            public ITerrain Nothing { get; }
            public ITerrain Ocean { get; }
            public ITerrain DeepOcean { get; }
            public ITerrain Arctic { get; }
            public ITerrain Desert { get; }
            public ITerrain Forest { get; }
            public ITerrain Gras { get; }
            public ITerrain Hills { get; }
            public ITerrain Mountains { get; }
            public ITerrain Plains { get; }
            public ITerrain Tundra { get; }
            public ITerrain Swamp { get; }
        }
    }
    
    [Flags]
    public enum TerrainClass
    {
        None = 0,
        Land = 1,
        Water = 2
    }
}