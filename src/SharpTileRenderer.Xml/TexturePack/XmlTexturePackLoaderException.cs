using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SharpTileRenderer.Xml.TexturePack
{
    public class XmlTexturePackLoaderException : IOException
    {
        public XmlTexturePackLoaderException()
        { }

        public XmlTexturePackLoaderException(string message) : base(message)
        { }

        public XmlTexturePackLoaderException(string message, IXmlLineInfo? lineInfo) : base(AppendLineInfo(message, lineInfo))
        { }

        static string AppendLineInfo(string message, IXmlLineInfo? lineInfo)
        {
            if (lineInfo?.HasLineInfo() == true)
            {
                return $"{message} [{lineInfo.LineNumber}:{lineInfo.LinePosition}]";
            }

            return message;
        }

        public XmlTexturePackLoaderException(string message, int hresult) : base(message, hresult)
        { }

        public XmlTexturePackLoaderException(string message, Exception innerException) : base(message, innerException)
        { }

        protected XmlTexturePackLoaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
