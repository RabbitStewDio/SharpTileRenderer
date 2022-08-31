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
    public class XnaRenderLayerProducer<TEntity, TClassification> : RenderLayerProducerBase<TEntity, TClassification>
        where TClassification : struct, IEntityClassification<TClassification>
    {
        readonly GraphicsDeviceManager graphics;
        readonly Func<RenderLayerModel, ITileResolver<SpriteTag, TexturedTile<XnaTexture>>> tileSprites;
        readonly SpriteBatch? spriteBatch;

        public XnaRenderLayerProducer(ITileDataSetProducer<TEntity> dataSets,
                                      Optional<string> featureFlag,
                                      GraphicsDeviceManager graphics,
                                      Func<RenderLayerModel, ITileResolver<SpriteTag, TexturedTile<XnaTexture>>> tileSprites) : base(dataSets, featureFlag)
        {
            this.graphics = graphics;
            this.tileSprites = tileSprites;
            var graphicsDevice = this.graphics.GraphicsDevice;
            if (graphicsDevice != null)
            {
                this.spriteBatch = new SpriteBatch(graphicsDevice);
            }
            else
            {
                this.spriteBatch = null;
            }
        }

        protected override ITileRenderer<TEntity> CreateRenderer(RenderLayerModel layer, IRenderLayerProducerData<TClassification> parameters)
        {
            return new SpriteBatchTileRenderer<TEntity, TexturedTile<XnaTexture>>(layer.Id ?? "", graphics, tileSprites(layer), spriteBatch);
        }
    }
}