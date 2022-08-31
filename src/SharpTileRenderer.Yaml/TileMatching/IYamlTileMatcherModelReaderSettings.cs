using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpYaml.Model;
using System;

namespace SharpTileRenderer.Yaml.TileMatching
{
    public interface IYamlTileMatcherModelReaderSettings
    {
        void RegisterDataSetHandler(string id, Func<YamlMapping, YamlParserContext, IDataSetModel> fn);

        void RegisterSelectorHandler(string id, Func<YamlMapping, YamlParserContext, ISelectorModel> fn);
    }
}