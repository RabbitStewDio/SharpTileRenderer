using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing
{
    class DataContextHolder<TEntity> : IDataContextHolder, ITileDataSetProducer<TEntity>
    {
        readonly List<ITileDataSetProducer<TEntity>> dataSets;

        public DataContextHolder()
        {
            this.dataSets = new List<ITileDataSetProducer<TEntity>>();
        }

        public void Add(ITileDataSetProducer<TEntity> dataSet)
        {
            this.dataSets.Add(dataSet);
        }

        public bool CanHandleFully(RenderLayerModel model)
        {
            if (model.EntitySource != null && model.EntitySource.EntityQueryId != null)
            {
                if (!ContainsDataSet(model.EntitySource.EntityQueryId))
                {
                    return false;
                }
            }

            foreach (var subLayer in model.SubLayers)
            {
                if (!CanHandleFully(subLayer))
                {
                    return false;
                }
            }

            return true;
        }
        
        public bool ContainsDataSet(string id)
        {
            foreach (var ds in dataSets)
            {
                if (ds.ContainsDataSet(id))
                {
                    return true;
                }
            }

            return false;
        }

        public Optional<ILayer> Apply(TileMatcherModel model, RenderLayerModel layer, IRenderLayerTypeLift l) => l.Apply<TEntity>(model, layer, this);

        public ITileDataSet<GraphicTag, TEntity> CreateGraphicDataSet(string id)
        {
            foreach (var ds in dataSets)
            {
                if (ds.ContainsDataSet(id))
                {
                    return ds.CreateGraphicDataSet(id);
                }
            }

            throw new ArgumentException($"Unable to create data set '{id}' for entity type {typeof(TEntity)}");
        }

        public IQuantifiedTagTileDataSet<GraphicTag, TEntity, int> CreateCountedGraphicDataSet(string id)
        {
            foreach (var ds in dataSets)
            {
                if (ds.ContainsDataSet(id))
                {
                    return ds.CreateCountedGraphicDataSet(id);
                }
            }

            throw new ArgumentException($"Unable to create data set '{id}' for entity type {typeof(TEntity)}");
        }
    }
}