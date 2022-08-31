using Serilog;
using SharpTileRenderer.TileMatching.Selectors.TileTags;
using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching.Selectors.BuiltIn
{
    public static class ClassSelectorBuilder
    {
        static readonly ILogger logger = SLog.ForContext(typeof(ClassSelectorBuilder));
        
        public static Dictionary<GraphicTag, ITileTagEntrySelection> PrepareSelectionTags<TClass>(IGraphicTagMetaDataRegistry<TClass> tagRegistry,
                                                                                                  EntityClassificationRegistry<TClass> classRegistry,
                                                                                                  TClass matchSet,
                                                                                                  out TileTagEntrySelectionFactory<TClass> cardinality) 
            where TClass : struct, IEntityClassification<TClass>
        {
            var classCollector = new List<(TClass Key, string Value)>();
            var usedClasses = classRegistry.CollectTagData(matchSet, classCollector).ToArray();
            var factory = new TileTagEntrySelectionFactory<TClass>(usedClasses);
            cardinality = factory;

            var result = new Dictionary<GraphicTag, ITileTagEntrySelection>();
            foreach (var t in tagRegistry.KnownTags)
            {
                var classes = tagRegistry.QueryClasses(t);
                classCollector.Clear();
                classRegistry.CollectTagData(classes, classCollector);

                foreach (var c in classCollector)
                {
                    if (!factory.TryLookup(c.Key).TryGetValue(out var selector))
                    {
                        continue;
                    }


                    if (result.TryGetValue(t, out var v))
                    {
                        logger.Warning("Duplicate class {Class}, non deterministic matching could happen. Conflicting tag class is {ConflictingClass}", t, v.Tag);
                    }
                    else
                    {
                        result[t] = selector;
                    }
                }
            }

            return result;
        }
    }
}