using System.Collections.Generic;

namespace SharpTileRenderer.TileMatching
{
    public static class EntityClassificationRegistryExtensions
    {
        public static T FromClassNames<T>(this EntityClassificationRegistry<T> reg, IList<string> classNames)
            where T : struct, IEntityClassification<T>
        {
            var matchSelf = default(T);
            for (var index = 0; index < classNames.Count; index++)
            {
                var ms = classNames[index];
                if (reg.TryGetClassification(ms, out var c))
                {
                    matchSelf = matchSelf.Merge(c);
                }
            }

            return matchSelf;
        }

        public static List<(TClass Selector, string TagValue)> CollectTagData<TClass>(this EntityClassificationRegistry<TClass> classRegistry,
                                                                                      TClass matchSet,
                                                                                      List<(TClass Selector, string TagValue)>? retval = null)
            where TClass : struct, IEntityClassification<TClass>
        {
            retval ??= new List<(TClass Selector, string TagValue)>();
            var knownGraphicTags = new HashSet<string>();
            foreach (var v in retval)
            {
                knownGraphicTags.Add(v.TagValue);
            }

            foreach (var c in classRegistry.KnownClasses)
            {
                if (!c.Value.MatchesAny(matchSet) || c.Key.Length == 0)
                {
                    continue;
                }

                var key = $"{c.Key[0]}";
                if (knownGraphicTags.Contains(key))
                {
                    // warn of duplicate keys ..
                    continue;
                }

                retval.Add((c.Value, key));
                knownGraphicTags.Add(key);
            }

            return retval;
        }
    }
}