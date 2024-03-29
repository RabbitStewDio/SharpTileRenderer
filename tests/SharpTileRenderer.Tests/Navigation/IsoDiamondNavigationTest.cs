﻿using NUnit.Framework;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.Navigation.Navigators;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.Navigation
{
    public class IsoDiamondNavigationTest
    {
        public static IEnumerable<NavigationTestCase> TestData
        {
            get
            {
                var data = new NavigationTestDataGenerator(new MapCoordinate(0, 0));
                yield return data.CaseData(GridDirection.West, -1, 0);
                yield return data.CaseData(GridDirection.NorthWest, -1, -1);
                yield return data.CaseData(GridDirection.North, 0, -1);
                yield return data.CaseData(GridDirection.NorthEast, 1, -1);
                yield return data.CaseData(GridDirection.East, 1, 0);
                yield return data.CaseData(GridDirection.SouthEast, 1, 1);
                yield return data.CaseData(GridDirection.South, 0, 1);
                yield return data.CaseData(GridDirection.SouthWest, -1, 1);
                yield return data.CaseData(GridDirection.None, 0, 0);
            }
        }

        [Test]
        public void ValidateNavigation()
        {
            var nav = new IsoDiamondGridNavigator();
            TestData.ValidateAll(nav);
        }
    }
}
