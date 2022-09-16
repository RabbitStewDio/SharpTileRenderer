using Microsoft.Extensions.ObjectPool;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Selectors;
using SharpTileRenderer.Util;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Drawing.TileResolvers
{
    public class DirectLayerTileResolver<TEntity> : ILayerTileResolver<GraphicTag, TEntity>
    {
        readonly ISpriteMatcher<GraphicTag> spriteSelector;
        readonly ObjectPool<List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>> pool;
 
        public DirectLayerTileResolver(ISpriteMatcher<GraphicTag> spriteSelector)
        {
            this.spriteSelector = spriteSelector ?? throw new ArgumentNullException(nameof(spriteSelector));
            this.pool = new DefaultObjectPool<List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>>
                (new ListObjectPolicy<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>());
        }

        public bool IsThreadSafe => spriteSelector.IsThreadSafe;

        public List<RenderInstruction<TEntity>> ResolveTiles(int z,
                                                             List<SparseTagQueryResult<GraphicTag, TEntity>> entities,
                                                             List<RenderInstruction<TEntity>> result)
        {
            var buffer = pool.Get();
            try
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
            }
            finally
            {
                pool.Return(buffer);
            }

            return result;
        }
    }
}