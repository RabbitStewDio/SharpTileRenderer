using Microsoft.Extensions.ObjectPool;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.TileMatching.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace SharpTileRenderer.TileBlending.Matcher
{
    public class BlendingSpriteMatcher<TEntityClass> : ISpriteMatcher<GraphicTag>
        where TEntityClass : struct, IEntityClassification<TEntityClass>
    {
        readonly IGraphicTagMetaDataRegistry<TEntityClass> tagMeta;
        readonly TEntityClass self;
        readonly TEntityClass others;
        readonly IMapNavigator<GridDirection> navigator;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;
        readonly TextureQuadrantIndex[] directions;
        readonly ObjectPool<List<SparseTagQueryResult<GraphicTag, Unit>>> queryPool;
        readonly string prefix;

        public string MatcherType => BlendingSelectorModel.SelectorName;
        public bool IsThreadSafe => dataSet.MetaData.IsThreadSafe;

        public BlendingSpriteMatcher(IMapNavigator<GridDirection> navigator,
                                     ITileDataSet<GraphicTag, Unit> dataSet,
                                     IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData,
                                     TEntityClass self,
                                     TEntityClass others,
                                     string? prefix = null)
        {
            this.tagMeta = tagMetaData ?? throw new ArgumentNullException(nameof(tagMetaData));
            this.self = self;
            this.others = others;
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            this.dataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
            this.prefix = string.IsNullOrEmpty(prefix) ? "t.blend." : prefix;
            this.queryPool = new DefaultObjectPool<List<SparseTagQueryResult<GraphicTag, Unit>>>(new ListObjectPolicy<SparseTagQueryResult<GraphicTag, Unit>>());
            this.directions = new[] { TextureQuadrantIndex.North, TextureQuadrantIndex.East, TextureQuadrantIndex.South, TextureQuadrantIndex.West };
        }

        public bool Match(in SpriteMatcherInput<GraphicTag> q,
                          int z,
                          List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            var selfClasses = tagMeta.QueryClasses(q.TagData);
            var blendSelf = selfClasses.MatchesAny(this.self);
            var navigationBuffer = ArrayPool<MapCoordinate>.Shared.Rent(4);
            try
            {
                navigator.NavigateCardinalNeighbours(q.Position.Normalize(), navigationBuffer);

                for (var i = 0; i < 4; i++)
                {
                    var pos = navigationBuffer[i];
                    var dir = directions[i];
                    var queryResult = queryPool.Get();
                    try
                    {
                        dataSet.QueryPoint(pos, z, queryResult);
                        for (var qrIdx = 0; qrIdx < queryResult.Count; qrIdx++)
                        {
                            var qr = queryResult[qrIdx];
                            var blend = tagMeta.QueryClasses(qr.TagData).MatchesAny(others);
                            if (!blendSelf && !blend)
                            {
                                continue;
                            }

                            var spriteTag = qr.TagData.AsSpriteTag().WithPrefix(prefix).WithQualifier(BlendSuffixFor(dir));
                            resultCollector.Add((spriteTag, SpritePosition.Whole, q.Position));
                        }
                    }
                    finally
                    {
                        queryResult.Clear();
                        queryPool.Return(queryResult);
                    }
                }
            }
            finally
            {
                ArrayPool<MapCoordinate>.Shared.Return(navigationBuffer);
            }

            return true;
        }

        public static string BlendSuffixFor(TextureQuadrantIndex idx) => idx switch
        {
            TextureQuadrantIndex.North => "_north",
            TextureQuadrantIndex.East => "_east",
            TextureQuadrantIndex.South => "_south",
            TextureQuadrantIndex.West => "_west",
            _ => throw new ArgumentOutOfRangeException(nameof(idx), idx, null)
        };

        public static ISpriteMatcher<GraphicTag> Create(ISelectorModel model,
                                                        IMatcherFactory<TEntityClass> factory,
                                                        IMatchFactoryContext<TEntityClass> context)
        {
            if (model is not BlendingSelectorModel m) throw new ArgumentException();

            var dataSet = context.ContextDataSetProducer.CreateGraphicDataSet(m.ContextDataSet ?? throw new ArgumentException());
            var matchSelf = context.ClassRegistry.FromClassNames(m.MatchSelf);
            var matchWith = m.MatchWith.Count == 0 ? matchSelf : context.ClassRegistry.FromClassNames(m.MatchWith);

            return new BlendingSpriteMatcher<TEntityClass>(context.GridNavigator, dataSet, context.TagMetaData, matchSelf, matchWith, m.Prefix);
        }
    }
}