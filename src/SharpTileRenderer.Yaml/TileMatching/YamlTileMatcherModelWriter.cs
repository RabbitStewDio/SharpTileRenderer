using SharpTileRenderer.TileMatching.Model;
using SharpYaml.Serialization;

namespace SharpTileRenderer.Yaml.TileMatching
{
    public class YamlTileMatcherModelWriter
    {
        public string Write(TileMatcherModel model)
        {
            var serializerSettings = CreateSerializerSettings();
            var serializer = new Serializer(serializerSettings);
            return serializer.Serialize(model);
        }

        protected virtual SerializerSettings CreateSerializerSettings()
        {
            return YamlSerializerSettings.CreateDefaultSerializerSettings();
        }
    }
}
