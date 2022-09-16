using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;

namespace SharpTileRenderer.Drawing
{
    /// <summary>
    ///   Step zero of producing render layers.
    /// </summary>
    public readonly struct RenderLayerFactoryWithNavigator
    {
        readonly NavigatorMetaData md;

        public RenderLayerFactoryWithNavigator(NavigatorMetaData md)
        {
            this.md = md;
        }

        public RenderLayerFactoryWithClassification<TClassification> WithClassification<TClassification>()
            where TClassification : struct, IEntityClassification<TClassification>
        {
            return new RenderLayerFactoryWithClassification<TClassification>(md, new EntityClassificationRegistry<TClassification>());
        }
    }
}