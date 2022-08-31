using System;

namespace SharpTileRenderer.Strategy.Base.Model
{
    public readonly struct TerrainResourceId : IEquatable<TerrainResourceId>
    {
        readonly byte id;

        public TerrainResourceId(byte id)
        {
            this.id = id;
        }

        public bool Equals(TerrainResourceId other)
        {
            return id == other.id;
        }

        public override bool Equals(object? obj)
        {
            return obj is TerrainResourceId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(TerrainResourceId left, TerrainResourceId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TerrainResourceId left, TerrainResourceId right)
        {
            return !left.Equals(right);
        }
    }
    
    /// <summary>
    ///  Defines a resource bonus for terrain tiles. This is something like
    ///  gold, iron or oil. Resources can grant a fixed production bonus,
    ///  which will increase even more with terrain improvements.
    /// </summary>
    public interface ITerrainResource : IRuleElement
    {
        Resources BonusResources { get; }
        TerrainResourceId ResourceId { get; }
    }

    public class TerrainResource : RuleElement, ITerrainResource
    {
        public TerrainResource(TerrainResourceId resourceId, 
                               string id,
                               char asciiId,
                               string name,
                               Resources bonusResources,
                               string graphicTag,
                               params string[] alternativeGraphicTags) :
            base(id, asciiId, name, graphicTag, alternativeGraphicTags)
        {
            ResourceId = resourceId;
            BonusResources = bonusResources;
        }

        public TerrainResourceId ResourceId { get; }
        public Resources BonusResources { get; }
    }
}
