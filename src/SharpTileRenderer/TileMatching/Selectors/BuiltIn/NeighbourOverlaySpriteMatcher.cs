using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    /// <summary>
    ///    A matcher that connects the current tile with any N8-neighbour cell. This will
    ///    draw up to 8 sprites that overlay each other. If none of the neighbours matches,
    ///    an 'isolated' tile will be drawn instead.
    /// </summary>
    /// <typeparam name="TEntityClass"></typeparam>
    public class NeighbourOverlaySpriteMatcher<TEntityClass> : ISpriteMatcher<GraphicTag>
        where TEntityClass : struct, IEntityClassification<TEntityClass>
    {
        static readonly string[] suffix = { "_n", "_ne", "_e", "_se", "_s", "_sw", "_w", "_nw" };
        readonly IMapNavigator<GridDirection> navigator;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;
        readonly TEntityClass self;
        readonly TEntityClass others;
        readonly Optional<GraphicTag> forceGraphic;
        readonly string? prefix;
        readonly SpriteMatcherQueryHelper<TEntityClass> queryHelper;
        readonly IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData;

        public NeighbourOverlaySpriteMatcher(IMapNavigator<GridDirection> navigator,
                                             ITileDataSet<GraphicTag, Unit> dataSet,
                                             IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData,
                                             TEntityClass self,
                                             TEntityClass others,
                                             Optional<GraphicTag> forceGraphic,
                                             string? prefix)
        {
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            this.dataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
            this.tagMetaData = tagMetaData ?? throw new ArgumentNullException(nameof(tagMetaData));
            this.self = self;
            this.others = others;
            this.forceGraphic = forceGraphic;
            this.prefix = prefix;
            this.queryHelper = new SpriteMatcherQueryHelper<TEntityClass>(tagMetaData, dataSet);
        }

        public bool IsThreadSafe => dataSet.MetaData.IsThreadSafe;
        public string MatcherType => BuiltInSelectors.NeighbourOverlay;

        public bool Match(in SpriteMatcherInput<GraphicTag> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            var selfClasses = tagMetaData.QueryClasses(q.TagData);
            if (!self.MatchesAny(selfClasses))
            {
                return false;
            }

            if (!this.forceGraphic.TryGetValue(out var tag))
            {
                tag = q.TagData;
            }

            var buffer = ArrayPool<MapCoordinate>.Shared.Rent(8);
            try
            {
                var matchedOnce = false;
                navigator.NavigateNeighbours(q.Position.Normalize(), buffer);
                for (var index = 0; index < 8; index++)
                {
                    var mapCoordinate = buffer[index];
                    if (!queryHelper.Match(mapCoordinate, z, others))
                    {
                        continue;
                    }

                    matchedOnce = true;
                    resultCollector.Add((tag.AsSpriteTag().WithPrefix(prefix).WithQualifier(suffix[index]), SpritePosition.Whole, q.Position));
                }

                if (!matchedOnce)
                {
                    resultCollector.Add((tag.AsSpriteTag().WithPrefix(prefix).WithQualifier("_isolated"), SpritePosition.Whole, q.Position));
                }

                return true;
            }
            finally
            {
                ArrayPool<MapCoordinate>.Shared.Return(buffer);
            }
        }

        public static ISpriteMatcher<GraphicTag> Create(ISelectorModel model,
                                                        IMatcherFactory<TEntityClass> factory,
                                                        IMatchFactoryContext<TEntityClass> context)
        {
            if (model is not NeighbourOverlaySelectorModel m) throw new ArgumentException();

            var dataSet = context.ContextDataSetProducer.CreateGraphicDataSet(m.ContextDataSet ?? throw new ArgumentException());
            var matchSelf = context.ClassRegistry.FromClassNames(m.MatchSelf);
            var matchWith = m.MatchWith.Count == 0 ? matchSelf : context.ClassRegistry.FromClassNames(m.MatchWith);
            var forceGraphicTag = m.ForceGraphic == null ? Optional.Empty<GraphicTag>() : Optional.ValueOf(GraphicTag.From(m.ForceGraphic)); 

            return new NeighbourOverlaySpriteMatcher<TEntityClass>(context.GridNavigator, dataSet, context.TagMetaData, matchSelf, matchWith, forceGraphicTag, m.Prefix);
        }
    }
}