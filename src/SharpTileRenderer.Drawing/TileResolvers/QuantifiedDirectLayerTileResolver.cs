using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.TileResolvers
{
    public class QuantifiedDirectLayerTileResolver<TEntity, TQuantity> : ILayerTileResolver<GraphicTag, (TEntity entity, TQuantity quantity)>
    {
        readonly ISpriteMatcher<(GraphicTag, TQuantity)> spriteSelector;
        readonly List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> buffer;


        public QuantifiedDirectLayerTileResolver(ISpriteMatcher<(GraphicTag, TQuantity)> spriteSelector)
        {
            this.spriteSelector = spriteSelector ?? throw new ArgumentNullException(nameof(spriteSelector));
            this.buffer = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();
        }

        public bool IsThreadSafe => spriteSelector.IsThreadSafe;

        public List<RenderInstruction<(TEntity entity, TQuantity quantity)>> ResolveTiles(int z,
                                                                                          List<SparseTagQueryResult<GraphicTag, (TEntity entity, TQuantity quantity)>> entities,
                                                                                          List<RenderInstruction<(TEntity entity, TQuantity quantity)>> result)
        {
            foreach (var (tag, entity, pos) in entities)
            {
                if (tag == GraphicTag.Empty)
                {
                    continue;
                }

                buffer.Clear();
                var smi = new SpriteMatcherInput<(GraphicTag, TQuantity)>((tag, entity.quantity), pos);
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