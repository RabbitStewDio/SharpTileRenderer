namespace SharpTileRenderer.Strategy.Base.Model
{
    /// <summary>
    ///  Container structure to hold all definitions of terrain data.
    /// </summary>
    public readonly struct TerrainData
    {
        const byte RoadAndRiverMask = 0b0000_0111;
        const byte RoadMask = 0b0000_0110;
        const byte NotRoadMask = 0b1111_1001;
        const byte RiverMask = 0b0000_0001;
        const byte NotRiverMask = 0b1111_1110;

        /// <summary>
        ///  The index of the terrain in the terrain lookup table.
        /// </summary>
        public readonly TerrainId TerrainIdx;

        readonly byte ownerData;

        /// <summary>
        ///  Player who controls the land. (6bit)
        ///  How control is retained: Farming, Purchased (2 bit)
        /// </summary>
        public PlayerId Owner => new PlayerId((byte)(ownerData & 0b0011_1111));

        public TerrainOwnershipType TerrainOwnershipType => (TerrainOwnershipType)(ownerData & 0b1100_0000);

        /// <summary>
        ///  Nearest city that controls the land. 
        /// </summary>
        public readonly SettlementId City;

        /// <summary>
        ///  Resource for this tile, or 0 for none. Resources
        ///  are enumerated, only one resource can exist on a 
        ///  tile.
        /// </summary>
        public readonly TerrainResourceId Resources;

        /// <summary>
        ///  The type of improvement built on the terrain. 
        ///  Multiple improvements can exist on the terrain at
        ///  the same time. 
        /// 
        ///  Rivers (1bit), 
        ///  Encodes Roads/Railways (2bit), 
        ///  Buildings like Irrigation, Mines, Fortress, ... 
        ///  (remaining bits) 
        /// </summary>
        readonly ushort improvement;

        public RoadTypeId RoadsAndRiver
        {
            get { return (RoadTypeId)(improvement & RoadAndRiverMask); }
        }

        public TerrainData WithRoad(RoadTypeId value)
        {
            var modifiedImprovement = this.improvement;
            modifiedImprovement &= NotRoadMask;
            modifiedImprovement |= (byte)((int)value & RoadMask);
            return new TerrainData(TerrainIdx, ownerData, City, Resources, modifiedImprovement);
        }
        
        public TerrainData WithRiver(RoadTypeId value)
        {
            var modifiedImprovement = this.improvement;
            modifiedImprovement &= NotRiverMask;
            modifiedImprovement |= (byte)((int)value & RiverMask);
            return new TerrainData(TerrainIdx, ownerData, City, Resources, modifiedImprovement);
        }
        
        public TerrainImprovementId Improvement => new TerrainImprovementId((short)((improvement & NotRoadMask) >> 3));

        public TerrainData(TerrainId terrainIdx, byte ownerData, SettlementId city, TerrainResourceId resources, ushort improvement)
        {
            TerrainIdx = terrainIdx;
            this.ownerData = ownerData;
            City = city;
            Resources = resources;
            this.improvement = improvement;
        }

        public override string ToString()
        {
            return $"{nameof(TerrainIdx)}: {TerrainIdx}, {nameof(Improvement)}: {Improvement}, {nameof(Owner)}: {Owner}, {nameof(City)}: {City}, {nameof(RoadsAndRiver)}: {RoadsAndRiver}";
        }

        public TerrainData WithTerrainIdx(TerrainId value)
        {
            return new TerrainData(value, ownerData, City, Resources, improvement);
        }

        public TerrainData WithImprovement(TerrainImprovementId valueId)
        {
            var modImprovement = improvement & RoadMask;
            var impId = valueId.ImprovementId << 3;
            modImprovement |= impId;
            return new TerrainData(TerrainIdx, ownerData, City, Resources, (ushort) modImprovement);
        }

        public TerrainData WithResource(TerrainResourceId valueResourceId)
        {
            return new TerrainData(TerrainIdx, ownerData, City, valueResourceId, improvement);
            
        }

        public TerrainData WithSettlement(SettlementId dataId)
        {
            return new TerrainData(TerrainIdx, ownerData, dataId, Resources, improvement);
        }
    }
}