using SharpTileRenderer.TileMatching.Model.DataSets;
using System;

namespace SharpTileRenderer.TileMatching.Model
{
    public static class TileMatcherModelExtensions
    {
        public static TileMatcherModel WithDataSet(this TileMatcherModel m, IDataSetModel data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            m.DataSets.Add(data);
            return m;
        }

        public static TileMatcherModel WithRenderLayer(this TileMatcherModel m, RenderLayerModel layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            m.RenderLayers.Add(layer);
            return m;
        }
    }
}