using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpYaml.Model;
using SharpYaml.Serialization;

namespace SharpTileRenderer.Yaml.TileMatching
{
    public interface IYamlTileMatcherModelParserContext
    {
        SerializerSettings SerializerSettings { get; }
        ISelectorModel ParseSelector(YamlMapping matchNodeYamlMapping);
        IDataSetModel ParseDataSet(YamlMapping element);
    }
}