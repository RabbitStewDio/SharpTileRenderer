using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;

namespace SharpTileRenderer.Tests.Drawing
{
    [TestFixture]
    public class IsoStaggeredScreenToMapConverterTest
    {
        ViewPort viewPort;
        IsoStaggeredScreenToMapConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new IsoStaggeredScreenToMapConverter();
            viewPort = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 32));
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);
        }

        [Test]
        public void Test()
        {
            converter.ScreenToMap(viewPort, new ScreenPosition(0, 0)).Should().Be(new VirtualMapCoordinate(-5f, -7.75f));
            converter.ScreenToMap(viewPort, new ScreenPosition(160, 120)).Should().Be(new VirtualMapCoordinate(0, 0));
            converter.ScreenToMap(viewPort, new ScreenPosition(160, 120 + 32)).Should().Be(new VirtualMapCoordinate(0, 2));
            converter.ScreenToMap(viewPort, new ScreenPosition(160 + 32, 120)).Should().Be(new VirtualMapCoordinate(1, 0));
            converter.ScreenToMap(viewPort, new ScreenPosition(160 - 16, 120 + 16)).Should().Be(new VirtualMapCoordinate(0, 1));
            converter.ScreenToMap(viewPort, new ScreenPosition(160 + 16, 120 + 16)).Should().Be(new VirtualMapCoordinate(1, 1));
            converter.ScreenToMap(viewPort, new ScreenPosition(160 + 32, 120 + 32)).Should().Be(new VirtualMapCoordinate(1, 2));
        }
    }
}