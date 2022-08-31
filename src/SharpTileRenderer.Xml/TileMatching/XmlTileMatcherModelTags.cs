using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public static class XmlTileSelectorModelTags
    {
        public static readonly XNamespace NS = "https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/";
        public static readonly XName prefixTag = XmlTileSelectorModelTags.NS + "prefix";
        public static readonly XName contextDataSetTag = XmlTileSelectorModelTags.NS + "context-data-set";
        public static readonly XName matchClassTag = XmlTileSelectorModelTags.NS + "class";
        public static readonly XName defaultClassTag = XmlTileSelectorModelTags.NS + "default-class";
    }
    
    public static class XmlTileMatcherModelTags
    {
        public static readonly XNamespace NS = "https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/";
        public static readonly XName TileSpecTag = NS + "tile-set";
        public static readonly XName AuthorTag = NS + "author";
        public static readonly XName DocumentationTag = NS + "documentation";
        public static readonly XName VersionTag = NS + "version";
        public static readonly XName DataSetsTag = NS + "data-sets";
        public static readonly XName LayersTag = NS + "layers";
        public static readonly XName TagsTag = NS + "tags";
        public static readonly XName FeatureFlagsTag = NS + "feature-flags";
        public static readonly XName FeatureFlagTag = NS + "feature-flag";
        public static readonly XName FlagsTag = NS + "flags";
        public static readonly XName FlagTag = NS + "flag";
        public static readonly XName GraphicTagTag = NS + "graphic-tag";
        public static readonly XName LayerTag = NS + "layer";
        public static readonly XName IdTag = NS + "id";
        public static readonly XName LayerIdTag = NS + "layer-id";
        public static readonly XName EnabledTag = NS + "enabled";
        public static readonly XName RenderOrder = NS + "render-order";
        public static readonly XName EntitySourceTag = NS + "entity-source";
        public static readonly XName EntityQueryIdTag = NS + "entity-query-id";
        public static readonly XName EntitySortOrderTag = NS + "sort-order";
        public static readonly XName EntityQueryTypeTag = NS + "query-type";
        public static readonly XName ClassesTag = NS + "classes";
        public static readonly XName ClassTag = NS + "class";
        public static readonly XName PropertiesTag = NS + "properties";
        public static readonly XName PropertyTag = NS + "property";
        public static readonly XName NameTag = NS + "name";
        public static readonly XName ValueTag = NS + "value";
    }
}