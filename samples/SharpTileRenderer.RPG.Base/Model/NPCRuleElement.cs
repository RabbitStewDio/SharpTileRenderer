namespace SharpTileRenderer.RPG.Base.Model
{
    public class NPCRuleElement : RuleElementBase
    {
        public NPCRuleElement(string id, char asciiId, string name, string? graphicTag = null, params string[]? alternativeGraphics) : base(id, asciiId, name, graphicTag, alternativeGraphics)
        {
            WalkingSpeed = 1;
        }

        public float WalkingSpeed { get; }
    }
}