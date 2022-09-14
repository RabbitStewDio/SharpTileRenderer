namespace SharpTileRenderer.RPG.Base.Model
{
    public class ItemElement : RuleElementBase
    {
        public ItemElement(string id, char asciiId, string name, string? graphicTag = null, params string[]? alternativeGraphics) : base(id, asciiId, name, graphicTag, alternativeGraphics)
        {
        }
    }
}