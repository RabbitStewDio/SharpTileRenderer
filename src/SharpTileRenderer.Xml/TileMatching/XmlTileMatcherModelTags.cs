using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public static class XmlTileSelectorModelTags
    {
        public static readonly XNamespace Ns = "https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/";
        public static readonly XName PrefixTag = Ns + "prefix";
        public static readonly XName SuffixTag = Ns + "suffix";
        public static readonly XName ContextDataSetTag = Ns + "context-data-set";
        public static readonly XName MatchClassTag = Ns + "class";
        public static readonly XName DefaultClassTag = Ns + "default-class";
        public static readonly XName ForceGraphicTag = Ns + "force-graphic";
    }
    
    public static class XmlTileMatcherModelTags
    {
        public static readonly XNamespace Ns = "https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/";
        public static readonly XName TileSpecTag = Ns + "tile-set";
        public static readonly XName AuthorTag = Ns + "author";
        public static readonly XName DocumentationTag = Ns + "documentation";
        public static readonly XName VersionTag = Ns + "version";
        public static readonly XName DataSetsTag = Ns + "data-sets";
        public static readonly XName LayersTag = Ns + "layers";
        public static readonly XName TagsTag = Ns + "tags";
        public static readonly XName FeatureFlagsTag = Ns + "feature-flags";
        public static readonly XName FeatureFlagTag = Ns + "feature-flag";
        public static readonly XName FlagsTag = Ns + "flags";
        public static readonly XName FlagTag = Ns + "flag";
        public static readonly XName GraphicTagTag = Ns + "graphic-tag";
        public static readonly XName LayerTag = Ns + "layer";
        public static readonly XName SubLayerTag = Ns + "sub-layer";
        public static readonly XName IdTag = Ns + "id";
        public static readonly XName LayerIdTag = Ns + "layer-id";
        public static readonly XName EnabledTag = Ns + "enabled";
        public static readonly XName RenderOrder = Ns + "render-order";
        public static readonly XName EntitySourceTag = Ns + "entity-source";
        public static readonly XName EntityQueryIdTag = Ns + "entity-query-id";
        public static readonly XName EntitySortOrderTag = Ns + "sort-order";
        public static readonly XName EntityQueryTypeTag = Ns + "query-type";
        public static readonly XName ClassesTag = Ns + "classes";
        public static readonly XName ClassTag = Ns + "class";
        public static readonly XName PropertiesTag = Ns + "properties";
        public static readonly XName PropertyTag = Ns + "property";
        public static readonly XName NameTag = Ns + "name";
        public static readonly XName ValueTag = Ns + "value";
    }
}