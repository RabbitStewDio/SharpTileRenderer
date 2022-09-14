using System.Collections.Generic;

namespace SharpTileRenderer.RPG.Base.Model
{
    public static class RuleElementExtensions
    {
        public static List<string> AllGraphicTags<TRuleElement>(this TRuleElement t, List<string>? tags = null)
            where TRuleElement: IRuleElement
        {
            tags ??= new List<string>();
            if (t.GraphicTag != null)
            {
                tags.Add(t.GraphicTag);
            }

            tags.AddRange(t.AlternativeGraphicTags);
            return tags;
        }
    }
}