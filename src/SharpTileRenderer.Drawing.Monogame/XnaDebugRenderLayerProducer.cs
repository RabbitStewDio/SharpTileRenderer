using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Util;
using System;

namespace SharpTileRenderer.Drawing.Monogame
{
    public class XnaDebugRenderLayerProducer<TEntity, TClassification> : RenderLayerProducerBase<TEntity, TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        readonly GraphicsDeviceManager graphics;
        readonly SpriteFont font;
        readonly Func<RenderLayerModel, ITileResolver<SpriteTag, TexturedTile<XnaTexture>>> tileSprites;

        public XnaDebugRenderLayerProducer(ITileDataSetProducer<TEntity> dataSets,
                                           Optional<string> featureFlag,
                                           GraphicsDeviceManager graphics,
                                           SpriteFont font,
                                           Func<RenderLayerModel, ITileResolver<SpriteTag, TexturedTile<XnaTexture>>> tileSprites) : base(dataSets, featureFlag)
        {
            this.graphics = graphics;
            this.font = font;
            this.tileSprites = tileSprites;
        }

        protected override ITileRenderer<TEntity> CreateRenderer(RenderLayerModel layer, IRenderLayerProducerData<TClassification> parameters)
        {
            return new DebugRenderer<TEntity, TexturedTile<XnaTexture>>(graphics, tileSprites(layer), font);
        }
    }
}