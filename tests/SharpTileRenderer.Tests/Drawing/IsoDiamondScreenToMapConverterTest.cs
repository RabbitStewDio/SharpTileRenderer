﻿using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;

namespace SharpTileRenderer.Tests.Drawing
{
    [TestFixture]
    public class IsoDiamondScreenToMapConverterTest
    {
        ViewPort viewPort;
        IsoDiamondScreenToMapConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new IsoDiamondScreenToMapConverter();
            viewPort = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 32));
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);
        }

        [Test]
        public void Test()
        {
            converter.ScreenToMap(viewPort, new ScreenPosition(0, 0)).Should().Be(new VirtualMapCoordinate(-8.75f, 1.25f));
            converter.ScreenToMap(viewPort, new ScreenPosition(160, 120)).Should().Be(new VirtualMapCoordinate(0, 0));
            converter.ScreenToMap(viewPort, new ScreenPosition(160 + 16, 120 + 16)).Should().Be(new VirtualMapCoordinate(1, 0));
            converter.ScreenToMap(viewPort, new ScreenPosition(160 + 32, 120 + 32)).Should().Be(new VirtualMapCoordinate(2, 0));
        }
    }
}