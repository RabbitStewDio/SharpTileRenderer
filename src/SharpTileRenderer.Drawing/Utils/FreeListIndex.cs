using System;

namespace SharpTileRenderer.Drawing.Utils
{
    public readonly struct FreeListIndex : IEquatable<FreeListIndex>
    {
        public static readonly FreeListIndex Empty = new FreeListIndex(1);
        public static readonly FreeListIndex Invalid = new FreeListIndex(0);
        readonly int value;
        
        FreeListIndex(int value)
        {
            if (value < 0) throw new ArgumentException("Invalid internal representation");
            this.value = value;
        }

        public int Value => value - 2;

        public bool IsEmpty => value == 1;
        public bool IsInvalid  => value == 0;

        public bool Equals(FreeListIndex other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is FreeListIndex other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value;
        }

        public static bool operator ==(FreeListIndex left, FreeListIndex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FreeListIndex left, FreeListIndex right)
        {
            return !left.Equals(right);
        }

        public static FreeListIndex operator +(FreeListIndex left, int right)
        {
            if (left.IsEmpty)
            {
                return Empty;
            }
            
            return new FreeListIndex(left.Value + right);
        }

        public static FreeListIndex Of(int i)
        {
            if (i < 0) throw new ArgumentException();
            return new FreeListIndex(i + 2);
        }

        public override string ToString()
        {
            if (value <= 0) return "FreeListIndex(invalid)";
            if (value == 1) return "FreeListIndex(empty)";
            
            return 
                $"FreeListIndex({nameof(value)}: {value - 2})";
        }
    }
}
