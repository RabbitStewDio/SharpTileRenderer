using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.Drawing
{
    public class ScreenPositionMapperTest
    {
        [Test]
        public void SimpleTest()
        {
            var sm = new ScreenPositionMapper(new IntDimension(32, 32));
            sm.AddPhysical(new MapCoordinate(1,1), new ScreenPosition(1, 2));
            sm.AddPhysical(new MapCoordinate(1,1), new ScreenPosition(3, 4));
            sm.AddPhysical(new MapCoordinate(1,1), new ScreenPosition(5, 6));

            var list = new List<ScreenPosition>();
            sm.TryMapPhysical(new MapCoordinate(0, 0), list).Should().BeFalse();
            sm.TryMapPhysical(new MapCoordinate(1, 1), list).Should().BeTrue();
            
        }
    }
}