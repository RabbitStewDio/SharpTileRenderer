using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpTileRenderer.TileMatching
{
    public static class LanguageUtils
    {
        public static int GetValueContentsHashCode<T>(this IList<T> list)
            where T : struct
        {
            unchecked
            {
                int hashCode = list.Count;
                for (var i = 0; i < list.Count; i++)
                {
                    var x = list[i];
                    hashCode = hashCode * 397 ^ x.GetHashCode();
                }

                return hashCode;
            }
        }

        public static int GetContentsHashCode<T>(this IList<T> list)
            where T : class
        {
            unchecked
            {
                int hashCode = list.Count;
                for (var i = 0; i < list.Count; i++)
                {
                    var x = list[i];
                    hashCode = hashCode * 397 ^ x?.GetHashCode() ?? 0;
                }

                return hashCode;
            }
        }

        public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> d1, IDictionary<TKey, TValue> d2)
        {
            var valueComp = EqualityComparer<TValue>.Default;
            return d1.Count == d2.Count && d1.All(dict1Content => d2.TryGetValue(dict1Content.Key, out var d2Value) && valueComp.Equals(dict1Content.Value, d2Value));
        }
        
        public static int GetContentsHashCode<TKey, TValue>(this IDictionary<TKey, TValue> list)
        {
            unchecked
            {
                var hashCode = list.Count;
                var keyComp = EqualityComparer<TKey>.Default;
                var valueComp = EqualityComparer<TValue>.Default;
                foreach (var content in list)
                {
                    hashCode = hashCode * 397 ^ keyComp.GetHashCode(content.Key);
                    hashCode = hashCode * 397 ^ valueComp.GetHashCode(content.Value);
                }

                return hashCode;
            }
        }
    }
}