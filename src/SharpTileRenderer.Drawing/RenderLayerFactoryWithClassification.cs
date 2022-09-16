using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing
{
    /// <summary>
    ///    Second stage of layer production. 
    /// </summary>
    /// <typeparam name="TClassification"></typeparam>
    public class RenderLayerFactoryWithClassification<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        public RenderLayerFactoryWithClassification(NavigatorMetaData md, EntityClassificationRegistry<TClassification> registry)
        {
            this.features = new List<IFeatureModule>();
            this.MapNavigator = md;
            this.Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.TagMetaData = new GraphicTagMetaDataRegistry<TClassification>(registry);
            this.MatcherFactory = new MatcherFactory<TClassification>();
        }

        readonly List<IFeatureModule> features;

        public NavigatorMetaData MapNavigator { get; }

        public EntityClassificationRegistry<TClassification> Registry { get; }

        public GraphicTagMetaDataRegistry<TClassification> TagMetaData { get; }

        public MatcherFactory<TClassification> MatcherFactory { get; }

        public RenderLayerFactoryWithClassification<TClassification> WithFeature(IFeatureModule f)
        {
            features.Add(f);
            return this;
        }

        public RenderLayerFactoryWithClassification<TClassification> WithDefaultMatchers()
        {
            this.MatcherFactory.WithDefaultMatchers();
            return this;
        }

        public RenderLayerFactoryWithClassification<TClassification> RegisterTagSelector(string id, MatcherFactory<TClassification>.MatcherFactoryDelegate<GraphicTag> f)
        {
            this.MatcherFactory.RegisterTagSelector(id, f);
            return this;
        }

        public RenderLayerFactoryWithClassification<TClassification> RegisterQuantifiedTagSelector(string id, MatcherFactory<TClassification>.MatcherFactoryDelegate<(GraphicTag, int)> f)
        {
            this.MatcherFactory.RegisterQuantifiedTagSelector(id, f);
            return this;
        }

        public RenderLayerFactoryWithClassification<TClassification> Register(Action<MatcherFactory<TClassification>> action)
        {
            action(MatcherFactory);
            return this;
        }

        public RenderLayerFactoryWithData<TClassification> PrepareForData()
        {
            var featureModules = features.ToArray();
            return new RenderLayerFactoryWithData<TClassification>(MapNavigator, Registry, TagMetaData, MatcherFactory, featureModules);
        }
    }
}