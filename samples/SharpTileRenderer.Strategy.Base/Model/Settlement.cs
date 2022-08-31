using SharpTileRenderer.Strategy.Base.Map;
using System;

namespace SharpTileRenderer.Strategy.Base.Model
{
    public readonly struct SettlementId : IEquatable<SettlementId>
    {
        readonly int id;

        public SettlementId(int id)
        {
            this.id = id;
        }

        public bool Equals(SettlementId other)
        {
            return id == other.id;
        }

        public override bool Equals(object? obj)
        {
            return obj is SettlementId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id;
        }

        public static bool operator ==(SettlementId left, SettlementId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SettlementId left, SettlementId right)
        {
            return !left.Equals(right);
        }
    }
    
    public class Settlement : ISettlement
    {
        public Settlement(SettlementId dataId, string name, Point location, PlayerId owner, long population, bool walled)
        {
            DataId = dataId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Location = location;
            Owner = owner;
            Population = population;
            Walled = walled;
        }

        public SettlementId DataId { get; }
        public Point Location { get; }
        public string Name { get; set; }
        public PlayerId Owner { get; set; }
        public long Population { get; set; }
        public bool Walled { get; set; }
    }
}
