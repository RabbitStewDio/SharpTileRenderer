using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;

namespace SharpTileRenderer.Tests.Drawing
{
    [TestFixture]
    public class ScreenToMapConverterTest
    {
        [Test]
        public void TestMapping()
        {
            var converter = ScreenToMapConverter.Create(GridType.Grid);
            var viewPort = new ViewPort(NavigatorMetaData.FromGridType(GridType.Grid), TileShape.Grid, new IntDimension(32, 32));
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            
            converter.ScreenToMap(viewPort, new ScreenPosition(0, 0)).Should().Be(new VirtualMapCoordinate(-5, -3.75f));
        }
    }
}