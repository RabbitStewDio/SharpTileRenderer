﻿using System;

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
    }
}
