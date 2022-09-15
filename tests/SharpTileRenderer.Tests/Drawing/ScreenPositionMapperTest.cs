using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Drawing.ViewPorts.ScreenMapConverters;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.Drawing
{
    public class ScreenPositionMapperTest
    {
        [Test]
        public void SimpleTest()
        {
            var viewPort = new ViewPort(NavigatorMetaData.FromGridType(GridType.Grid), TileShape.Grid, new IntDimension(32, 32));
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            var snav = viewPort.ScreenSpaceNavigator;

            var sm = new ScreenPositionMapper(GridType.Grid, new IntDimension(32, 32));
            sm.AddPhysical(new MapCoordinate(1,1), new ScreenPosition(1, 2));
            sm.AddPhysical(new MapCoordinate(1,1), new ScreenPosition(3, 4));
            sm.AddPhysical(new MapCoordinate(1,1), new ScreenPosition(5, 6));

            var list = new List<ScreenPosition>();
            sm.TryMapPhysical(viewPort, new MapCoordinate(0, 0), list).Should().BeFalse();
            sm.TryMapPhysical(viewPort, new MapCoordinate(1, 1), list).Should().BeTrue();
        }

        [Test]
        public void ValidateScreenMapping_Center()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            vp.Focus = new VirtualMapCoordinate();
            
            var origin = vp.ScreenSpaceNavigator.TranslateViewToWorld(vp, new ScreenPosition(160, 120)).VirtualCoordinate;
            origin.Should().Be(vp.Focus);

            vp.ScreenSpaceNavigator.MapInverse(vp, new ContinuousMapCoordinate()).Should().BeEquivalentTo(new ScreenPosition(160, 120));

        }

        [Test]
        public void Test_Iso()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            vp.Focus = new VirtualMapCoordinate(0.6f, 0);

            var forwardMapper = ScreenToMapConverter.Create(GridType.IsoDiamond);
            forwardMapper.ScreenToMap(vp, new ScreenPosition(160, 120)).Should().Be(vp.Focus);
            forwardMapper.ScreenToMap(vp, new ScreenPosition(160 + 16, 120)).Should().Be(vp.Focus + new VirtualMapCoordinate(0.5f, 0.5f));
        }
        
        [Test]
        public void Test_Iso2()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            vp.Focus = new VirtualMapCoordinate(0f, 0);

            var forwardMapper = ScreenToMapConverter.Create(GridType.IsoDiamond);
            forwardMapper.ScreenToMap(vp, new ScreenPosition(160, 120)).Should().Be(vp.Focus);
            forwardMapper.ScreenToMap(vp, new ScreenPosition(160 + 16, 120)).Should().Be(vp.Focus + new VirtualMapCoordinate(0.5f, 0.5f));
            forwardMapper.ScreenToMap(vp, new ScreenPosition(160 - 16, 120)).Should().Be(vp.Focus + new VirtualMapCoordinate(-0.5f, -0.5f));
        }
        
        [Test]
        public void Test_Grid()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.Grid), TileShape.Grid, new IntDimension(32, 16));
            vp.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            vp.Focus = new VirtualMapCoordinate(0.5f, 0.25f);

            var forwardMapper = ScreenToMapConverter.Create(GridType.Grid);
            forwardMapper.ScreenToMap(vp, new ScreenPosition(160, 120)).Should().Be(vp.Focus);
            forwardMapper.ScreenToMap(vp, new ScreenPosition(160 + 16, 120)).Should().Be(vp.Focus + new VirtualMapCoordinate(0.5f, 0));
            forwardMapper.ScreenToMap(vp, new ScreenPosition(160 - 16, 120)).Should().Be(vp.Focus + new VirtualMapCoordinate(-0.5f, 0));
        }
        
        [Test]
        public void ValidateScreenMapping_Offset()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            vp.Focus = new VirtualMapCoordinate(0.5f, 0.25f);
            
            var origin = vp.ScreenSpaceNavigator.TranslateViewToWorld(vp, new ScreenPosition(160, 120)).VirtualCoordinate;
            origin.Should().Be(vp.Focus);

            vp.ScreenSpaceNavigator.MapInverse(vp, new ContinuousMapCoordinate(vp.Focus.X, vp.Focus.Y)).Should().BeEquivalentTo(new ScreenPosition(160, 120));
        }

        [Test]
        public void ValidateScreenMapping_Offset_Repeat()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            vp.Focus = new VirtualMapCoordinate(0.0f, 0.0f);
            vp.Focus = new VirtualMapCoordinate(0.1f, 0.2f);
            
            var origin = vp.ScreenSpaceNavigator.TranslateViewToWorld(vp, new ScreenPosition(160, 120)).VirtualCoordinate;
            origin.Should().Be(vp.Focus);

            vp.ScreenSpaceNavigator.MapInverse(vp, new ContinuousMapCoordinate(vp.Focus.X, vp.Focus.Y)).Should().BeEquivalentTo(new ScreenPosition(160, 120));
        }

        [Test]
        public void ValidateScreenMapping_ViewToWorld()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            vp.Focus = new VirtualMapCoordinate(0.0f, 0.0f);
            
            // vp.ScreenSpaceNavigator.TranslateViewToWorld(vp, new ScreenPosition(160, 120)).VirtualCoordinate.Should().Be(vp.Focus);
            vp.ScreenSpaceNavigator.TranslateViewToWorld(vp, new ScreenPosition(160, 120 + vp.TileSize.Height)).VirtualCoordinate.Should().Be(vp.Focus + new VirtualMapCoordinate(-1, +1));
            vp.ScreenSpaceNavigator.TranslateViewToWorld(vp, new ScreenPosition(160 + vp.TileSize.Width, 120)).VirtualCoordinate.Should().Be(vp.Focus + new VirtualMapCoordinate(+1, +1));
        }

        [Test]
        public void ValidateScreenMapping_Grid_Offset_Repeat()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.Grid), TileShape.Grid, new IntDimension(32, 16));
            vp.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            vp.Focus = new VirtualMapCoordinate(0.1f, 0.2f);
            
            var origin = vp.ScreenSpaceNavigator.TranslateViewToWorld(vp, new ScreenPosition(160, 120)).VirtualCoordinate;
            origin.Should().Be(vp.Focus);

            vp.ScreenSpaceNavigator.MapInverse(vp, new ContinuousMapCoordinate(vp.Focus.X, vp.Focus.Y)).Should().BeEquivalentTo(new ScreenPosition(160, 120));
        }

        [Test]
        public void InverseMappingTest()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            vp.Focus = new VirtualMapCoordinate(0f, 0f);

            var origin = vp.ScreenSpaceNavigator.TranslateViewToWorld(vp, new ScreenPosition(160, 120)).VirtualCoordinate;
            origin.Should().Be(vp.Focus);

            var center = vp.PixelBounds.Center;
            //vp.ScreenSpaceNavigator.MapInverse(vp, new ContinuousMapCoordinate(0, 0)).Should().BeEquivalentTo(center);

            var x = vp.ScreenSpaceNavigator.MapInverse(vp, new ContinuousMapCoordinate(0, 0.5f))[0];
            Console.WriteLine("HERE " + (x - center));
            
            vp.ScreenSpaceNavigator.MapInverse(vp, new ContinuousMapCoordinate(0, 0.5f)).Should().BeEquivalentTo(center + new ScreenPosition(8, 4));
            vp.ScreenSpaceNavigator.MapInverse(vp, new ContinuousMapCoordinate(0.5f, 0f)).Should().BeEquivalentTo(center + new ScreenPosition(8, -4));
        }

        [Test]
        public void TestIsoMapping()
        {
            ScreenPositionMapper.ComputeIsoMapToScreenOffset(0.5f, 0).Should().Be((0.25f, -0.25f));
            ScreenPositionMapper.ComputeIsoMapToScreenOffset(0f, 0.5f).Should().Be((0.25f, 0.25f));
            ScreenPositionMapper.ComputeIsoMapToScreenOffset(-0.5f, 0f).Should().Be((-0.25f, 0.25f));
            ScreenPositionMapper.ComputeIsoMapToScreenOffset(0, -0.5f).Should().Be((-0.25f, -0.25f));
        }
    }
}