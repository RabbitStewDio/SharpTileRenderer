namespace SharpTileRenderer.Strategy.Base.Model
{
    /// <summary>
    ///   A basic, freeciv inspired terrain definition.
    ///   The interface is exposing readonly properties as no sane program changes
    ///   its basic data on my watch.
    /// </summary>
    public interface ITerrain : IRuleElement
    {
        TerrainId TerrainId { get; }
        
        /// <summary>
        ///   The terrain classification. FreeCiv uses only two tags: Land and Oceanic, but
        ///   we could group terrain into more sets if needed. Tagging terrain with
        ///   classes allows us to generalize movement of units, for instance.
        /// </summary>
        TerrainClass Class { get; }

        /// <summary>
        ///   Base movement costs. Those can be modified by building roads on a tile.
        ///   Once a road is built, the road's move cost is used instead.
        /// </summary>
        int MoveCost { get; }

        /// <summary>
        ///   Base production.
        /// </summary>
        Resources Production { get; }

        /// <summary>
        ///   Production changes after building roads.
        /// </summary>
        Resources RoadBonus { get; }

        /// <summary>
        ///  Percentage boost after building roads. (ie 50% more trade = ResourceBoost(0,0,0.5))
        /// </summary>
        ResourcesBoost RoadBoost { get; }

        /// <summary>
        ///   Production changes after building mines.
        /// </summary>
        Resources MiningBonus { get; }

        /// <summary>
        ///   Production changes after building irrigation systems.
        /// </summary>
        Resources IrrigationBonus { get; }

        /// <summary>
        ///   Time in turns to build mines.
        /// </summary>
        int MiningTime { get; }

        /// <summary>
        ///   Time in turns to build irrigation.
        /// </summary>
        int IrrigationTime { get; }

        /// <summary>
        ///   Time in turns to build roads.
        /// </summary>
        int RoadTime { get; }
    }
}