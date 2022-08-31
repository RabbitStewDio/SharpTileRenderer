using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Strategy.Base.Util
{
    public interface ITypeRegistry<TKey, TData> : IEnumerable<TKey>
    {
        (TKey key, TData value) DefaultValue { get; }
        Optional<TData> this[TKey idx] { get; }
        bool TryGetValue(in TKey key, [MaybeNullWhen(false)] out TData data);
        IEnumerable<(TKey key, TData value)> Contents { get; }
    }

    public static class TypeRegistryExtensions
    {/*
        public static int IndexOf<T>(this ITypeRegistry<T> reg, T data)
        {
            int idx = 0;
            foreach (var v in reg)
            {
                if (Equals(v, data))
                {
                    return idx;
                }

                idx += 1;
            }

            return -1;
        }
*/
        public static Dictionary<char, byte> ToIndexDict<TKey, TValue>(this ITypeRegistry<TKey, TValue> reg, Func<TValue, char> keyFn)
        {
            var byCharId = new Dictionary<char, byte>();
            var idx = 0;
            foreach (var key in reg)
            {
                if (!reg.TryGetValue(key, out var value))
                {
                    continue;
                }
                byCharId.Add(keyFn(value), (byte)idx);
                idx += 1;
                if (idx > 255)
                {
                    throw new ArgumentException("Too many terrain types in this registry.");
                }
            }

            return byCharId;
        }

        public static Dictionary<char, T> ToDict<T>(this IEnumerable<T> reg, Func<T, char> keyFn)
        {
            return ToDict(reg, keyFn, t => t);
        }

        public static Dictionary<char, TResult> ToDict<T, TResult>(this IEnumerable<T> reg, Func<T, char> keyFn, Func<T, TResult> r)
        {
            var byCharId = new Dictionary<char, TResult>();
            var idx = 0;
            foreach (var roadType in reg)
            {
                byCharId.Add(keyFn(roadType), r(roadType));
                idx += 1;
                if (idx > 255)
                {
                    throw new ArgumentException("Too many terrain types in this registry.");
                }
            }

            return byCharId;
        }
    }
}
