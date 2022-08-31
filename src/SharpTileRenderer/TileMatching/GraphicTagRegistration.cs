using System;

namespace SharpTileRenderer.TileMatching
{
    public readonly struct GraphicTagRegistration<TEntityClassification>
        where TEntityClassification : struct, IEntityClassification<TEntityClassification>
    {
        readonly GraphicTagMetaDataRegistry<TEntityClassification>? registry;
        readonly GraphicTag tag;
        
        public GraphicTagRegistration(GraphicTagMetaDataRegistry<TEntityClassification> registry, GraphicTag tag)
        {
            this.registry = registry;
            this.tag = tag;
        }

        public GraphicTagRegistration<TEntityClassification> WithClassification(string className)
        {
            if (string.IsNullOrEmpty(className)) throw new ArgumentException();
            if (registry == null) throw new InvalidOperationException();
            registry.AddClass(tag, className);
            return this;
        }

        public GraphicTagRegistration<TEntityClassification> WithFlag(string flag)
        {
            if (string.IsNullOrEmpty(flag)) throw new ArgumentException();
            if (registry == null) throw new InvalidOperationException();
            registry.AddFlag(tag, flag);
            return this;
        }

        public GraphicTagRegistration<TEntityClassification> WithProperty(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException();
            if (string.IsNullOrEmpty(value)) throw new ArgumentException();
            if (registry == null) throw new InvalidOperationException();
            registry.AddProperty(tag, name, value);
            return this;
        }
    }
}