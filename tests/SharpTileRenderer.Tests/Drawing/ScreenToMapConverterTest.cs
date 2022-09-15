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
        public void TestGridMapping()
        {
            var converter = ScreenToMapConverter.Create(GridType.Grid);
            var viewPort = new ViewPort(NavigatorMetaData.FromGridType(GridType.Grid), TileShape.Grid, new IntDimension(32, 32));
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);

            converter.ScreenToMap(viewPort, new ScreenPosition(0, 0)).Should().Be(new VirtualMapCoordinate(-5, -3.75f));
            viewPort.ScreenSpaceNavigator.TranslateViewToWorld(viewPort, new ScreenPosition(0, 0))
                    .Should()
                    .Be(new WorldPosition(new ContinuousMapCoordinate(-5, -3.75f), new VirtualMapCoordinate(-5, -3.75f), new NavigationInfo(0, 0, false, false)));
        }

        [Test]
        public void TestIsometricMapping()
        {
            var converter = ScreenToMapConverter.Create(GridType.IsoDiamond);
            var viewPort = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 32));
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            var snav = viewPort.ScreenSpaceNavigator;

            converter.ScreenToMap(viewPort, viewPort.PixelBounds.Center).Should().Be(new VirtualMapCoordinate(0, 0));
            converter.ScreenToMap(viewPort, new ScreenPosition(0, 0)).Should().Be(new VirtualMapCoordinate(-1.25f, -8.75f));
            snav.TranslateViewToWorld(viewPort, new ScreenPosition(0, 0))
                .Should()
                .Be(new WorldPosition(new ContinuousMapCoordinate(-1.25f, -8.75f), new VirtualMapCoordinate(-1.25f, -8.75f), new NavigationInfo()));
        }

        [Test]
        public void TestScreenMapping_Iso()
        {
            var viewPort = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 32));
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);

            var converter = ScreenToMapConverter.Create(GridType.IsoDiamond);
            converter.ScreenToMap(viewPort, viewPort.PixelBounds.Center).Should().Be(new VirtualMapCoordinate(0, 0));
            converter.ScreenToMap(viewPort, viewPort.PixelBounds.Center + new ScreenPosition(0, viewPort.TileSize.Height)).Should().Be(new VirtualMapCoordinate(-1, 1));
            converter.ScreenToMap(viewPort, viewPort.PixelBounds.Center + new ScreenPosition(viewPort.TileSize.Width, 0)).Should().Be(new VirtualMapCoordinate(+1, 1));
        }
    }
}