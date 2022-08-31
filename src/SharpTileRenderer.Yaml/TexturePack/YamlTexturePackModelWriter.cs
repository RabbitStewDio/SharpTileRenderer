using SharpTileRenderer.Yaml.TexturePack.Model;
using SharpYaml.Serialization;

namespace SharpTileRenderer.Yaml.TexturePack
{
    public class YamlTexturePackModelWriter
    {
        public string Write(TexturePackModel model)
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