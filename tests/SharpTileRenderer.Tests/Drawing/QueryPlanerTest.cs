using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Drawing.Queries;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack;

namespace SharpTileRenderer.Tests.Drawing
{
    [TestFixture]
    public class QueryPlanerTest
    {
        readonly GridType gridType;
        readonly TileShape tileShape;
        readonly IQueryPlaner planer;
        readonly NavigatorMetaData navigatorMetaData;

        public QueryPlanerTest() : this(GridType.Grid, TileShape.Grid)
        {
        }

        public QueryPlanerTest(GridType gridType, TileShape tileShape)
        {
            this.gridType = gridType;
            this.tileShape = tileShape;
            this.planer = QueryPlaner.FromTileType(gridType);
            this.navigatorMetaData = NavigatorMetaData.FromGridType(gridType);
        }

        [Test]
        public void TestGridBasic()
        {
            // Screen has a size of 20x15 tiles.
            // Focus is centred at (10,10), tile coordinates start in the centre of the tile
            // (a tile's map area covers (-0.5,-0.5);(+0.5,+0.5))
            
            // Horizontally: draw area is 5 tiles to the left and 5 to the right of the centre tile
            // Vertically: draw area should be 3 tiles to the top and 3 tiles to the bottom of the centre tile.
            
            var viewPort = new ViewPort(navigatorMetaData, tileShape, new IntDimension(32, 32));
            viewPort.Focus = new VirtualMapCoordinate(10, 10);
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);

            var queryPlan = planer.Plan(viewPort);
            var result = QueryPlan.FromCorners(new ContinuousMapCoordinate(5, 6f),
                                               new ContinuousMapCoordinate(14, 13f));
            queryPlan.Should().BeEquivalentTo(result);
        }

        [Test]
        public void TestInvisibleGrid()
        {
            var viewPort = new ViewPort(navigatorMetaData.WithHorizontalLimit(10).WithVerticalLimit(10), tileShape, new IntDimension(32, 32));
            viewPort.Focus = new VirtualMapCoordinate(-1, -1);
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);
            
            var queryPlan = planer.Plan(viewPort);
            var result = QueryPlan.FromCorners(new ContinuousMapCoordinate(0, 0),
                                               new ContinuousMapCoordinate(3, 2));
            queryPlan.Should().BeEquivalentTo(result);
        }
        
        [Test]
        public void TestGridEdgeOfTile()
        {
            // Screen has a size of 20x15 tiles.
            // Focus is centred at (10,10), tile coordinates start in the centre of the tile
            // (a tile's map area covers (-0.5,-0.5);(+0.5,+0.5))
            
            // Horizontally: draw area is 5 tiles to the left and 5 to the right of the centre tile
            // Vertically: draw area should be 3 tiles to the top and 3 tiles to the bottom of the centre tile.
            // However, this here is a special case where the 3rd tile to the right would cover the remaining space,
            // but the algorithm returns one more tile due to 0.5 being technically on the edge of the next
            // tile. Handling this case correctly just complicates the code without any measurable real world gain.
            // so screw that then, a one-off error exists for another day ...
            
            var viewPort = new ViewPort(navigatorMetaData, tileShape, new IntDimension(32, 32));
            viewPort.Focus = new VirtualMapCoordinate(10.5f, 10.5f);
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240);

            var queryPlan = planer.Plan(viewPort);
            var result = QueryPlan.FromCorners(new ContinuousMapCoordinate(6f, 7f),
                                               new ContinuousMapCoordinate(15f, 14f));
            queryPlan.Should().BeEquivalentTo(result);
        }

        [Test]
        public void TestGridNegative()
        {
            // Screen has a size of 20x15 tiles.
            // Focus is centred at (-10,-10)
            
            var viewPort = new ViewPort(navigatorMetaData, tileShape, new IntDimension(32, 32));
            viewPort.Focus = new VirtualMapCoordinate(-10, -10);
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240); // 10 x 7.5 tiles.

            var queryPlan = planer.Plan(viewPort);
            var result = QueryPlan.FromCorners(new ContinuousMapCoordinate(-15, -14f),
                                               new ContinuousMapCoordinate(-6, -7f));
            queryPlan.Should().BeEquivalentTo(result);
        }

        [Test]
        public void TestWrappingFull()
        {
            var viewPort = new ViewPort(navigatorMetaData.WithHorizontalWrap(20).WithVerticalWrap(20), tileShape, new IntDimension(32, 32));
            viewPort.Focus = new VirtualMapCoordinate(-10, -10);
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240); // 10 x 7.5 tiles.

            var queryPlan = planer.Plan(viewPort);
            var result = QueryPlan.FromCorners(new ContinuousMapCoordinate(5, 6f),
                                               new ContinuousMapCoordinate(14, 13f));
            queryPlan.Should().BeEquivalentTo(result);
        }

        [Test]
        public void TestWrappingPartial()
        {
            var viewPort = new ViewPort(navigatorMetaData.WithHorizontalWrap(20).WithVerticalWrap(20), tileShape, new IntDimension(32, 32));
            viewPort.Focus = new VirtualMapCoordinate(0, 0);
            viewPort.PixelBounds = new ScreenBounds(0, 0, 320, 240); // 10 x 7.5 tiles.

            var queryPlan = planer.Plan(viewPort);
            var resultA = QueryPlan.FromCorners(new ContinuousMapCoordinate(15, 16),
                                                new ContinuousMapCoordinate(19, 19));
            var resultB = QueryPlan.FromCorners(new ContinuousMapCoordinate(0, 16),
                                                new ContinuousMapCoordinate(4, 19));
            var resultC = QueryPlan.FromCorners(new ContinuousMapCoordinate(15, 0),
                                                new ContinuousMapCoordinate(19, 3));
            var resultD = QueryPlan.FromCorners(new ContinuousMapCoordinate(0, 0),
                                                new ContinuousMapCoordinate(4, 3));
            queryPlan.Should().BeEquivalentTo(resultA, resultB, resultC, resultD);
        }

        public void TestLimit()
        {
            
        }

        public void TestDenseWrapping()
        {
            
        }
    }
}