using System.Collections.Generic;

namespace SharpTileRenderer.Strategy.Base.Model
{
    public static class TerrainExtensions
    {
        public static List<string> AllGraphicTags(this IRuleElement t, List<string>? tags = null)
        {
            tags ??= new List<string>();
            if (t.GraphicTag != null)
            {
                tags.Add(t.GraphicTag);
            }

            tags.AddRange(t.AlternativeGraphicTags);
            return tags;
        }

        public static Terrain WithGraphic(this Terrain t, string graphicTag, params string[] extraGraphics)
        {
            t.GraphicTag = graphicTag;

            var tags = new List<string>(t.AlternativeGraphicTags);
            tags.AddRange(extraGraphics);
            t.AlternativeGraphicTags = tags.AsReadOnly();
            return t;
        }

        public static Terrain WithClass(this Terrain t, TerrainClass tc)
        {
            t.Class |= tc;
            return t;
        }

        public static Terrain WithMoveCost(this Terrain t, int mc)
        {
            t.MoveCost = mc;
            return t;
        }

        public static Terrain WithMining(this Terrain t, int mc, Resources bonus)
        {
            t.MiningTime = mc;
            t.MiningBonus = bonus;
            return t;
        }

        public static Terrain WithIrrigation(this Terrain t, int mc, Resources bonus)
        {
            t.IrrigationTime = mc;
            t.IrrigationBonus = bonus;
            return t;
        }

        public static Terrain WithRoads(this Terrain t, int mc, ResourcesBoost bonus = new ResourcesBoost())
        {
            t.RoadTime = mc;
            t.RoadBoost = bonus;
            return t;
        }

        public static Terrain WithBaseProduction(this Terrain t, Resources prod)
        {
            t.Production = prod;
            return t;
        }
    }
}