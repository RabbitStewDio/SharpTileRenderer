using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.TileResolvers
{
    public class DirectLayerTileResolver<TEntity> : ILayerTileResolver<GraphicTag, TEntity>
    {
        readonly ISpriteMatcher<GraphicTag> spriteSelector;
        readonly List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)> buffer;

        public DirectLayerTileResolver(ISpriteMatcher<GraphicTag> spriteSelector)
        {
            this.spriteSelector = spriteSelector ?? throw new ArgumentNullException(nameof(spriteSelector));
            this.buffer = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();
        }

        public bool IsThreadSafe => spriteSelector.IsThreadSafe;

        public List<RenderInstruction<TEntity>> ResolveTiles(int z,
                                                             List<SparseTagQueryResult<GraphicTag, TEntity>> entities,
                                                             List<RenderInstruction<TEntity>> result)
        {
            foreach (var (tag, entity, pos) in entities)
            {
                if (tag == GraphicTag.Empty)
                {
                    continue;
                }

                buffer.Clear();
                var smi = new SpriteMatcherInput<GraphicTag>(tag, pos);
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