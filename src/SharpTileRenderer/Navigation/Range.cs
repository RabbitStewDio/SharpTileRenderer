using System;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.Navigation
{
    public readonly struct Range
    {
        public int Min { get; }
        public int Max { get; }

        public Range(int min, int max)
        {
            Min = Math.Min(min, max);
            Max = Math.Max(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Clamp(int value)
        {
            return Math.Min(Max, Math.Max(value, Min));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Clamp(float value)
        {
            return Math.Min(Max, Math.Max(value, Min));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Wrap(int value)
        {
            var delta = Max - Min;
            return ((value - Min) % delta + delta) % delta + Min;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Wrap(float value)
        {
            var delta = Max - Min;
            return ((value - Min) % delta + delta) % delta + Min;
        }

    }
}
