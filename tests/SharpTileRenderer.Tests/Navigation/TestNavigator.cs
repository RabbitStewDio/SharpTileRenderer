using NUnit.Framework;
using SharpTileRenderer.Navigation;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.Navigation
{
    class TestNavigator: IMapNavigator<GridDirection>
    {
        readonly Dictionary<(GridDirection, MapCoordinate, int), (bool, MapCoordinate)> expectedCalls;
        readonly Dictionary<(GridDirection, MapCoordinate, int),(bool, MapCoordinate, NavigationInfo)> expectedCalls2;

        public TestNavigator()
        {
            expectedCalls = new Dictionary<(GridDirection, MapCoordinate, int), (bool, MapCoordinate)>();
            expectedCalls2 = new Dictionary<(GridDirection, MapCoordinate, int), (bool, MapCoordinate, NavigationInfo)>();
        }

        public void ExpectNavigateTo((GridDirection direction, MapCoordinate source, int steps) parameter, (bool, MapCoordinate) result)
        {
            expectedCalls[parameter] = result;
        }
            
        public void ExpectNavigate((GridDirection direction, MapCoordinate source, int steps) parameter, (bool, MapCoordinate) result)
        {
            expectedCalls[parameter] = result;
        }
            
        public bool NavigateTo(GridDirection direction, in MapCoordinate origin, out MapCoordinate result, int steps = 1)
        {
            var key = (direction, source: origin, steps);
            if (expectedCalls.TryGetValue(key, out var r))
            {
                result = r.Item2;
                return r.Item1;
            }

            throw new AssertionException("Unexpected call");
        }

        public bool Navigate(GridDirection direction, in MapCoordinate origin, out MapCoordinate result, out NavigationInfo info, int steps = 1)
        {
            var key = (direction, source: origin, steps);
            if (expectedCalls2.TryGetValue(key, out var r))
            {
                result = r.Item2;
                info = r.Item3;
                return r.Item1;
            }

            throw new AssertionException("Unexpected call");
        }

        public NavigatorMetaData MetaData => NavigatorMetaData.FromGridType(GridType.Grid);
    }
}