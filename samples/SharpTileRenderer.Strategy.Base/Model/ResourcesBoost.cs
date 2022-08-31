namespace SharpTileRenderer.Strategy.Base.Model
{
    /// <summary>
    ///  Defines a resource in Civ style. 
    /// </summary>
    public struct ResourcesBoost
    {
        public float Food { get; set; }
        public float Production { get; set; }
        public float Trade { get; set; }

        public ResourcesBoost(float food, float production, float trade)
        {
            Food = food;
            Production = production;
            Trade = trade;
        }
    }
}