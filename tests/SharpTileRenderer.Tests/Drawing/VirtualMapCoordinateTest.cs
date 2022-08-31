using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Navigation;

namespace SharpTileRenderer.Tests.Drawing
{
    [TestFixture]
    public class VirtualMapCoordinateTest
    {
        [Test]
        public void TestNormalize()
        {
            var m = new VirtualMapCoordinate();
            m.Normalize().Should().Be(new MapCoordinate());
        }
    }
}