using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Selectors;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.TileResolvers
{
    public class ClassificationLayerTileResolver<TEntityClassification, TEntity> : ILayerTileResolver<TEntityClassification, TEntity>
        where TEntityClassification : struct, IEntityClassification<TEntityClassification>
    {
        readonly List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> buffer;
        readonly ISpriteMatcher<TEntityClassification> spriteSelector;

        public ClassificationLayerTileResolver(ISpriteMatcher<TEntityClassification> spriteSelector)
        {
            this.spriteSelector = spriteSelector;
            this.buffer = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();
        }

        public bool IsThreadSafe => spriteSelector.IsThreadSafe;

        public List<RenderInstruction<TEntity>> ResolveTiles(int z,
                                                             List<SparseTagQueryResult<TEntityClassification, TEntity>> entities,
                                                             List<RenderInstruction<TEntity>> result)
        {
            foreach (var (tag, entity, pos) in entities)
            {
                if (tag.IsEmpty)
                {
                    continue;
                }

                buffer.Clear();
                var smi = new SpriteMatcherInput<TEntityClassification>(tag, pos);
                if (!spriteSelector.Match(smi, z, buffer))
                {
                    continue;
                }

                foreach (var b in buffer)
                {
                    result.Add(new RenderInstruction<TEntity>(entity, b.tag, b.spriteOffset, b.pos));
                }
            }

            return result;
        }
    }
}