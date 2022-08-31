using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Util;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Drawing.Monogame
{
    public class XnaTextureTileModule : IDrawingFeatureModule, ITexturedTileModule<XnaTexture>, ITexturedTileRendererFeatureModule<XnaTexture>
    {
        readonly GraphicsDeviceManager mgr;
        readonly XnaTextureOperations textureOperations;
        Optional<SpriteFont> debugRenderer;

        XnaTextureTileModule(GraphicsDeviceManager mgr)
        {
            this.mgr = mgr;
            textureOperations = new XnaTextureOperations(mgr);
        }

        public void Initialize<TClassification>(IFeatureInitializer<TClassification> initializer) where TClassification : struct, IEntityClassification<TClassification>
        {
        }

        public XnaTextureTileModule WithTileSet(SpriteTagTileResolver<TexturedTile<XnaTexture>> tileSet)
        {
            this.TileSet = tileSet;
            return this;
        }

        public XnaTextureTileModule WithDebugRenderer(SpriteFont font)
        {
            this.debugRenderer = font;
            return this;
        }

        public static XnaTextureTileModule For(GraphicsDeviceManager mgr) => new XnaTextureTileModule(mgr);

        public bool CreateRendererForData<TEntity, TClassification>(RenderLayerProducerData<TClassification> p,
                                                      ITileDataSetProducer<TEntity> dataSet,
                                                      [MaybeNullWhen(false)] out IRenderLayerProducer<TClassification> c)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (TileSet.TryGetValue(out var ts))
            {
                return CreateRendererForData(dataSet, l => ts, Optional.Empty<string>(), out c);
            }

            c = null;
            return false;
        }


        public T WithTextureOperation<T>(ITileSetOperationFunc<T> t)
        {
            return t.Apply(this);
        }

        public Optional<ITileResolver<SpriteTag, TexturedTile<XnaTexture>>> TileSet { get; private set; }

        public T WithTextureOperation<T>(ITextureOperationFunc<T, XnaTexture> t)
        {
            return t.Apply(textureOperations);
        }

        public bool CreateRendererForData<TEntity, TClassification>(ITileDataSetProducer<TEntity> dataSet,
                                                      Func<RenderLayerModel, ITileResolver<SpriteTag, TexturedTile<XnaTexture>>> ts,
                                                      Optional<string> feature,
                                                      [MaybeNullWhen(false)] out IRenderLayerProducer<TClassification> c)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (debugRenderer.TryGetValue(out var font))
            {
                c = new XnaDebugRenderLayerProducer<TEntity, TClassification>(dataSet, feature, mgr, font, ts);
            }
            else
            {
                c = new XnaRenderLayerProducer<TEntity, TClassification>(dataSet, feature, mgr, ts);
            }

            return true;
        }
    }
}