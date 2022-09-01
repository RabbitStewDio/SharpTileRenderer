using System;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching
{
    public class EntityClassificationRegistry<TEntityClassification>
        where TEntityClassification : struct, IEntityClassification<TEntityClassification>
    {
        readonly int maximumCardinality;
        readonly Dictionary<string, TEntityClassification> knownClasses;

        public EntityClassificationRegistry() : this(MaxCardinalityFromData)
        {
        }

        public TEntityClassification EmptyClassificationSet => new TEntityClassification();

        public EntityClassificationRegistry(int maximumCardinality)
        {
            if (maximumCardinality < 0 || maximumCardinality > MaxCardinalityFromData)
            {
                throw new ArgumentException();
            }

            this.maximumCardinality = maximumCardinality;
            knownClasses = new Dictionary<string, TEntityClassification>();
        }

        static int MaxCardinalityFromData => default(TEntityClassification).Cardinality;

        public IReadOnlyDictionary<string, TEntityClassification> KnownClasses => knownClasses;

        public TEntityClassification Register(string classId)
        {
            if (knownClasses.TryGetValue(classId, out var pos))
            {
                return pos;
            }

            if (knownClasses.Count >= (maximumCardinality))
            {
                throw new ArgumentException($"Failed to register class {classId}; maximum cardinality of {maximumCardinality} reached");
            }

            int cardinalPosition = knownClasses.Count;
            pos = default(TEntityClassification).Create(cardinalPosition);
            knownClasses.Add(classId, pos);
            return pos;
        }

        public bool TryGetClassification(string classId, out TEntityClassification value) => knownClasses.TryGetValue(classId, out value);
    }
}