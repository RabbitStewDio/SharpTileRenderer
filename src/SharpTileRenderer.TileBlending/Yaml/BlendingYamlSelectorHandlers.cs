using SharpTileRenderer.TileBlending.Matcher;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.Yaml.TileMatching;
using SharpYaml.Model;

namespace SharpTileRenderer.TileBlending.Yaml
{
    public static class BlendingYamlSelectorHandlers
    {
        public static IYamlTileMatcherModelReaderSettings ConfigureBlendingHandlers(this IYamlTileMatcherModelReaderSettings c)
        {
            c.RegisterSelectorHandler(BlendingSelectorModel.SelectorName, ParseBlendingSelectorModel);
            return c;
        }

        public static ISelectorModel ParseBlendingSelectorModel(YamlMapping node, YamlParserContext context)
        {
            var retval = new BlendingSelectorModel();
            // todo
            return retval;
        }


    }
}