using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;

namespace SharpTileRenderer.Tests.Drawing
{
    public class ViewPortTest
    {
        [Test]
        public void ValidateMapNavigation_Iso_North()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.Navigation[MapNavigationType.Map].Navigate(GridDirection.North, new MapCoordinate(), out var mapResult, out _).Should().BeTrue();
            mapResult.Should().Be(new MapCoordinate(0, -1), "because north is up along the y axis on the map");
        }
    
        [Test]
        public void ValidateScreenNavigation_Iso_North()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.Navigation[MapNavigationType.Screen].Navigate(GridDirection.North, new MapCoordinate(), out var mapResult, out _).Should().BeTrue();
            mapResult.Should().Be(new MapCoordinate(+1, -1), "because north is up along the screen axis");
        }
        
        [Test]
        public void ValidateMapNavigation_Iso_East()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.Navigation[MapNavigationType.Map].Navigate(GridDirection.East, new MapCoordinate(), out var mapResult, out _).Should().BeTrue();
            mapResult.Should().Be(new MapCoordinate(1, 0), "because north is up along the y axis on the map");
        }
    
        [Test]
        public void ValidateScreenNavigation_Iso_East()
        {
            var vp = new ViewPort(NavigatorMetaData.FromGridType(GridType.IsoDiamond), TileShape.Isometric, new IntDimension(32, 16));
            vp.Navigation[MapNavigationType.Screen].Navigate(GridDirection.East, new MapCoordinate(), out var mapResult, out _).Should().BeTrue();
            mapResult.Should().Be(new MapCoordinate(+1, 1), "because north is up along the screen axis");
        }
    }
}