using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    /// <summary>
    ///    A cardinal direction wild-card matcher. After matching for self, checks the cardinal neighbours for
    ///    class-membership and draws a directional capstone sprite towards that match.
    ///
    ///    This is derived from FreeCiv's river outlet drawing - where the self-match checks for lake or ocean,
    ///    and the cardinal match checks for rivers. If matching, it draws a river delta from the direction of
    ///    the river. 
    /// </summary>
    /// <remarks>
    ///    As we know that any produced sprite will be sorted anyway, we can safely reverse the matching and
    ///    use the river data set as primary data set (there should be less river tiles than ocean tiles in normal
    ///    maps).  
    /// </remarks>
    /// <typeparam name="TEntityClass"></typeparam>
    public class RiverOutletSpriteMatcher<TEntityClass>: ISpriteMatcher<GraphicTag> 
        where TEntityClass : struct, IEntityClassification<TEntityClass>
    {
        readonly IMapNavigator<GridDirection> navigator;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;
        readonly TEntityClass self;
        readonly TEntityClass others;
        readonly string prefix;
        readonly SpriteMatcherQueryHelper<TEntityClass> queryHelper;
        readonly IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData;
        public string MatcherType => BuiltInSelectors.RiverOutlet;
        public bool IsThreadSafe => dataSet.MetaData.IsThreadSafe;

        public RiverOutletSpriteMatcher(IMapNavigator<GridDirection> navigator,
                                        ITileDataSet<GraphicTag, Unit> dataSet,
                                        IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData,
                                        TEntityClass self,
                                        TEntityClass others,
                                        string prefix)
        {
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            this.dataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
            this.tagMetaData = tagMetaData ?? throw new ArgumentNullException(nameof(tagMetaData));
            this.self = self;
            this.others = others;
            this.prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            this.queryHelper = new SpriteMatcherQueryHelper<TEntityClass>(tagMetaData, dataSet);
        }

        public bool Match(in SpriteMatcherInput<GraphicTag> q, int z, List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            // try to match the river first ..
            var selfClasses = tagMetaData.QueryClasses(q.TagData);
            if (!self.MatchesAny(selfClasses))
            {
                return false;
            }
            
            var navigationBuffer = ArrayPool<MapCoordinate>.Shared.Rent(4);
            try
            {
                // now check if there is an ocean tile in any of the cardinal directions.
                // if so, we'll draw an river outlet in *inverse* direction. So if there
                // is a ocean at the south, there will be a river outlet connecting the
                // ocean tile with the northern river.
                var matchedTag = q.TagData;
                navigator.NavigateCardinalNeighbours(q.Position.Normalize(), navigationBuffer);
                if (queryHelper.Match(navigationBuffer[CardinalIndex.North.AsInt()], z, others))
                {
                    resultCollector.Add((matchedTag.AsSpriteTag().WithPrefix(prefix).WithQualifier(".s"), SpritePosition.Whole, navigationBuffer[CardinalIndex.North.AsInt()]));
                }
                
                if (queryHelper.Match(navigationBuffer[CardinalIndex.East.AsInt()], z, others))
                {
                    resultCollector.Add((matchedTag.AsSpriteTag().WithPrefix(prefix).WithQualifier(".w"), SpritePosition.Whole, navigationBuffer[CardinalIndex.East.AsInt()]));
                }
                
                if (queryHelper.Match(navigationBuffer[CardinalIndex.South.AsInt()], z, others))
                {
                    resultCollector.Add((matchedTag.AsSpriteTag().WithPrefix(prefix).WithQualifier(".n"), SpritePosition.Whole, navigationBuffer[CardinalIndex.South.AsInt()]));
                }
                
                if (queryHelper.Match(navigationBuffer[CardinalIndex.West.AsInt()], z, others))
                {
                    resultCollector.Add((matchedTag.AsSpriteTag().WithPrefix(prefix).WithQualifier(".e"), SpritePosition.Whole, navigationBuffer[CardinalIndex.West.AsInt()]));
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
            if (model is not RiverOutletSelectorModel m) throw new ArgumentException();

            var dataSet = context.ContextDataSetProducer.CreateGraphicDataSet(m.ContextDataSet ?? throw new ArgumentException());
            var matchSelf = context.ClassRegistry.FromClassNames(m.MatchWith);
            var matchWith = m.MatchWith.Count == 0 ? matchSelf : context.ClassRegistry.FromClassNames(m.MatchWith);
            var prefix = m.Prefix ?? throw new ArgumentException();
            return new RiverOutletSpriteMatcher<TEntityClass>(context.GridNavigator, dataSet, context.TagMetaData, matchSelf, matchWith, prefix);
        }
    }
}