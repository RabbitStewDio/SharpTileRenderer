using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Navigation;

namespace SharpTileRenderer.Tests.Navigation
{
    [TestFixture]
    public class WrapAroundNavigatorTest
    {
        [Test]
        public void UpperLimitTest()
        {
            var input = new MapCoordinate(99, 0);
            var nav = new TestNavigator();
            nav.ExpectNavigateTo((GridDirection.East, input, 1), 
                (true, new MapCoordinate(100, 0)));

            var w = nav.Wrap(new Range(0, 100), new Range(0, 100));
            w.NavigateTo(GridDirection.East, input, out var m).Should().BeTrue();
            m.Should().Be(new MapCoordinate(0, 0));
        }

        [Test]
        public void LowerLimitTest()
        {
            var input = new MapCoordinate(99, 0);
            var nav = new TestNavigator();
            nav.ExpectNavigateTo((GridDirection.North, input, 1), 
                       (true, new MapCoordinate(99, -1)));

            var w = nav.Wrap(new Range(0, 100), new Range(0, 100));

            w.NavigateTo(GridDirection.North, input, out var m).Should().BeTrue();
            m.Should().Be(new MapCoordinate(99, 99));
        }
    }
}
