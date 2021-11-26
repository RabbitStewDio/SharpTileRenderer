using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.Registry;

namespace SharpTileRenderer.TileMatching.Sprites
{
    public delegate bool BlendNeighbourLookupFn<TRenderTile>(int x, int y, CardinalIndex d, out TRenderTile t);

    public class BlendNeighboursSelector<TRenderTile, TContext> : ITileMatcher<TRenderTile, TContext>
    {
        readonly ITileRegistryEx<CardinalIndex, TRenderTile> registry;
        readonly IMapNavigator<GridDirection> gridNavigator;
        readonly MapQuery<TContext> contextProvider;
        readonly CardinalIndex[] directions;
        readonly MapQuery<string> mapQuery;
        readonly MapQuery<bool> isBlending;
        MapCoordinate[] mapCoords;

        public BlendNeighboursSelector(ITileRegistryEx<CardinalIndex, TRenderTile> registry,
                                        IMapNavigator<GridDirection> gridNavigator,
                                        MapQuery<string> mapQuery,
                                        MapQuery<bool> isBlending,
                                        MapQuery<TContext> contextProvider = null)
        {
            this.registry = registry;
            this.gridNavigator = gridNavigator;
            this.mapQuery = mapQuery;
            this.isBlending = isBlending;
            this.contextProvider = contextProvider ?? DefaultContextProvider;
            this.directions = new[] {CardinalIndex.North, CardinalIndex.East, CardinalIndex.South, CardinalIndex.West};
        }

        static TContext DefaultContextProvider(int x, int y)
        {
            return default(TContext);
        }

        public bool Match(int x, int y, TileResultCollector<TRenderTile, TContext> resultCollector)
        {
            mapCoords = gridNavigator.NavigateCardinalNeighbours(new MapCoordinate(x, y), mapCoords);
            var blendSelf = isBlending(x, y);
            var retval = false;
            for (var i = 0; i < mapCoords.Length; i++)
            {
                var c = mapCoords[i];
                if (!blendSelf && !isBlending(c.X, c.Y))
                {
                    continue;
                }

                var mq = mapQuery(c.X, c.Y);
                if (mq != null && registry.TryFind(mq, directions[i], out var tile))
                {
                    resultCollector(SpritePosition.Whole, tile, contextProvider(x, y));
                    retval = true;
                }
            }

            return retval;
        }
    }
}
