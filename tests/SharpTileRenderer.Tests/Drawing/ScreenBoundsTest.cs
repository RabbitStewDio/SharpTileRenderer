using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.TexturePack;

namespace SharpTileRenderer.Tests.Drawing
{
    [TestFixture]
    public class ScreenBoundsTest
    {
        [Test]
        public void TestDivision()
        {
            var sb = new ScreenBounds(5f, 5f, 17f, 17f);
            var r = sb / new IntDimension(10, 10);
            r.Should().Be(new TileBounds(0, 0, 3, 3));
        }
    }
}