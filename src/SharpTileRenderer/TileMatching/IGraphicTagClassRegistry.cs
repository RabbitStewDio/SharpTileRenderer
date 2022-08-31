using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.TileMatching
{
    public interface IGraphicTagMetaDataRegistry<TEntityClassification>
        where TEntityClassification : struct, IEntityClassification<TEntityClassification>
    {
        public IEnumerable<GraphicTag> KnownTags { get; }
        
        public bool HasFlag(GraphicTag t, string flag);
        public bool TryGetProperty(GraphicTag t, string propertyName, [MaybeNullWhen(false)] out string value);

        public TEntityClassification QueryClasses(GraphicTag t);

        /// <summary>
        ///   Classes are special flags used during matching. The set of classes is
        ///   shared across all graphic tags and is used during tile selection.
        ///   The number of classes that can exist is limited by the EntityClassification
        ///   registry used.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="className"></param>
        public void AddClass(GraphicTag tag, string className);

        /// <summary>
        ///    Adds a general purpose flag to the given graphic tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="flag"></param>
        public void AddFlag(GraphicTag tag, string flag);

        /// <summary>
        ///    Adds some general purpose data to the given graphic tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void AddProperty(GraphicTag tag, string propertyName, string value);
    }
}