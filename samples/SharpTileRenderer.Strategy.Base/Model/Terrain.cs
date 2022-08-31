using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Strategy.Base.Model
{
    public readonly struct TerrainId : IEquatable<TerrainId>
    {
        readonly byte id;

        public TerrainId(byte id)
        {
            this.id = id;
        }

        public bool Equals(TerrainId other)
        {
            return id == other.id;
        }

        public override bool Equals(object? obj)
        {
            return obj is TerrainId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(TerrainId left, TerrainId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TerrainId left, TerrainId right)
        {
            return !left.Equals(right);
        }
    }
    
    public class Terrain : ITerrain, IEquatable<Terrain>
    {
        readonly List<string> alternativeGraphicTags;

        public Terrain(TerrainId terrainId, char asciiId, string idText)
        {
            TerrainId = terrainId;
            AsciiId = asciiId;
            Id = idText ?? throw new ArgumentNullException(nameof(idText));
            Name = Id; // a sensible default ..
            Class = TerrainClass.Land;
            alternativeGraphicTags = new List<string>();
        }

        public bool Equals(Terrain? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return AsciiId == other.AsciiId && string.Equals(Id, other.Id);
        }

        public string Name { get; set; }
        public string? GraphicTag { get; set; }

        public IReadOnlyList<string> AlternativeGraphicTags
        {
            get { return alternativeGraphicTags; }
            set
            {
                if (value == null!)
                {
                    return;
                }
                alternativeGraphicTags.Clear();
                alternativeGraphicTags.AddRange(value);
            }
        }

        public TerrainId TerrainId { get; }
        public char AsciiId { get; }
        public string Id { get; }
        public TerrainClass Class { get; set; }

        public int MoveCost { get; set; }
        public Resources Production { get; set; }
        public Resources RoadBonus { get; set; }
        public ResourcesBoost RoadBoost { get; set; }
        public Resources MiningBonus { get; set; }
        public Resources IrrigationBonus { get; set; }
        public int MiningTime { get; set; }
        public int IrrigationTime { get; set; }
        public int RoadTime { get; set; }
        
        public void AddAlternateGraphicTag(string c)
        {
            alternativeGraphicTags.Add(c ?? throw new ArgumentNullException(nameof(c)));
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Terrain)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AsciiId.GetHashCode() * 397) ^ (Id.GetHashCode());
            }
        }

        public static bool operator ==(Terrain left, Terrain right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Terrain left, Terrain right)
        {
            return !Equals(left, right);
        }
    }
}
