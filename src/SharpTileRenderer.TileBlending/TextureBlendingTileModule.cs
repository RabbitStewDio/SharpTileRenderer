using SharpTileRenderer.Drawing;
using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileBlending.Matcher;
using SharpTileRenderer.TileBlending.Textures;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using System;
using System.Diagnostics.CodeAnalysis;

// the CSharp compiler is messing up the MaybeNullWhen(false) annotation on "layerProducer.TryGetFeature". So screw the bogus compiler warning
#pragma warning disable CS8600

namespace SharpTileRenderer.TileBlending
{
    public class TextureBlendingTileModule : IDrawingFeatureModule
    {
        public void Initialize<TClassification>(IFeatureInitializer<TClassification> initializer) where TClassification : struct, IEntityClassification<TClassification>
        {
            initializer.MatcherFactory.RegisterTagSelector(BlendingSelectorModel.SelectorName, BlendingSpriteMatcher<TClassification>.Create);
        }

        public bool CreateRendererForData<TEntity, TClassification>(RenderLayerProducerData<TClassification> layerProducer,
                                                                    ITileDataSetProducer<TEntity> dataSet,
                                                                    [MaybeNullWhen(false)] out IRenderLayerProducer<TClassification> c)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            
            if (!layerProducer.TryGetFeature(out ITextureTileModule f))
            {
                c = default;
                return false;
            }

            var to = new First<TEntity, TClassification>(layerProducer, dataSet);
            c = f.WithTextureOperation(to);
            return c != null;
        }

        class First<TEntity, TClassification> : ITileSetOperationFunc<IRenderLayerProducer<TClassification>?>
            where TClassification : struct, IEntityClassification<TClassification>
        {
            readonly RenderLayerProducerData<TClassification> layerProducer;
            readonly ITileDataSetProducer<TEntity> tileDataSetProducer;

            public First(RenderLayerProducerData<TClassification> layerProducer,
                         ITileDataSetProducer<TEntity> tileDataSetProducer)
            {
                this.layerProducer = layerProducer;
                this.tileDataSetProducer = tileDataSetProducer;
            }

            public IRenderLayerProducer<TClassification>? Apply<TTexture>(ITexturedTileModule<TTexture> mod)
                where TTexture : ITexture<TTexture>
            {
                if (!mod.TileSet.TryGetValue(out var ts))
                {
                    return null;
                }

                ITileResolver<SpriteTag, TexturedTile<TTexture>> Produce(RenderLayerModel m)
                {
                    return mod.WithTextureOperation(new Second<TTexture>(m, ts)) ?? throw new Exception();
                }
                
                if (!layerProducer.TryGetFeature(out ITexturedTileRendererFeatureModule<TTexture> t))
                {
                    return null;
                }
                
                if (t.CreateRendererForData<TEntity, TClassification>(tileDataSetProducer, Produce, "blend-layer", out var p))
                {
                    return p;
                }

                return null;
            }
        }

        class Second<TTexture> : ITextureOperationFunc<ITileResolver<SpriteTag, TexturedTile<TTexture>>, TTexture>
            where TTexture : ITexture<TTexture>
        {
            readonly RenderLayerModel renderLayerModel;
            readonly ITileResolver<SpriteTag, TexturedTile<TTexture>> modTileSet;

            public Second(RenderLayerModel renderLayerModel, ITileResolver<SpriteTag, TexturedTile<TTexture>> modTileSet)
            {
                this.renderLayerModel = renderLayerModel;
                this.modTileSet = modTileSet;
            }

            public ITileResolver<SpriteTag, TexturedTile<TTexture>> Apply<TColor>(ITextureOperations<TTexture, TColor> op)
            {
                var blendOp = new BlendTileGenerator<TTexture, TColor>(op, modTileSet.TileSize);
                var blendTile = renderLayerModel.Properties["blend-tile"];

                var blendTileTag = SpriteTag.Parse(blendTile);
                if (!blendTileTag.TryGetValue(out var tag))
                {
                    tag = SpriteTag.Create("t", "dither-mask", null);
                }
                return new BlendedTileResolver<TTexture>(modTileSet, tag, blendOp);
            }
        }
    }
}