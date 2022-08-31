using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace SharpTileRenderer.Util
{
    public sealed class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
                                                             IDictionary,
                                                             IReadOnlyDictionary<TKey, TValue>,
                                                             INotifyCollectionChanged,
                                                             INotifyPropertyChanged
    {
        const string CountString = "Count";
        const string IndexerName = "Item[]";
        const string KeysName = "Keys";
        const string ValuesName = "Values";

        readonly IEqualityComparer<TValue> valueComparer;
        readonly Dictionary<TKey, TValue> dictionary;

        public ObservableDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
            valueComparer = EqualityComparer<TValue>.Default;
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);
            valueComparer = EqualityComparer<TValue>.Default;
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(comparer);
            valueComparer = EqualityComparer<TValue>.Default;
        }

        public ObservableDictionary(int capacity)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity);
            valueComparer = EqualityComparer<TValue>.Default;
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> keyComparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary, keyComparer);
            valueComparer = EqualityComparer<TValue>.Default;
        }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> keyComparer)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity, keyComparer);
            valueComparer = EqualityComparer<TValue>.Default;
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        ICollection IDictionary.Keys => Keys;

        public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => dictionary.Keys;

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var removed = dictionary.Remove(key);
            if (removed)
            {
                OnCollectionChanged();
            }

            return removed;
        }


        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        ICollection IDictionary.Values => Values;

        public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => dictionary.Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return Keys; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return Values; }
        }

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => Insert(key, value, false);
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Insert(item.Key, item.Value, true);

        public void Clear()
        {
            if (dictionary.Count > 0)
            {
                dictionary.Clear();
                OnCollectionChanged();
            }
        }


        public bool Contains(KeyValuePair<TKey, TValue> item) => dictionary.Contains(item);


        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)dictionary).CopyTo(array, arrayIndex);

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        IDictionaryEnumerator IDictionary.GetEnumerator() => GetEnumerator();

        Dictionary<TKey, TValue>.Enumerator GetEnumerator() => dictionary.GetEnumerator();

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsSynchronized => ((IDictionary)dictionary).IsSynchronized;
        public object SyncRoot => ((IDictionary)dictionary).SyncRoot;
        public bool IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

        object IDictionary.this[object key]
        {
            get
            {
                return ((IDictionary)dictionary)[key];
            }
            set
            {
                ((IDictionary)dictionary)[key] = value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IDictionary)dictionary).CopyTo(array, index);
        }

        bool IDictionary.Contains(object key) => ((IDictionary)dictionary).Contains(key);

        void IDictionary.Remove(object key)
        {
            ((IDictionary)dictionary).Remove(key);
        }

        void IDictionary.Add(object key, object value)
        {
            ((IDictionary)dictionary).Add(key, value);
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (items.Count > 0)
            {
                if (items.Keys.Any((k) => dictionary.ContainsKey(k)))
                {
                    throw new ArgumentException("An item with the same key has already been added.");
                }

                foreach (var item in items)
                {
                    dictionary.Add(item.Key, item.Value);
                }

                OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
            }
        }


        void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (dictionary.TryGetValue(key, out var item))
            {
                if (add) throw new ArgumentException("An item with the same key has already been added.");
                if (valueComparer.Equals(item, value)) return;
                dictionary[key] = value;

                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        void OnPropertyChanged()
        {
            NotifyPropertyChanged(CountString);
            NotifyPropertyChanged(IndexerName);
            NotifyPropertyChanged(KeysName);
            NotifyPropertyChanged(ValuesName);
        }

        void NotifyPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void OnCollectionChanged()
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem));
        }

        void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
        }

        void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItems));
        }
    }
}