using SharpTileRenderer.Util;
using System;

namespace SharpTileRenderer.Strategy.Base.Model
{
    public readonly struct TerrainImprovementId : IEquatable<TerrainImprovementId>
    {
        readonly short improvementId;

        public TerrainImprovementId(short improvementId)
        {
            this.improvementId = improvementId;
        }

        public bool Equals(TerrainImprovementId other)
        {
            return improvementId == other.improvementId;
        }

        public short ImprovementId
        {
            get { return improvementId; }
        }

        public override bool Equals(object? obj)
        {
            return obj is TerrainImprovementId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return improvementId.GetHashCode();
        }

        public static bool operator ==(TerrainImprovementId left, TerrainImprovementId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TerrainImprovementId left, TerrainImprovementId right)
        {
            return !left.Equals(right);
        }
    }

    public class TerrainImprovement : RuleElement, ITerrainExtra
    {
        public TerrainImprovement(TerrainImprovementId dataId,
                                  string id,
                                  char asciiId,
                                  string name,
                                  Optional<ITerrainExtra> requires,
                                  string graphicTag,
                                  params string[] alternativeGraphicTags)
            : base(id, asciiId, name, graphicTag, alternativeGraphicTags)
        {
            DataId = dataId;
            Requires = requires;
        }

        public TerrainImprovementId DataId { get; }
        public Optional<ITerrainExtra> Requires { get; }
    }
}