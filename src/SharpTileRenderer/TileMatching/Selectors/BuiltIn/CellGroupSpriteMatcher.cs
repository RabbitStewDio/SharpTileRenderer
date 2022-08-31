using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.TileMatching.Selectors.TileTags;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public class CellGroupSpriteMatcher<TClass> : ISpriteMatcher<GraphicTag>
        where TClass : struct, IEntityClassification<TClass>
    {
        delegate void NavigateDelegate(in MapCoordinate origin,
                                       out MapCoordinate coordA,
                                       out MapCoordinate coordB,
                                       out MapCoordinate coordC,
                                       out MapCoordinate coordD);

        readonly IMapNavigator<GridDirection> navigator;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;
        readonly string baseSpriteTag;
        readonly SpriteTag[] tiles;
        readonly NavigateDelegate navigationStrategy;
        readonly MatchStrategy matcher;

        public CellGroupSpriteMatcher(IMapNavigator<GridDirection> navigator,
                                      ITileDataSet<GraphicTag, Unit> dataSet,
                                      IGraphicTagMetaDataRegistry<TClass> tagRegistry,
                                      EntityClassificationRegistry<TClass> classRegistry,
                                      TClass matchSet,
                                      TClass defaultMatch,
                                      string baseSpriteTag,
                                      CellGroupNavigationDirection navigationDirection)
        {
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            this.dataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
            tagRegistry = tagRegistry ?? throw new ArgumentNullException(nameof(tagRegistry));
            classRegistry = classRegistry ?? throw new ArgumentNullException(nameof(classRegistry));
            this.baseSpriteTag = baseSpriteTag ?? throw new ArgumentNullException(nameof(baseSpriteTag));

            var graphicTagToClassMapping = ClassSelectorBuilder.PrepareSelectionTags(tagRegistry, classRegistry, matchSet, out var cardinality);
            this.tiles = PrepareResultMappings(cardinality);
            var tileTagEntrySelections = cardinality.TryLookup(defaultMatch).Cast<ITileTagEntrySelection>();
            this.matcher = new MatchStrategy(dataSet, graphicTagToClassMapping, tileTagEntrySelections);

            if (this.navigator.MetaData.GridType == GridType.Grid)
            {
                this.navigationStrategy = navigationDirection switch
                {
                    CellGroupNavigationDirection.Down => NavigateForDownwardRenderDirectionGrid,
                    CellGroupNavigationDirection.Up => NavigateForUpwardRenderDirectionGrid,
                    _ => throw new ArgumentException()
                };
            }
            else
            {
                this.navigationStrategy = navigationDirection switch
                {
                    CellGroupNavigationDirection.Down => NavigateForDownwardRenderDirectionIso,
                    CellGroupNavigationDirection.Up => NavigateForUpwardRenderDirectionIso,
                    _ => throw new ArgumentException()
                };
            }
        }

        SpriteTag[] PrepareResultMappings(TileTagEntrySelectionFactory<TClass> owner)
        {
            var cardinality = owner.Count;
            var result = new SpriteTag[cardinality * cardinality * cardinality * cardinality];
            for (int a = 0; a < cardinality; a += 1)
            {
                for (int b = 0; b < cardinality; b += 1)
                {
                    for (int c = 0; c < cardinality; c += 1)
                    {
                        for (int d = 0; d < cardinality; d += 1)
                        {
                            var key = new CellGroupSelectorKey(owner[a], owner[b], owner[c], owner[d]);
                            result[key.LinearIndex] = SpriteTag.Create(null, baseSpriteTag, key.FormatSuffix());
                        }
                    }
                }
            }

            return result;
        }

        public string MatcherType => BuiltInSelectors.CellGroup;
        public bool IsThreadSafe => dataSet.MetaData.IsThreadSafe;

        void NavigateForUpwardRenderDirectionIso(in MapCoordinate origin,
                                                 out MapCoordinate coordA,
                                                 out MapCoordinate coordB,
                                                 out MapCoordinate coordC,
                                                 out MapCoordinate coordD)
        {
            //
            //       /\
            //      /A \
            //     /\ 2/\
            //    / D\/ B \
            //    \1 /\ 3 /
            //     \/ C\/
            //      \**/
            //       \/
            //
            // tile_cell_A_B_C_D
            // self is at C

            coordC = origin;
            navigator.NavigateTo(GridDirection.North, coordC, out coordA);
            navigator.NavigateTo(GridDirection.NorthEast, coordC, out coordB);
            navigator.NavigateTo(GridDirection.NorthWest, coordC, out coordD);
        }

        void NavigateForDownwardRenderDirectionIso(in MapCoordinate origin,
                                                   out MapCoordinate coordA,
                                                   out MapCoordinate coordB,
                                                   out MapCoordinate coordC,
                                                   out MapCoordinate coordD)
        {
            //
            //       /\
            //      /A \
            //     /\**/\       N 
            //    / D\/ B\     / \ 
            //    \  /\  /    / | \   
            //     \/ C\/       |   
            //      \  / 
            //       \/
            //
            // tile_cell_A_B_C_D
            // self is A
            coordA = origin;
            navigator.NavigateTo(GridDirection.South, origin, out coordC);
            navigator.NavigateTo(GridDirection.SouthWest, origin, out coordD);
            navigator.NavigateTo(GridDirection.SouthEast, origin, out coordB);
        }


        void NavigateForUpwardRenderDirectionGrid(in MapCoordinate origin,
                                                  out MapCoordinate coordA,
                                                  out MapCoordinate coordB,
                                                  out MapCoordinate coordC,
                                                  out MapCoordinate coordD)
        {
            //
            //    A B
            //    D C
            //
            // tile_cell_A_B_C_D
            // self is at C

            coordC = origin;
            navigator.NavigateTo(GridDirection.NorthWest, origin, out coordA);
            navigator.NavigateTo(GridDirection.North, origin, out coordB);
            navigator.NavigateTo(GridDirection.West, origin, out coordD);
        }

        void NavigateForDownwardRenderDirectionGrid(in MapCoordinate origin,
                                                    out MapCoordinate coordA,
                                                    out MapCoordinate coordB,
                                                    out MapCoordinate coordC,
                                                    out MapCoordinate coordD)
        {
            //
            //    A B
            //    D C
            //
            // tile_cell_A_B_C_D
            // self is A
            coordA = origin;
            navigator.NavigateTo(GridDirection.East, origin, out coordB);
            navigator.NavigateTo(GridDirection.SouthEast, origin, out coordC);
            navigator.NavigateTo(GridDirection.South, origin, out coordD);
        }


        public bool Match(in SpriteMatcherInput<GraphicTag> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            navigationStrategy(q.Position.Normalize(),
                               out MapCoordinate coordA,
                               out MapCoordinate coordB,
                               out MapCoordinate coordC,
                               out MapCoordinate coordD);

            if (matcher.TryMatch(coordA, z, out var matchA) &&
                matcher.TryMatch(coordB, z, out var matchB) &&
                matcher.TryMatch(coordC, z, out var matchC) &&
                matcher.TryMatch(coordD, z, out var matchD))
            {
                var linearIndex = CellGroupSelectorKey.LinearIndexOf(matchA, matchB, matchC, matchD);
                var tile = tiles[linearIndex];
                resultCollector.Add((tile, SpritePosition.CellMap, q.Position));
                return true;
            }

            return false;
        }

        public static ISpriteMatcher<GraphicTag> Create(ISelectorModel model,
                                                        IMatcherFactory<TClass> factory,
                                                        IMatchFactoryContext<TClass> context)
        {
            if (model is not CellGroupSelectorModel m)
            {
                throw new ArgumentException();
            }

            var dataSet = context.ContextDataSetProducer.CreateGraphicDataSet(m.ContextDataSet ?? throw new ArgumentException());
            var matches = context.ClassRegistry.FromClassNames(m.Matches);
            var defaultMatch = context.ClassRegistry.TryGetClassification(m.DefaultClass ?? "", out var ma) ? ma : default;
            var direction = m.Direction;
            var spriteTag = m.Prefix ?? throw new ArgumentException();

            return new CellGroupSpriteMatcher<TClass>(context.GridNavigator, dataSet, context.TagMetaData, context.ClassRegistry,
                                                      matches, defaultMatch, spriteTag, direction);
        }
    }
}