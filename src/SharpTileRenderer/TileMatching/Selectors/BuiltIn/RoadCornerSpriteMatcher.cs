using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    /// <summary>
    ///    A cardinal direction wildcard matcher. This sprite matcher draws connections between
    ///    diagonal connecting tiles. When drawing thick diagonal lines on a tile grid, connections
    ///    between tiles look wrong, as corners seem to be missing between tiles.
    ///
    ///           *** |
    ///            ***|
    ///             **|?
    ///          -----+----- 
    ///              ?|**    
    ///               |***   
    ///               | ***  
    ///                      
    /// 
    ///   There are only two corner cases we need to check:
    ///   Matching is based on a single tile and checks for diagonal tile connections to the north
    ///   east and north west. There is no need to check any connections to the south, as this case
    ///   will be handled when the southern tiles check towards *their* northern neighbours.
    ///
    ///   A connection from the center tile to the north west will draw a sprite with suffix
    ///   "ne" and "sw". The connection towards a north eastern tile will draw the sprites with
    ///   suffix "sw" and "ne" instead.
    /// </summary>
    /// <typeparam name="TEntityClass"></typeparam>
    public class RoadCornerSpriteMatcher<TEntityClass> : ISpriteMatcher<GraphicTag>
        where TEntityClass : struct, IEntityClassification<TEntityClass>
    {
        readonly IMapNavigator<GridDirection> navigator;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;
        readonly IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData;
        readonly TEntityClass self;
        readonly TEntityClass others;
        readonly string? prefix;
        readonly SpriteMatcherQueryHelper<TEntityClass> queryHelper;

        public RoadCornerSpriteMatcher(IMapNavigator<GridDirection> navigator,
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
        }

        public bool IsThreadSafe => dataSet.MetaData.IsThreadSafe;
        public string MatcherType => BuiltInSelectors.RoadCorner;

        public bool Match(in SpriteMatcherInput<GraphicTag> q,
                          int z,
                          List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> resultCollector)
        {
            var selfClasses = tagMetaData.QueryClasses(q.TagData);
            if (!self.MatchesAny(selfClasses))
            {
                // have a road ..
                return false;
            }

            var navigationBuffer = ArrayPool<MapCoordinate>.Shared.Rent(8);
            try
            {
                var gridPos = q.Position.Normalize();
                if (navigator.NavigateTo(GridDirection.NorthEast, gridPos, out var neCoord) && 
                    queryHelper.Match(neCoord, z, others))
                {
                    navigator.NavigateTo(GridDirection.East, gridPos, out var eastCoord);
                    navigator.NavigateTo(GridDirection.North, gridPos, out var northCoord);
                    resultCollector.Add((q.TagData.AsSpriteTag().WithPrefix(prefix).WithQualifier(".se"), SpritePosition.Whole, northCoord));
                    resultCollector.Add((q.TagData.AsSpriteTag().WithPrefix(prefix).WithQualifier(".nw"), SpritePosition.Whole, eastCoord));
                }
                
                if (navigator.NavigateTo(GridDirection.NorthWest, gridPos, out var nwCoord) && 
                    queryHelper.Match(nwCoord, z, others))
                {
                    navigator.NavigateTo(GridDirection.West, gridPos, out var westCoord);
                    navigator.NavigateTo(GridDirection.North, gridPos, out var northCoord);
                    resultCollector.Add((q.TagData.AsSpriteTag().WithPrefix(prefix).WithQualifier(".ne"), SpritePosition.Whole, westCoord));
                    resultCollector.Add((q.TagData.AsSpriteTag().WithPrefix(prefix).WithQualifier(".sw"), SpritePosition.Whole, northCoord));
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
            if (model is not RoadCornerSelectorModel m) throw new ArgumentException();

            var dataSet = context.ContextDataSetProducer.CreateGraphicDataSet(m.ContextDataSet ?? throw new ArgumentException());
            var matchSelf = context.ClassRegistry.FromClassNames(m.MatchSelf);
            var matchWith = m.MatchWith.Count == 0 ? matchSelf : context.ClassRegistry.FromClassNames(m.MatchWith);
            var prefix = m.Prefix ?? throw new ArgumentException();
            
            return new RoadCornerSpriteMatcher<TEntityClass>(context.GridNavigator, dataSet, context.TagMetaData, matchSelf, matchWith, prefix);
        }
    }
}