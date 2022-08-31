using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public class RoadParitySpriteMatcher<TEntityClass> : ISpriteMatcher<GraphicTag>
        where TEntityClass : struct, IEntityClassification<TEntityClass>
    {
        readonly IMapNavigator<GridDirection> navigator;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;
        readonly IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData;
        readonly TEntityClass self;
        readonly TEntityClass others;
        readonly string? prefix;
        readonly SpriteMatcherQueryHelper<TEntityClass> queryHelper;
        readonly string[] diagonalSuffixes;
        readonly string[] cardinalSuffixes;

        public RoadParitySpriteMatcher(IMapNavigator<GridDirection> navigator,
                                       ITileDataSet<GraphicTag, Unit> dataSet,
                                       IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData,
                                       TEntityClass self,
                                       TEntityClass others,
                                       string? prefix = null)
        {
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            this.dataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
            this.tagMetaData = tagMetaData ?? throw new ArgumentNullException(nameof(tagMetaData));
            this.self = self;
            this.others = others;
            this.prefix = prefix;
            this.queryHelper = new SpriteMatcherQueryHelper<TEntityClass>(tagMetaData, dataSet);
            this.diagonalSuffixes = PrepareDiagonalSuffixes();
            this.cardinalSuffixes = PrepareCardinalSuffixes();
        }

        string[] PrepareDiagonalSuffixes()
        {
            var values = DiagonalSelectorKey.Values;
            var result = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                var k = values[i];
                result[i] = string.Intern(k.ToString(".nw{0}ne{1}se{2}sw{3}"));
            }

            return result;
        }

        string[] PrepareCardinalSuffixes()
        {
            var values = CardinalSelectorKey.Values;
            var result = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                var k = values[i];
                result[i] = string.Intern(k.ToString(".n{0}e{1}s{2}w{3}"));
            }

            return result;
        }

        public bool IsThreadSafe => dataSet.MetaData.IsThreadSafe;
        public string MatcherType => BuiltInSelectors.RoadParity;

        public bool Match(in SpriteMatcherInput<GraphicTag> q,
                          int z,
                          List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            var selfClasses = tagMetaData.QueryClasses(q.TagData);
            if (!self.MatchesAny(selfClasses))
            {
                return false;
            }

            var navigationBuffer = ArrayPool<MapCoordinate>.Shared.Rent(8);
            try
            {
                var isolated = true;
                navigator.NavigateNeighbours(q.Position.Normalize(), navigationBuffer);
                var n = queryHelper.Match(navigationBuffer[NeighbourIndex.North.AsInt()], z, others);
                var e = queryHelper.Match(navigationBuffer[NeighbourIndex.East.AsInt()], z, others);
                var s = queryHelper.Match(navigationBuffer[NeighbourIndex.South.AsInt()], z, others);
                var w = queryHelper.Match(navigationBuffer[NeighbourIndex.West.AsInt()], z, others);
                if (n || e || s || w)
                {
                    isolated = false;
                    var key = CardinalSelectorKey.ValueOf(n, e, s, w);
                    resultCollector.Add((q.TagData.AsSpriteTag().WithPrefix(prefix).WithQualifier(cardinalSuffixes[key.LinearIndex]), SpritePosition.Whole, q.Position));
                }
                var nw = queryHelper.Match(navigationBuffer[NeighbourIndex.NorthWest.AsInt()], z, others);
                var ne = queryHelper.Match(navigationBuffer[NeighbourIndex.NorthEast.AsInt()], z, others);
                var se = queryHelper.Match(navigationBuffer[NeighbourIndex.SouthEast.AsInt()], z, others);
                var sw = queryHelper.Match(navigationBuffer[NeighbourIndex.SouthWest.AsInt()], z, others);
                if (nw || ne || se || sw)
                {
                    isolated = false;
                    var key = DiagonalSelectorKey.ValueOf(nw, ne, se, sw);
                    resultCollector.Add((q.TagData.AsSpriteTag().WithPrefix(prefix).WithQualifier(diagonalSuffixes[key.LinearIndex]), SpritePosition.Whole, q.Position));
                }

                if (isolated)
                {
                    resultCollector.Add((q.TagData.AsSpriteTag().WithPrefix(prefix).WithQualifier(".isolated"), SpritePosition.Whole, q.Position));
                }
                return true;
            }
            finally
            {
                ArrayPool<MapCoordinate>.Shared.Return(navigationBuffer);
            }
        }

        public static ISpriteMatcher<GraphicTag> Create(ISelectorModel model,
                                                        IMatcherFactory<TEntityClass> factory,
                                                        IMatchFactoryContext<TEntityClass> context)
        {
            if (model is not RoadParitySelectorModel m) throw new ArgumentException();

            var dataSet = context.ContextDataSetProducer.CreateGraphicDataSet(m.ContextDataSet ?? throw new ArgumentException());
            var matchSelf = context.ClassRegistry.FromClassNames(m.MatchSelf);
            var matchWith = m.MatchWith.Count == 0 ? matchSelf : context.ClassRegistry.FromClassNames(m.MatchWith);

            return new RoadParitySpriteMatcher<TEntityClass>(context.GridNavigator, dataSet, context.TagMetaData, matchSelf, matchWith, m.Prefix);
        }
    }
}