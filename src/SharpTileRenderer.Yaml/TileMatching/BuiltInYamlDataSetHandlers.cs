using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpYaml.Serialization;

namespace SharpTileRenderer.Yaml.TileMatching
{
    public static class BuiltInYamlDataSetHandlers
    {
        public static T ConfigureBuiltInDataSetReadHandlers<T>(this T c)
            where T: IYamlTileMatcherModelReaderSettings
        {
            var namingRule = new CamelCaseNamingConvention();
            c.RegisterDataSetHandler<ClassSetDataSetModel>(namingRule.Convert(DataSetType.ClassSet.ToString()));
            c.RegisterDataSetHandler<TagDataSetModel>(namingRule.Convert(DataSetType.TagMap.ToString()));
            c.RegisterDataSetHandler<QuantifiedClassSetDataSetModel>(namingRule.Convert(DataSetType.QuantifiedClassSet.ToString()));
            c.RegisterDataSetHandler<QuantifiedTagDataSetModel>(namingRule.Convert(DataSetType.QuantifiedTagMap.ToString()));
            return c;
        }

    }
}