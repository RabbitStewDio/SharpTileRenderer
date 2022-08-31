using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public class DiagonalSpriteMatcher<TEntityClass> : ISpriteMatcher<GraphicTag>
        where TEntityClass : struct, IEntityClassification<TEntityClass>
    {
        readonly IMapNavigator<GridDirection> navigator;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;
        readonly IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData;
        readonly TEntityClass self;
        readonly TEntityClass others;
        readonly string? prefix;
        readonly SpriteMatcherQueryHelper<TEntityClass> queryHelper;
        readonly string[] cachedSuffixes;

        public DiagonalSpriteMatcher(IMapNavigator<GridDirection> navigator,
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
            this.cachedSuffixes = PrepareSuffixes();
        }

        string[] PrepareSuffixes()
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

        public bool IsThreadSafe => dataSet.MetaData.IsThreadSafe;
        public string MatcherType => BuiltInSelectors.Diagonal;

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
                navigator.NavigateNeighbours(q.Position.Normalize(), navigationBuffer);
                var ne = queryHelper.Match(navigationBuffer[NeighbourIndex.NorthEast.AsInt()], z, others);
                var se = queryHelper.Match(navigationBuffer[NeighbourIndex.SouthEast.AsInt()], z, others);
                var sw = queryHelper.Match(navigationBuffer[NeighbourIndex.SouthWest.AsInt()], z, others);
                var nw = queryHelper.Match(navigationBuffer[NeighbourIndex.NorthWest.AsInt()], z, others);
                var key = DiagonalSelectorKey.ValueOf(ne, se, sw, nw);
                resultCollector.Add((q.TagData.AsSpriteTag().WithPrefix(prefix).WithQualifier(cachedSuffixes[key.LinearIndex]), SpritePosition.Whole, q.Position));
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
            if (model is not DiagonalSelectorModel m) throw new ArgumentException();

            var dataSet = context.ContextDataSetProducer.CreateGraphicDataSet(m.ContextDataSet ?? throw new ArgumentException());
            var matchSelf = context.ClassRegistry.FromClassNames(m.MatchSelf);
            var matchWith = m.MatchWith.Count == 0 ? matchSelf : context.ClassRegistry.FromClassNames(m.MatchWith);

            return new DiagonalSpriteMatcher<TEntityClass>(context.GridNavigator, dataSet, context.TagMetaData, matchSelf, matchWith, m.Prefix);
        }
    }
}