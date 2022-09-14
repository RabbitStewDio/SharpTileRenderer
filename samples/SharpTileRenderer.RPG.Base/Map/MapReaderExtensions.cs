using System;
using System.IO;
using System.Text;

namespace SharpTileRenderer.RPG.Base.Map
{
    public static class MapReaderExtensions
    {
        public static string Strip(this string text, char barrier = '|')
        {
            var nl = Environment.NewLine;
            var sr = new StringReader(text);
            var line = sr.ReadLine();
            var result = new StringBuilder();
            var firstLine = true;
            while (line != null)
            {
                if (!firstLine)
                {
                    result.Append(nl);
                }

                int idx = line.IndexOf(barrier);
                if (idx >= 0)
                {
                    result.Append(line.Substring(idx + 1));
                    firstLine = false;
                }
                else if (result.Length == 0 && line.Length == 0)
                {
                    // skip leading empty lines.
                }
                else
                {
                    result.Append(line);
                    firstLine = false;
                }

                line = sr.ReadLine();
            }

            return result.ToString();
        }
    }
}