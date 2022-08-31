using SharpTileRenderer.TileMatching.Model.Meta;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.TileMatching
{
    public class GraphicTagMetaDataRegistry<TEntityClassification> : IGraphicTagMetaDataRegistry<TEntityClassification>
        where TEntityClassification : struct, IEntityClassification<TEntityClassification>
    {
        readonly EntityClassificationRegistry<TEntityClassification> registry;
        readonly Dictionary<GraphicTag, MetaData> metaStore;

        public GraphicTagMetaDataRegistry(EntityClassificationRegistry<TEntityClassification>? registry = null)
        {
            this.registry = registry ?? new EntityClassificationRegistry<TEntityClassification>();
            this.metaStore = new Dictionary<GraphicTag, MetaData>();
        }

        public IEnumerable<GraphicTag> KnownTags => metaStore.Keys;

        public bool HasFlag(GraphicTag t, string flag)
        {
            return metaStore.TryGetValue(t, out var meta) && meta.Flags.Contains(flag);
        }

        public bool TryGetProperty(GraphicTag t, string propertyName, [MaybeNullWhen(false)] out string value)
        {
            value = default;
            return metaStore.TryGetValue(t, out var meta) && meta.Properties.TryGetValue(propertyName, out value);
        }

        public TEntityClassification QueryClasses(GraphicTag t)
        {
            if (metaStore.TryGetValue(t, out var meta))
            {
                return meta.Classes;
            }

            return default;
        }

        public void AddClass(GraphicTag tag, string className)
        {
            if (className == null)
            {
                throw new ArgumentNullException(nameof(className));
            }

            var classValue = registry.Register(className);
            if (metaStore.TryGetValue(tag, out var meta))
            {
                meta = new MetaData(meta.Classes.Merge(classValue), meta.Flags, meta.Properties);
            }
            else
            {
                meta = new MetaData(classValue);
            }

            metaStore[tag] = meta;
        }

        public void AddFlag(GraphicTag tag, string flag)
        {
            if (flag == null)
            {
                throw new ArgumentNullException(nameof(flag));
            }

            if (!metaStore.TryGetValue(tag, out var meta))
            {
                meta = new MetaData(default);
                metaStore[tag] = meta;
            }

            meta.Flags.Add(flag);
        }

        public void AddProperty(GraphicTag tag, string propertyName, string value)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!metaStore.TryGetValue(tag, out var meta))
            {
                meta = new MetaData(default);
                metaStore[tag] = meta;
            }

            meta.Properties[propertyName] = value;
        }

        public GraphicTagRegistration<TEntityClassification> Register(GraphicTag tag)
        {
            if (!metaStore.TryGetValue(tag, out var meta))
            {
                meta = new MetaData(default);
                metaStore[tag] = meta;
            }

            return new GraphicTagRegistration<TEntityClassification>(this, tag);
        }

        public GraphicTagRegistration<TEntityClassification> Register(GraphicTagDefinitionModel tagModel)
        {
            if (string.IsNullOrEmpty(tagModel.Id)) throw new ArgumentException();
            var reg = Register(GraphicTag.From(tagModel.Id));
            foreach (var tagModelClass in tagModel.Classes)
            {
                reg.WithClassification(tagModelClass);
            }

            foreach (var flag in tagModel.Flags)
            {
                reg.WithFlag(flag);
            }

            foreach (var p in tagModel.Properties)
            {
                reg.WithProperty(p.Key, p.Value);
            }

            return reg;
        }

        readonly struct MetaData
        {
            public readonly TEntityClassification Classes;
            public readonly HashSet<string> Flags;
            public readonly Dictionary<string, string> Properties;

            public MetaData(TEntityClassification classes, 
                            HashSet<string>? flags = null, 
                            Dictionary<string, string>? properties = null)
            {
                this.Classes = classes;
                this.Flags = flags ?? new HashSet<string>();
                this.Properties = properties ?? new Dictionary<string, string>();
            }

            public override string ToString()
            {
                return $"{nameof(Classes)}: {Classes}, {nameof(Flags)}: [{string.Join(",", Flags)}], {nameof(Properties)}: [{string.Join(",", Properties)}]";
            }
        }
    }
}