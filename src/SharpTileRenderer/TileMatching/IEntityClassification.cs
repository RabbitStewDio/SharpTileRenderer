using System.Diagnostics.Contracts;

namespace SharpTileRenderer.TileMatching
{
    public interface IEntityClassification<TEntityClassification>
        where TEntityClassification : IEntityClassification<TEntityClassification>
    {
        int Cardinality { get; }
        [Pure]
        TEntityClassification Create(int cardinalPosition);

        [Pure]
        bool MatchesAny(TEntityClassification other);

        [Pure]
        TEntityClassification Matching(TEntityClassification other);
        
        bool IsEmpty { get; }

        [Pure]
        TEntityClassification Merge(TEntityClassification other);

    }
}