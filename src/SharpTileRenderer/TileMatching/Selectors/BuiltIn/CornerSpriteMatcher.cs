using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.TileMatching.Selectors.TileTags;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    /// <summary>
    ///   Splits the tile into 4 sub-tiles, each of which is matched independently against their 3 neighbours in clockwise rotation.
    ///
    ///              /\                  North
    ///             /  \                  /|\
    ///            /\up/\                  | 
    ///           /  \/  \         West    |    East
    ///           \le/\ri/
    ///            \/  \/
    ///             \do/                South
    ///              \/
    /// 
    ///   * North-west corner is named "u" (for up) and matched against its northwest (NW), north (N) and northeast (NE) neighbour tile.
    ///   * South-East corner is named "r" (for right) and matched against its NE, E and SE neighbour tile.
    ///   * North-west corner is named "d" (for down) and matched against its SE, S and SW neighbour tile.
    ///   * North-west corner is named "l" (for left) and matched against its SW, W and NW neighbour tile.
    ///   
    ///   The resulting sprite is named according to the pattern ("prefix", "tile", "u001").                        
    /// 
    /// </summary>
    public class CornerSpriteMatcher<TClass> : ISpriteMatcher<GraphicTag>
        where TClass : struct, IEntityClassification<TClass>
    {
        static readonly string[] directionSelectors = { "up", "right", "down", "left" };
        static readonly ITileTagEntrySelectionFactory<string> directionFactory = TileTagEntrySelectionFactory.FromTagsAsSingleCharKey(directionSelectors);

        readonly IMapNavigator<GridDirection> navigator;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;
        readonly string[] tiles;
        readonly SelectorDefinition[] selectors;
        readonly MatchStrategy matcher;
        readonly SpriteTag[] preparedSpriteTags;

        public string MatcherType => BuiltInSelectors.Corner;
        public bool IsThreadSafe => dataSet.MetaData.IsThreadSafe;

        public CornerSpriteMatcher(IMapNavigator<GridDirection> navigator,
                                   ITileDataSet<GraphicTag, Unit> dataSet,
                                   IGraphicTagMetaDataRegistry<TClass> tagRegistry,
                                   EntityClassificationRegistry<TClass> classRegistry,
                                   TClass matchSet,
                                   TClass defaultMatch,
                                   string? prefix)
        {
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            this.dataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));

            tagRegistry = tagRegistry ?? throw new ArgumentNullException(nameof(tagRegistry));
            classRegistry = classRegistry ?? throw new ArgumentNullException(nameof(classRegistry));
            var graphicTagToClassMapping = ClassSelectorBuilder.PrepareSelectionTags(tagRegistry, classRegistry, matchSet, out var cardinality);
            this.tiles = PrepareResultMappings(cardinality);
            var tileTagEntrySelections = cardinality.TryLookup(defaultMatch).Cast<ITileTagEntrySelection>();
            this.matcher = new MatchStrategy(dataSet, graphicTagToClassMapping, tileTagEntrySelections);

            this.selectors = PrepareSelectors();
            this.preparedSpriteTags = new SpriteTag[tiles.Length];
            for (var index = 0; index < tiles.Length; index++)
            {
                preparedSpriteTags[index] = SpriteTag.Create(prefix, null, tiles[index]);
            }
        }

        string[] PrepareResultMappings(TileTagEntrySelectionFactory<TClass> owner)
        {
            var cardinality = owner.Count;
            var result = new string[directionFactory.Count * cardinality * cardinality * cardinality];
            for (int directionIndex = 0; directionIndex < directionFactory.Count; directionIndex += 1)
            {
                for (int b = 0; b < cardinality; b += 1)
                {
                    for (int c = 0; c < cardinality; c += 1)
                    {
                        for (int d = 0; d < cardinality; d += 1)
                        {
                            var key = new CellGroupSelectorKey(directionFactory[directionIndex], owner[b], owner[c], owner[d]);
                            result[key.LinearIndex] = key.FormatSuffix("_cell_{0}_{1}_{2}_{3}");
                        }
                    }
                }
            }

            return result;
        }

        public bool Match(in SpriteMatcherInput<GraphicTag> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            if (!matcher.TryMatch(q.Position.Normalize(), z, out _))
            {
                return false;
            }

            var buffer = ArrayPool<MapCoordinate>.Shared.Rent(8);
            try
            {
                bool result = false;
                navigator.NavigateNeighbours(q.Position.Normalize(), buffer);
                for (var i = 0; i < selectors.Length; i++)
                {
                    var selectorKey = directionFactory[i];
                    var selector = selectors[i];
                    if (matcher.TryMatch(buffer[selector.SelectorCoordinates[0]], z, out var b) &&
                        matcher.TryMatch(buffer[selector.SelectorCoordinates[1]], z, out var c) &&
                        matcher.TryMatch(buffer[selector.SelectorCoordinates[2]], z, out var d))
                    {
                        var linearIndex = CellGroupSelectorKey.LinearIndexOf(selectorKey, b, c, d);
                        var tile = preparedSpriteTags[linearIndex].With(q.TagData);
                        // var tile = SpriteTag.Create(prefix, q.TagData.Id, tiles[linearIndex]);
                        resultCollector.Add((tile, selector.SpritePosition, q.Position));
                        result = true;
                    }
                }

                return result;
            }
            finally
            {
                ArrayPool<MapCoordinate>.Shared.Return(buffer);
            }
        }

        static SelectorDefinition[] PrepareSelectors()
        {
            return new[]
            {
                new SelectorDefinition(SpritePosition.Up,
                                       NeighbourIndex.North.AsInt(),
                                       NeighbourIndex.NorthEast.AsInt(),
                                       NeighbourIndex.East.AsInt()),
                new SelectorDefinition(SpritePosition.Right,
                                       NeighbourIndex.East.AsInt(),
                                       NeighbourIndex.SouthEast.AsInt(),
                                       NeighbourIndex.South.AsInt()),
                new SelectorDefinition(SpritePosition.Down,
                                       NeighbourIndex.South.AsInt(),
                                       NeighbourIndex.SouthWest.AsInt(),
                                       NeighbourIndex.West.AsInt()),
                new SelectorDefinition(SpritePosition.Left,
                                       NeighbourIndex.West.AsInt(),
                                       NeighbourIndex.NorthWest.AsInt(),
                                       NeighbourIndex.North.AsInt())
            };
        }

        readonly struct SelectorDefinition
        {
            public readonly SpritePosition SpritePosition;
            public readonly int[] SelectorCoordinates;

            public SelectorDefinition(SpritePosition spritePos,
                                      params int[] selectorCoordinates)
            {
                SpritePosition = spritePos;
                SelectorCoordinates = selectorCoordinates;
            }
        }

        public static ISpriteMatcher<GraphicTag> Create(ISelectorModel model,
                                                        IMatcherFactory<TClass> factory,
                                                        IMatchFactoryContext<TClass> context)
        {
            if (model is not CornerSelectorModel m)
            {
                throw new ArgumentException();
            }

            var dataSet = context.ContextDataSetProducer.CreateGraphicDataSet(m.ContextDataSet ?? throw new ArgumentException());
            var matches = context.ClassRegistry.FromClassNames(m.Matches);
            var defaultMatch = context.ClassRegistry.TryGetClassification(m.DefaultClass ?? "", out var ma) ? ma : default;
            var spriteTag = m.Prefix ?? throw new ArgumentException();

            return new CornerSpriteMatcher<TClass>(context.GridNavigator, dataSet, context.TagMetaData, context.ClassRegistry,
                                                   matches, defaultMatch, spriteTag);
        }
    }
}