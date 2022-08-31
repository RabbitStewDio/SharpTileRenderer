namespace SharpTileRenderer.Strategy.Base.Model
{
    public interface IRoadType : ITerrainExtra
    {
        RoadTypeId DataId { get; }
        
        /// <summary>
        ///   Movement costs when a road of this type is built on a tile. 
        /// </summary>
        int MoveCost { get; }

        /// <summary>
        ///  Extra flag designating this road type as a river. While
        ///  each tile can only have one road type active, tiles can have
        ///  both rivers and roads.
        /// </summary>
        bool River { get; }
    }
}