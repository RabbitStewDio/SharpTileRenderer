using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.TileResolvers
{
    public class QuantifiedClassificationLayerTileResolver<TEntityClassification, TEntity, TQuantity> : ILayerTileResolver<TEntityClassification, (TEntity entity, TQuantity quantity)>
        where TEntityClassification : struct, IEntityClassification<TEntityClassification>
        where TQuantity : IComparable<TQuantity>
    {
        readonly List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> buffer;
        readonly ISpriteMatcher<(TEntityClassification, TQuantity)> spriteSelector;

        public QuantifiedClassificationLayerTileResolver(ISpriteMatcher<(TEntityClassification, TQuantity)> spriteSelector)
        {
            this.spriteSelector = spriteSelector;
            this.buffer = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();
        }

        public bool IsThreadSafe => spriteSelector.IsThreadSafe;

        public List<RenderInstruction<(TEntity entity, TQuantity quantity)>> ResolveTiles(int z,
                                                                                          List<SparseTagQueryResult<TEntityClassification, (TEntity entity, TQuantity quantity)>> entities,
                                                                                          List<RenderInstruction<(TEntity entity, TQuantity quantity)>> result)
        {
            foreach (var (tag, entity, pos) in entities)
            {
                if (tag.IsEmpty)
                {
                    continue;
                }

                buffer.Clear();
                var smi = new SpriteMatcherInput<(TEntityClassification, TQuantity)>((tag, entity.quantity), pos);
                if (!spriteSelector.Match(smi, z, buffer))
                {
                    continue;
                }

                foreach (var b in buffer)
                {
                    result.Add(new RenderInstruction<(TEntity, TQuantity)>(entity, b.tag, b.spriteOffset, b.pos));
                }
            }

            return result;
        }
    }
}