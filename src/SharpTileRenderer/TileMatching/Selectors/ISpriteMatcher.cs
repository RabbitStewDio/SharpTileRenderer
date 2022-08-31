using SharpTileRenderer.Navigation;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors
{
    /// <summary>
    ///   Contains the logic to map game state into tiles.
    /// </summary>
    public interface ISpriteMatcher
    {
        string MatcherType { get; }
        bool IsThreadSafe { get; }
    }

    public interface ISpriteMatcher<TInputData>: ISpriteMatcher
    {
        /// <summary>
        ///  Reads the input map at position (x,y) and produces a matching render tile based
        ///  on the map data found and the context of that map entry.
        /// </summary>
        /// <param name="c">query result coordinate</param>
        /// <param name="z">query context</param>
        /// <param name="resultCollector">This callback receives all matched tiles.</param>
        /// <returns></returns>
        bool Match(in SpriteMatcherInput<TInputData> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector);
    }
}