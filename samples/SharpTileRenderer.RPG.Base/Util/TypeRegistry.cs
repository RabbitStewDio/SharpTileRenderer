using SharpTileRenderer.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace SharpTileRenderer.RPG.Base.Util
{
    public class TypeRegistry<TId, TData> : ITypeRegistry<TId, TData>
        where TId : notnull
        where TData : notnull
    {
        readonly Dictionary<TId, TData> types;
        readonly List<TId> keysByInsertOrder;

        public TypeRegistry(TId defaultId, TData defaultValue)
        {
            if (defaultValue == null)
            {
                throw new ArgumentNullException(nameof(defaultValue));
            }

            if (defaultId == null)
            {
                throw new ArgumentNullException(nameof(defaultId));
            }

            this.DefaultValue = (defaultId, defaultValue);

            keysByInsertOrder = new List<TId>();
            keysByInsertOrder.Add(defaultId);
            
            types = new Dictionary<TId, TData>();
            types.Add(defaultId, defaultValue);
        }

        public void Add(TId id, TData val)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }

            if (types.ContainsKey(id))
            {
                throw new ArgumentException("Duplicate value");
            }

            types.Add(id, val);
            keysByInsertOrder.Add(id);
        }

        public (TId key, TData value) DefaultValue { get; }

        public bool TryGetValue(in TId id, [MaybeNullWhen(false)] out TData data) => types.TryGetValue(id, out data);

        public Optional<TData> this[TId idx]
        {
            get
            {
                if (types.TryGetValue(idx, out var value))
                {
                    return value;
                }

                return default;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TId> GetEnumerator()
        {
            return keysByInsertOrder.GetEnumerator();
        }

        public IEnumerable<(TId key, TData value)> Contents => types.Select(kv => (kv.Key, kv.Value));
    }

    public static class TypeRegistry
    {
        public static TypeRegistry<TKey, TData> AppendFrom<TKey, TData, TSource>(this TypeRegistry<TKey, TData> reg, TSource o, Func<TData, TKey> keyFn)
            where TKey : notnull
            where TData : notnull
        {
            if (o == null) throw new ArgumentNullException();
            var defaultValue = reg.DefaultValue.value;
            var equalityComp = EqualityComparer<TData>.Default;
            foreach (var pi in o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!pi.CanRead)
                {
                    continue;
                }

                if (!typeof(TData).IsAssignableFrom(pi.PropertyType))
                {
                    continue;
                }

                var mth = pi.GetMethod;
                if (mth != null && mth.IsPublic && !mth.IsAbstract && mth.GetParameters().Length == 0)
                {
                    var raw = pi.GetValue(o);
                    if (raw is not TData data || equalityComp.Equals(data, defaultValue))
                    {
                        continue;
                    }

                    var key = keyFn(data);
                    reg.Add(key, data);
                }
            }

            return reg;
        }

        public static TypeRegistry<TKey, TData> CreateFrom<TKey, TData, TSource>(TSource o, 
                                                                                 TData defaultValue,
                                                                                 Func<TData, TKey> keyExtractor)
            where TKey : notnull
            where TData : notnull
        {
            var reg = new TypeRegistry<TKey, TData>(keyExtractor(defaultValue), defaultValue);
            return AppendFrom(reg, o, keyExtractor);
        }

        public static TypeRegistry<TKey, TData> CreateFromInstances<TKey, TData>(Func<TData, TKey> keyFn, 
                                                                                 TData value, params TData[] data)
            where TKey : notnull
            where TData : notnull
        {
            var reg = new TypeRegistry<TKey, TData>(keyFn(value), value);
            foreach (var d in data)
            {
                reg.Add(keyFn(d), d);
            }

            return reg;
        }
    }
}