using SharpTileRenderer.TexturePack.Yaml.Model;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SharpTileRenderer.TexturePack.Yaml
{
    public class TexturePackYamlWriter
    {
        void Serialize(TextWriter w, TexturePackModel model)
        {
            var serializer = new SerializerBuilder()
                             .WithNamingConvention(LowerCaseNamingConvention.Instance)
                             .Build();
            serializer.Serialize(w, model);
        }

        
    }
}