using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Navigation;

namespace SharpTileRenderer.Tests.Navigation
{
    [TestFixture]
    public class LimitNavigatorTest
    {
        [Test]
        public void UpperLimitTest()
        {
            var input = new MapCoordinate(99, 0);
            var nav = new TestNavigator();
            nav.ExpectNavigateTo((GridDirection.East, input, 1), 
                       (true, new MapCoordinate(100, 0)));

            var w = nav.Limit(new Range(0, 100), new Range(0, 100));

            w.NavigateTo(GridDirection.East, input, out var m, 1).Should().BeFalse();
            m.Should().Be(new MapCoordinate(100, 0));
        }
    }
}
