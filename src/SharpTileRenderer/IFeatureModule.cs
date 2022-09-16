using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Selectors;

namespace SharpTileRenderer
{
    public interface IFeatureInitializer<TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        public EntityClassificationRegistry<TClassification> Registry { get; }

        public GraphicTagMetaDataRegistry<TClassification> TagMetaData { get; }

        public MatcherFactory<TClassification> MatcherFactory { get; }
    }

    public interface IFeatureModule
    {
        /// <summary>
        ///   Modules with higher values are tried first.
        /// </summary>
        int PreferenceWeight { get; }
        
        void Initialize<TClassification>(IFeatureInitializer<TClassification> initializer) 
            where TClassification : struct, IEntityClassification<TClassification>;
    }
}