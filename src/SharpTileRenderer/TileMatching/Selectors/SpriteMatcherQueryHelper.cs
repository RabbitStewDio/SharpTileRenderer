using Microsoft.Extensions.ObjectPool;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.Util;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors
{
    public class SpriteMatcherQueryHelper<TEntityClass>
        where TEntityClass : struct, IEntityClassification<TEntityClass>
    {
        readonly ObjectPool<List<SparseTagQueryResult<GraphicTag, Unit>>> queryBuffer;
        readonly IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;

        public SpriteMatcherQueryHelper(IGraphicTagMetaDataRegistry<TEntityClass> tagMetaData, ITileDataSet<GraphicTag, Unit> dataSet)
        {
            this.queryBuffer = new DefaultObjectPool<List<SparseTagQueryResult<GraphicTag, Unit>>>(new ListObjectPolicy<SparseTagQueryResult<GraphicTag, Unit>>());
            this.tagMetaData = tagMetaData;
            this.dataSet = dataSet;
        }

        public bool Match(MapCoordinate c,
                          int z,
                          TEntityClass matching)
        {
            var result = false;
            var buffer = queryBuffer.Get();
            try
            {
                dataSet.QueryPoint(c, z, buffer);
                for (var i = 0; i < buffer.Count; i++)
                {
                    var tag = buffer[i].TagData;
                    var tagClass = tagMetaData.QueryClasses(tag);
                    result |= matching.MatchesAny(tagClass);
                }
            }
            finally
            {
                buffer.Clear();
                queryBuffer.Return(buffer);
            }

            return result;
        }        
        
        public bool Match(MapCoordinate c,
                          int z,
                          TEntityClass matching,
                          out GraphicTag matchedTag)
        {
            var result = false;
            matchedTag = default;
            var buffer = queryBuffer.Get();
            try
            {
                dataSet.QueryPoint(c, z, buffer);
                for (var i = 0; i < buffer.Count; i++)
                {
                    var tag = buffer[i].TagData;
                    var tagClass = tagMetaData.QueryClasses(tag);
                    if (matching.MatchesAny(tagClass))
                    {
                        result = true;
                        matchedTag = tag;
                    }
                }
                
                return result;
            }
            finally
            {
                buffer.Clear();
                queryBuffer.Return(buffer);
            }

        }
    }
}