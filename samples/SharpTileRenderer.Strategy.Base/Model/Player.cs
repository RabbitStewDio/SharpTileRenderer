using System;

namespace SharpTileRenderer.Strategy.Base.Model
{
    public class Player : IPlayer
    {
        public Player(PlayerId id, string name, PlayerColor playerColor, Culture culture)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            PlayerColor = playerColor;
            Culture = culture;
        }

        public PlayerId Id { get; }
        public string Name { get; }
        public PlayerColor PlayerColor { get; }
        public Culture Culture { get; }
    }

    public readonly struct PlayerId : IEquatable<PlayerId>
    {
        readonly byte id;

        public PlayerId(byte id)
        {
            this.id = id;
        }

        public bool Equals(PlayerId other)
        {
            return id == other.id;
        }

        public override bool Equals(object? obj)
        {
            return obj is PlayerId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(PlayerId left, PlayerId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayerId left, PlayerId right)
        {
            return !left.Equals(right);
        }
    }
}
