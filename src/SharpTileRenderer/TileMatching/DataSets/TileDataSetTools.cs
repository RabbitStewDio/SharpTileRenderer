using SharpTileRenderer.Navigation;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.DataSets
{
    public static class TileDataSetTools
    {
        /// <summary>
        ///   A helper method for any engine that does not have a spatial data-structure for fast queries.
        ///   This maps area queries into point queries. Slightly less efficient than being able to make
        ///   a box-query over a properly set up spatial index though.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="area"></param>
        /// <param name="z"></param>
        /// <param name="result"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static List<SparseTagQueryResult<TData, TEntity>> ScanQueryArea<TData, TEntity>(ITileDataSet<TData, TEntity> ds,
                                                                                               in ContinuousMapArea area,
                                                                                               int z,
                                                                                               List<SparseTagQueryResult<TData, TEntity>>? result = null)
        {
            result ??= new List<SparseTagQueryResult<TData, TEntity>>();

            var areaX1 = (int)Math.Floor(area.X);
            var areaY1 = (int)Math.Floor(area.Y);

            var areaX2 = (int)Math.Ceiling(area.X + area.Width);
            var areaY2 = (int)Math.Ceiling(area.Y + area.Height);

            for (int y = areaY1; y <= areaY2; y += 1)
            {
                for (int x = areaX1; x <= areaX2; x += 1)
                {
                    ds.QueryPoint(new MapCoordinate(x, y), z, result);
                }
            }

            return result;
        }
    }
}