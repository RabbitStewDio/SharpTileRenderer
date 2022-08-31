using SharpTileRenderer.Util;

namespace SharpTileRenderer.Strategy.Base.Model
{
    public class RoadType : RuleElement, IRoadType
    {
        public const int NoMoveCostBonus = -1;

        public RoadType(RoadTypeId dataId,
                        string id,
                        char asciiId,
                        string name,
                        bool river,
                        int moveCost,
                        Optional<ITerrainExtra> requiredExtra,
                        string graphicTag,
                        params string[] alternativeGraphicTags) : base(id, asciiId, name, graphicTag,
                                                                       alternativeGraphicTags)
        {
            DataId = dataId;
            River = river;
            MoveCost = moveCost;
            Requires = requiredExtra;
        }

        public RoadType(RoadTypeId dataId,
                        string id,
                        char asciiId,
                        string name,
                        bool river,
                        int moveCost,
                        string graphicTag,
                        params string[] alternativeGraphicTags) :
            this(dataId, id, asciiId, name, river, moveCost, default, graphicTag, alternativeGraphicTags)
        { }

        public bool River { get; }
        public int MoveCost { get; }
        public RoadTypeId DataId { get; }
        public Optional<ITerrainExtra> Requires { get; }
    }
}
