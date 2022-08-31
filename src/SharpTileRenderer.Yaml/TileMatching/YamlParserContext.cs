using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpYaml.Model;
using SharpYaml.Serialization;
using System;

namespace SharpTileRenderer.Yaml.TileMatching
{
    public class YamlParserContext: IYamlTileMatcherModelParserContext
    {
        readonly IYamlTileMatcherModelParserContext parser;
        public SerializerSettings SerializerSettings { get; }

        public YamlParserContext(IYamlTileMatcherModelParserContext parser, SerializerSettings serializerSettings)
        {
            this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
            SerializerSettings = serializerSettings ?? throw new ArgumentNullException(nameof(serializerSettings));
        }

        public ISelectorModel ParseSelector(YamlMapping matchNodeYamlMapping)
        {
            return parser.ParseSelector(matchNodeYamlMapping);
        }

        public IDataSetModel ParseDataSet(YamlMapping element)
        {
            return parser.ParseDataSet(element);
        }
    }
}