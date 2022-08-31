using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.Navigation.Navigators;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.Navigation
{
    public class GridNavigationTest
    {
        public static IEnumerable<NavigationTestCase> TestData
        {
            get
            {
                var data = new NavigationTestDataGenerator(new MapCoordinate(0, 0));
                yield return data.CaseData(GridDirection.NorthWest, -1, -1);
                yield return data.CaseData(GridDirection.North, 0, -1);
                yield return data.CaseData(GridDirection.NorthEast, 1, -1);
                yield return data.CaseData(GridDirection.East, 1, 0);
                yield return data.CaseData(GridDirection.SouthEast, 1, 1);
                yield return data.CaseData(GridDirection.South, 0, 1);
                yield return data.CaseData(GridDirection.SouthWest, -1, 1);
                yield return data.CaseData(GridDirection.West, -1, 0);
                yield return data.CaseData(GridDirection.None, 0, 0);
            }
        }

        [Test]
        public void ValidateNavigation()
        {
            var nav = new GridNavigator();
            TestData.ValidateAll(nav);
        }

        [Test]
        public void ValidateWrappingNormalization()
        {
            var nav = NavigatorMetaData.FromGridType(GridType.Grid).WithHorizontalWrap(10).WithVerticalLimit(5).BuildNavigator();
            nav.Navigate(GridDirection.None, new MapCoordinate(-5, -5), out var resultCoord, out var navigationInfo).Should().BeFalse();
            navigationInfo.LimitedX.Should().BeFalse();
            navigationInfo.LimitedY.Should().BeTrue();
            navigationInfo.WrapXIndicator.Should().Be(-1);
            navigationInfo.WrapYIndicator.Should().Be(0);
            resultCoord.Should().Be(new MapCoordinate(5, -5));
        }
    }
}
