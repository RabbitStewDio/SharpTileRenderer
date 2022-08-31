using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.DataSets
{
    public class DefaultTileDataSetProducer<TEntity>: ITileDataSetProducer<TEntity>
    {
        readonly Dictionary<string, Lazy<ITileDataSet<GraphicTag, TEntity>>> tagDataSets;
        readonly Dictionary<string, Lazy<IQuantifiedTagTileDataSet<GraphicTag, TEntity, int>>> quantifiedTagDataSets;

        public DefaultTileDataSetProducer()
        {
            tagDataSets = new Dictionary<string, Lazy<ITileDataSet<GraphicTag, TEntity>>>();
            quantifiedTagDataSets = new Dictionary<string, Lazy<IQuantifiedTagTileDataSet<GraphicTag, TEntity, int>>>();
        }

        public DefaultTileDataSetProducer<TEntity> WithDataSet(string id, ITileDataSet<GraphicTag, TEntity> dataSet)
        {
            tagDataSets[id] = new Lazy<ITileDataSet<GraphicTag, TEntity>>(dataSet);
            return this;
        }

        public DefaultTileDataSetProducer<TEntity> WithDataSet(string id, Func<ITileDataSet<GraphicTag, TEntity>> dataSet)
        {
            tagDataSets[id] = new Lazy<ITileDataSet<GraphicTag, TEntity>>(dataSet);
            return this;
        }
        
        public DefaultTileDataSetProducer<TEntity> WithQuantifiedDataSet(string id, IQuantifiedTagTileDataSet<GraphicTag, TEntity, int> dataSet)
        {
            quantifiedTagDataSets[id] = new Lazy<IQuantifiedTagTileDataSet<GraphicTag, TEntity, int>>(dataSet);
            return this;
        }

        public DefaultTileDataSetProducer<TEntity> WithQuantifiedDataSet(string id, Func<IQuantifiedTagTileDataSet<GraphicTag, TEntity, int>> dataSet)
        {
            quantifiedTagDataSets[id] = new Lazy<IQuantifiedTagTileDataSet<GraphicTag, TEntity, int>>(dataSet);
            return this;
        }

        public bool ContainsDataSet(string id)
        {
            return quantifiedTagDataSets.ContainsKey(id) || tagDataSets.ContainsKey(id);
        }

        public ITileDataSet<GraphicTag, TEntity> CreateGraphicDataSet(string id)
        {
            if (tagDataSets.TryGetValue(id, out var result))
            {
                return result.Value;
            }
            throw new ArgumentException($"No tag data set with id '${id}' defined");
        }

        public IQuantifiedTagTileDataSet<GraphicTag, TEntity, int> CreateCountedGraphicDataSet(string id)
        {
            if (quantifiedTagDataSets.TryGetValue(id, out var result))
            {
                return result.Value;
            }
            throw new ArgumentException($"No quantified tag data set with id '${id}' defined");
        }
    }
}