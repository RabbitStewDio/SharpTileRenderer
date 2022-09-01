using Microsoft.Extensions.ObjectPool;
using Serilog;
using Serilog.Events;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Selectors.TileTags;
using SharpTileRenderer.Util;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public readonly struct MatchStrategy
    {
        static readonly ILogger logger = SLog.ForContext<MatchStrategy>();
        
        readonly ObjectPool<List<SparseTagQueryResult<GraphicTag, Unit>>> queryBufferPool;
        readonly ITileDataSet<GraphicTag, Unit> dataSet;
        readonly Dictionary<GraphicTag, ITileTagEntrySelection> graphicTagToClassMapping;
        readonly Optional<ITileTagEntrySelection> defaultValue;

        public MatchStrategy(ITileDataSet<GraphicTag, Unit> dataSet, 
                             Dictionary<GraphicTag, ITileTagEntrySelection> graphicTagToClassMapping,
                             Optional<ITileTagEntrySelection> defaultValue = default)
        {
            this.queryBufferPool = new DefaultObjectPool<List<SparseTagQueryResult<GraphicTag, Unit>>>(new ListObjectPolicy<SparseTagQueryResult<GraphicTag, Unit>>());
            this.dataSet = dataSet;
            this.graphicTagToClassMapping = graphicTagToClassMapping;
            this.defaultValue = defaultValue;
        }

        [Pure]
        public bool TryMatch(in MapCoordinate c, int z, [MaybeNullWhen(false)] out ITileTagEntrySelection match)
        {
            var queryBuffer = queryBufferPool.Get();
            try
            {
                dataSet.QueryPoint(c, z, queryBuffer);
                for (var i = 0; i < queryBuffer.Count; i++)
                {
                    var tag = queryBuffer[i].TagData;
                    if (graphicTagToClassMapping.TryGetValue(tag, out match))
                    {
                        return true;
                    }
                }

                if (logger.IsEnabled(LogEventLevel.Debug))
                {
                    logger.Debug("Failed match at {Coordinate} for {QueryResult}", c, queryBuffer);
                }
                return defaultValue.TryGetValue(out match);
            }
            finally
            {
                queryBuffer.Clear();
                queryBufferPool.Return(queryBuffer);
            }
        }
    }
}