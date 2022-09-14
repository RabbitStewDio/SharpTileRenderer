using System;
using System.Collections.Generic;

namespace SharpTileRenderer.RPG.Base.Model
{
    public class RuleElementBase : IRuleElement
    {
        public RuleElementBase(string id, char asciiId, string name, string? graphicTag = null, params string[]? alternativeGraphics)
        {
            this.Id = id;
            this.AsciiId = asciiId;
            this.Name = name;
            this.GraphicTag = graphicTag;
            this.AlternativeGraphicTags = alternativeGraphics == null ? Array.Empty<string>() : (string[])alternativeGraphics.Clone();
        }

        public IReadOnlyList<string> AlternativeGraphicTags { get; }

        public string? GraphicTag { get; }

        public string Name { get; }
        public char AsciiId { get; }
        public string Id { get; }
    }
}