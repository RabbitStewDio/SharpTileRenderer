using SharpTileRenderer.Navigation;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.ViewPorts
{
    public interface IScreenPositionMapper
    {
        bool TryMapVirtual(VirtualMapCoordinate pos, out ScreenPosition sp);
        bool TryMapPhysical(ContinuousMapCoordinate pos, List<ScreenPosition> results);
    }
}