using System;

namespace SharpTileRenderer.Tests
{
    public readonly struct TestEntityKey : IEquatable<TestEntityKey>
    {
        public bool Equals(TestEntityKey other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is TestEntityKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(TestEntityKey left, TestEntityKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TestEntityKey left, TestEntityKey right)
        {
            return !left.Equals(right);
        }

        public readonly int Id;

        public TestEntityKey(int id)
        {
            this.Id = id;
        }
    }
}