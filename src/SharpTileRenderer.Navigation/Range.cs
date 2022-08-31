using System;

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

        public int Clamp(int value)
        {
            return Math.Min(Max, Math.Max(value, Min));
        }

        public float Clamp(float value)
        {
            return Math.Min(Max, Math.Max(value, Min));
        }
    }
}
