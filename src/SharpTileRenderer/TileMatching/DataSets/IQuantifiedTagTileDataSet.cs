using System;

namespace SharpTileRenderer.TileMatching.DataSets
{
    /// <summary>
    ///    A sparse data set that allows to query quantified (countable) data. Useful to
    ///    express unit counts or to alter graphic representation based on a sliding scale.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TQuantity"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface IQuantifiedTagTileDataSet<TData, TEntity, TQuantity> : ITileDataSet<TData, (TEntity, TQuantity)>
        where TQuantity : IComparable<TQuantity>
    {
    }
}