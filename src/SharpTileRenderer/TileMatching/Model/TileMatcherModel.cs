using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.Meta;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace SharpTileRenderer.TileMatching.Model
{
    [DataContract]
    public class TileMatcherModel : ModelBase, IEquatable<TileMatcherModel>
    {
        [DataMember(Order = 2)]
        string? author;

        [DataMember(Order = 3)]
        string? documentation;

        [DataMember(Order = 4)]
        string? version;

        public TileMatcherModel()
        {
            DataSets = new ObservableCollection<IDataSetModel>();
            RenderLayers = new ObservableCollection<RenderLayerModel>();
            Tags = new ObservableCollection<GraphicTagDefinitionModel>();

            RegisterObservableList(nameof(DataSets), DataSets);
            RegisterObservableList(nameof(Tags), Tags);
            RegisterObservableList(nameof(RenderLayers), RenderLayers);
        }

        [IgnoreDataMember]
        public string? Documentation
        {
            get
            {
                return documentation;
            }
            set
            {
                if (value == documentation) return;
                documentation = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        public string? Author
        {
            get
            {
                return author;
            }
            set
            {
                if (value == author) return;
                author = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        public string? Version
        {
            get
            {
                return version;
            }
            set
            {
                if (value == version) return;
                version = value;
                OnPropertyChanged();
            }
        }

        [DataMember(Order = 1)]
        readonly string kind = "Selector-Specification";

        public string Kind => kind;

        [DataMember(Order = 5)]
        public ObservableCollection<IDataSetModel> DataSets { get; }

        [DataMember(Order = 6)]
        public ObservableCollection<RenderLayerModel> RenderLayers { get; }

        [DataMember(Order = 7)]
        public ObservableCollection<GraphicTagDefinitionModel> Tags { get; }

        public bool Equals(TileMatcherModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return author == other.author &&
                   version == other.version &&
                   documentation == other.documentation &&
                   DataSets.SequenceEqual(other.DataSets) &&
                   Tags.SequenceEqual(other.Tags) &&
                   RenderLayers.SequenceEqual(other.RenderLayers);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((TileMatcherModel)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (author != null ? author.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (version != null ? version.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (documentation != null ? documentation.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ DataSets.GetHashCode();
                hashCode = (hashCode * 397) ^ Tags.GetHashCode();
                hashCode = (hashCode * 397) ^ RenderLayers.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(TileMatcherModel? left, TileMatcherModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TileMatcherModel? left, TileMatcherModel? right)
        {
            return !Equals(left, right);
        }

        public bool HasFeature(string feature)
        {
            foreach (var l in RenderLayers)
            {
                if (l.FeatureFlags.Contains(feature))
                {
                    return true;
                }
            }

            return false;
        }

        public TileMatcherModel Merge(TileMatcherModel parent, TileMatcherModel child)
        {
            var tm = new TileMatcherModel();
            tm.Author = MergeMetaData(parent.Author, child.Author);
            tm.Documentation = MergeMetaData(parent.Documentation, child.Documentation);
            tm.Version = MergeMetaData(parent.Version, child.Version);

            MergeInto(tm.Tags, parent.Tags, child.Tags, k => k.Id, MergeTag);
            MergeInto(tm.DataSets, parent.DataSets, child.DataSets, k => k.Id, ReplaceWithNewer);
            MergeInto(tm.RenderLayers, parent.RenderLayers, child.RenderLayers, k => k.Id, ReplaceWithNewer);
            return tm;
        }

        T ReplaceWithNewer<T>(T a, T b) => b;

        GraphicTagDefinitionModel MergeTag(GraphicTagDefinitionModel arg1, GraphicTagDefinitionModel arg2)
        {
            var gt = new GraphicTagDefinitionModel();
            gt.Id = arg1.Id;
            MergeInto(gt.Classes, arg1.Classes, arg2.Classes, k => k, ReplaceWithNewer);
            MergeInto(gt.Flags, arg1.Flags, arg2.Flags, k => k, ReplaceWithNewer);

            foreach (var p in arg1.Properties)
            {
                gt.Properties[p.Key] = p.Value;
            }

            foreach (var p in arg2.Properties)
            {
                gt.Properties[p.Key] = p.Value;
            }

            return gt;
        }

        /// <summary>
        ///   Merges the contents of the two source collections into a new target collection.
        ///   This operation will collapse any duplicate entries on either source collection.
        ///   This operation will preserve the order of entries of source collection a over
        ///   source collection b.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sourceA"></param>
        /// <param name="sourceB"></param>
        /// <param name="keyExtractor"></param>
        /// <param name="mergeFunction"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        void MergeInto<T, TKey>(ObservableCollection<T> target,
                                ObservableCollection<T> sourceA,
                                ObservableCollection<T> sourceB,
                                Func<T, TKey> keyExtractor,
                                Func<T, T, T> mergeFunction)
        {
            var aKeys = ExtractOrderedIndex(sourceA, keyExtractor, mergeFunction);
            var bKeys = ExtractOrderedIndex(sourceB, keyExtractor, mergeFunction);

            foreach (var a in aKeys.OrderBy(e => e.Value.Item1))
            {
                var key = a.Key;
                if (bKeys.TryGetValue(key, out var replacementValue))
                {
                    target.Add(mergeFunction(a.Value.Item2, replacementValue.Item2));
                    bKeys.Remove(key);
                }
                else
                {
                    target.Add(a.Value.Item2);
                }
            }

            foreach (var a in bKeys.OrderBy(e => e.Value.Item1))
            {
                target.Add(a.Value.Item2);
            }
        }

        static Dictionary<KeyNullProtection<TKey>, (int, T)> ExtractOrderedIndex<T, TKey>(ObservableCollection<T> sourceB,
                                                                                          Func<T, TKey> keyExtractor,
                                                                                          Func<T, T, T> mergeFunction)
        {
            var bKeys = new Dictionary<KeyNullProtection<TKey>, (int, T)>();
            for (var index = 0; index < sourceB.Count; index++)
            {
                var b = sourceB[index];
                var key = keyExtractor(b);
                if (bKeys.TryGetValue(new KeyNullProtection<TKey>(key), out var existing))
                {
                    var merged = mergeFunction(existing.Item2, b);
                    bKeys[new KeyNullProtection<TKey>(key)] = (existing.Item1, merged);
                }
                else
                {
                    bKeys[new KeyNullProtection<TKey>(key)] = (index, b);
                }
            }

            return bKeys;
        }

        readonly struct KeyNullProtection<T> : IEquatable<KeyNullProtection<T>>
        {
            readonly T data;

            public KeyNullProtection(T data)
            {
                this.data = data;
            }

            public bool Equals(KeyNullProtection<T> other)
            {
                return EqualityComparer<T>.Default.Equals(data, other.data);
            }

            public override bool Equals(object? obj)
            {
                return obj is KeyNullProtection<T> other && Equals(other);
            }

            public override int GetHashCode()
            {
                return EqualityComparer<T>.Default.GetHashCode(data);
            }

            public static bool operator ==(KeyNullProtection<T> left, KeyNullProtection<T> right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(KeyNullProtection<T> left, KeyNullProtection<T> right)
            {
                return !left.Equals(right);
            }
        }

        string? MergeMetaData(string? a, string? b)
        {
            if (string.IsNullOrWhiteSpace(a)) return b;
            if (string.IsNullOrWhiteSpace(b)) return a;
            return $"{a}; {b}";
        }
    }
}