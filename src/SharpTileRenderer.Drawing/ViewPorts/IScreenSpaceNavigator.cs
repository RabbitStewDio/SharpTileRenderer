using SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    /// <summary>
    ///   Encapsulates the knowledge on how to translate between coordinate spaces. Screen space
    ///   is always a continuous coordinate space, while map organization can be non-continuous.
    ///   It is not safe (from a user's perspective) to assume that coordinates that are close by
    ///   on the screen are also close by in the map. (See staggered iso map type).
    ///
    ///   If you need to interpolate between two map points, always do so in screen space. 
    /// </summary>
    public interface IScreenSpaceNavigator
    {
        /// <summary>
        ///   Attempts to map the given screen position into a map coordinate. This is equivalent of
        ///   ray-casting from the screen through the camera/viewport into the world, without taking
        ///   limits or wrap-around into account. 
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="pos"></param>
        /// <returns>a world position matching the current screen.</returns>
        WorldPosition TranslateViewToWorld(IViewPort vp, ScreenPosition pos);

        /// <summary>
        ///   Attempts to map from the given world position into a screen space position. This is an
        ///   unsafe operation! When map wrapping is enabled, there may be more than one screen position
        ///   for any given map point. There is no good way to detect this condition.
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="pos"></param>
        /// <param name="result"></param>
        /// <returns>false if the world position is invalid as map coordinate. This still can produce a screen position.</returns>
        bool TryMapInverse(IViewPort vp, VirtualMapCoordinate pos, out ScreenPosition result);

        List<ScreenPosition> MapInverse(IViewPort vp, ContinuousMapCoordinate pos, List<ScreenPosition>? result = null);
    }

    class ScreenSpaceNavigator : IScreenSpaceNavigator
    {
        readonly ScreenPositionMapping reverseMapper;
        readonly IScreenToMapConverter forwardMapper;

        public ScreenSpaceNavigator(GridType t, IntDimension tileSize)
        {
            forwardMapper = ScreenToMapConverter.Create(t);
            reverseMapper = new ScreenPositionMapping(t, tileSize);
        }

        public void Refresh(IViewPort p)
        {
            reverseMapper.Refresh(p);
        }
        
        public WorldPosition TranslateViewToWorld(IViewPort vp, ScreenPosition pos)
        {
            var virtualPos = forwardMapper.ScreenToMap(vp, pos);
            var delta = virtualPos - virtualPos.Normalize();
            // Navigation eliminates fractional position data (where are we inside the tile), and thus needs to be preserved.
            vp.Navigation[MapNavigationType.Map].Navigate(GridDirection.None, virtualPos.Normalize(), out var mc, out var info);

            // todo rotation
            return new WorldPosition(new ContinuousMapCoordinate(mc.X + delta.X, mc.Y + delta.Y), virtualPos, info);
        }

        public bool TryMapInverse(IViewPort vp, VirtualMapCoordinate pos, out ScreenPosition result)
        {
            return reverseMapper.TryMapVirtual(vp, pos, out result);
        }

        public List<ScreenPosition> MapInverse(IViewPort vp, ContinuousMapCoordinate pos, List<ScreenPosition>? result = null)
        {
            result ??= new List<ScreenPosition>(); 
            result.Clear();
            reverseMapper.TryMapPhysical(vp, pos, result);
            return result;
        }
    }
}