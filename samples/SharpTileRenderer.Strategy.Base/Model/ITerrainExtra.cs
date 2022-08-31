using SharpTileRenderer.Util;

namespace SharpTileRenderer.Strategy.Base.Model
{
    /// <summary>
    ///  Terrain extras are stored in a bit vector, and thus we 
    ///  need to assign them a globally valid index position for
    ///  that vector.
    /// </summary>
    public interface ITerrainExtra : IRuleElement
    {
        /// <summary>
        ///  Defines a dependency between terrain extras. This 
        ///  is used for road and railroads.
        /// </summary>
        Optional<ITerrainExtra> Requires { get; }
    }
}
