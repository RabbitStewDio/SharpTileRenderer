using System;

namespace SharpTileRenderer.Strategy.Base.Model
{
    /// <summary>
    ///  Defines a resource in Civ style. 
    /// </summary>
    public struct Resources
    {
        public int Food { get; set; }
        public int Production { get; set; }
        public int Trade { get; set; }

        public Resources(int food, int production, int trade)
        {
            Food = food;
            Production = production;
            Trade = trade;
        }

        public static Resources operator +(Resources r1, Resources r2)
        {
            return new Resources(r1.Food + r2.Food, r1.Production + r2.Production, r1.Trade + r2.Trade);
        }

        public static Resources operator *(Resources r1, ResourcesBoost r2)
        {
            return new Resources((int)Math.Round(r1.Food * (1 + r2.Food)),
                                 (int)Math.Round(r1.Production * (1 + r2.Production)),
                                 (int)Math.Round(r1.Trade * (1 + r2.Trade)));
        }
    }
}
