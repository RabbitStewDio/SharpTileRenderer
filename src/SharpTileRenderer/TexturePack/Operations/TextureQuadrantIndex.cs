using System;
using System.Runtime.CompilerServices;

namespace SharpTileRenderer.TexturePack.Operations
{
    public enum TextureQuadrantIndex
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
    }

    public static class TextureQuadrantIndexExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AsInt(this TextureQuadrantIndex idx) => (int)idx;

        public static string AsString(this TextureQuadrantIndex idx) =>
            idx switch
            {
                TextureQuadrantIndex.North => nameof(TextureQuadrantIndex.North),
                TextureQuadrantIndex.East => nameof(TextureQuadrantIndex.East),
                TextureQuadrantIndex.South => nameof(TextureQuadrantIndex.South),
                TextureQuadrantIndex.West => nameof(TextureQuadrantIndex.West),
                _ => throw new ArgumentOutOfRangeException(nameof(idx), idx, null)
            };
    }
}