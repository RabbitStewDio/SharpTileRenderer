using System;
using System.Runtime.Serialization;

namespace SharpTileRenderer.Yaml
{
    public class YamlParsingException: Exception
    {
        public YamlParsingException()
        {
        }

        protected YamlParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public YamlParsingException(string message) : base(message)
        {
        }

        public YamlParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}