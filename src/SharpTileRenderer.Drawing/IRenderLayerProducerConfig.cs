using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Drawing
{
    /// <summary>
    ///    A parameter object used when modules are building render layers.
    /// </summary>
    /// <typeparam name="TClassification"></typeparam>
    public interface IRenderLayerProducerConfig<TClassification> : IFeatureInitializer<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        public NavigatorMetaData MapNavigator { get; }
        bool TryGetFeature<TFeature>([MaybeNullWhen(false)] out TFeature f);
        bool TryGetFeature<TFeature>([MaybeNullWhen(false)] out TFeature f, Func<TFeature, bool> featureFilter);
    }
}