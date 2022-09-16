using Serilog;
using SharpTileRenderer.Drawing;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileBlending.Matcher;
using SharpTileRenderer.TileBlending.Textures;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.Util;
using System.Collections.Generic;
using System.Linq;

namespace SharpTileRenderer.TileBlending
{
    public static class TextureBlendingTileModule
    {
        public const string FeatureFlag = "blend-layer";

        public static TextureBlendingTileModule<TTexture, TColor> For<TTexture, TColor>(ITextureOperations<TTexture, TColor> op)
            where TTexture : ITexture<TTexture>
        {
            return new TextureBlendingTileModule<TTexture, TColor>(op);
        }
    }

    public class TextureBlendingTileModule<TTexture, TColor> : IDrawingFeatureModule
        where TTexture : ITexture<TTexture>
    {
        readonly ILogger logger = SLog.ForContext<TextureBlendingTileModule<TTexture, TColor>>();

        readonly ITextureOperations<TTexture, TColor> textureOperations;
        bool reentryCheck;

        public TextureBlendingTileModule(ITextureOperations<TTexture, TColor> textureOperations)
        {
            this.textureOperations = textureOperations;
        }

        public void Initialize<TClassification>(IRenderLayerProducerConfig<TClassification> initializer) where TClassification : struct, IEntityClassification<TClassification>
        {
            initializer.MatcherFactory.RegisterTagSelector(BlendingSelectorModel.SelectorName, BlendingSpriteMatcher<TClassification>.Create);
        }

        public TextureBlendingTileModule<TTexture, TColor> WithTileSet(ITileResolver<SpriteTag, TexturedTile<TTexture>> tileSet)
        {
            this.TileSet = Optional.OfNullable(tileSet);
            return this;
        }

        public Optional<ITileResolver<SpriteTag, TexturedTile<TTexture>>> TileSet { get; private set; }

        public int PreferenceWeight => 1000;

        public void Initialize<TClassification>(IFeatureInitializer<TClassification> initializer)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            initializer.MatcherFactory.RegisterTagSelector(BlendingSelectorModel.SelectorName, BlendingSpriteMatcher<TClassification>.Create);
        }

        public Optional<ITileRenderer<TEntity>> CreateRendererForData<TEntity, TClassification>(IRenderLayerProducerConfig<TClassification> layerProducer,
                                                                                                TileMatcherModel model,
                                                                                                RenderLayerModel layer)
            where TClassification : struct, IEntityClassification<TClassification>
        {
            if (!layer.FeatureFlags.Contains(TextureBlendingTileModule.FeatureFlag))
            {
                return Optional.Empty();
            }

            if (!TileSet.TryGetValue(out var tileSet))
            {
                logger.Warning("Skipping blend-layer renderer; No tile set provided");
                return Optional.Empty();
            }

            if (reentryCheck)
            {
                logger.Warning("Skipping request to build renderer. Recursive calls are not supported by the blend-module");
                return Optional.Empty();
            }

            reentryCheck = true;
            try
            {
                if (!FindBlendSelector(layer).TryGetValue(out var blendSelector))
                {
                    logger.Warning("Skipping request to build renderer. Unable to find blend-selector in layer {Layer}", layer.Id);
                    return Optional.Empty();
                }

                if (!layerProducer.TryGetFeature<ITexturedTileRendererFeatureModule<TTexture>>(out var drawFeature, f => f.PreferenceWeight < PreferenceWeight))
                {
                    logger.Warning("Skipping request to build renderer. Unable to find parent renderer");
                    return Optional.Empty();
                }

                var blendTileSpriteTag = FindBlendTile(layer);
                var graphicTags = FindMatchingGraphics(model, blendSelector.MatchWith);
                var blendGenerator = new BlendTileGenerator<TTexture, TColor>(textureOperations, tileSet.TileSize);

                if (!tileSet.TryFind(blendTileSpriteTag, out var blendTile))
                {
                    logger.Warning("Skipping request to build renderer. Unable to find blend tile {BlendTile}", blendTileSpriteTag);
                    return Optional.Empty();
                }

                var tileRegistry = new SpriteTagTileResolver<TexturedTile<TTexture>>(tileSet.TileSize);
                var directions = new[] { TextureQuadrantIndex.North, TextureQuadrantIndex.East, TextureQuadrantIndex.South, TextureQuadrantIndex.West };
                foreach (var tag in graphicTags)
                {
                    var spriteTag = tag.AsSpriteTag().WithPrefix(blendSelector.SourcePrefix).WithQualifier(blendSelector.SourceSuffix);
                    if (!tileSet.TryFind(spriteTag, out var tile))
                    {
                        logger.Warning("Blend-Tile generator: Unable to find source tile {SourceTile}", spriteTag);
                        continue;
                    }

                    foreach (var dir in directions)
                        if (blendGenerator.TryCreateBlendTile(tile, blendTile, dir, out var generatedTile))
                        {
                            var generatedSpriteTag = tag.AsSpriteTag()
                                                        .WithPrefix(blendSelector.Prefix)
                                                        .WithQualifier(BlendingSpriteMatcher<TClassification>.BlendSuffixFor(dir));
                            tileRegistry.Add(generatedSpriteTag, generatedTile);
                        }
                }

                return drawFeature.CreateRendererForData<TEntity, TClassification>(layerProducer, model, layer, tileRegistry);
            }
            finally
            {
                reentryCheck = false;
            }
        }

        List<GraphicTag> FindMatchingGraphics(TileMatcherModel model, IReadOnlyList<string> classes)
        {
            var tags = new List<GraphicTag>();
            foreach (var tag in model.Tags)
            {
                var graphicTag = GraphicTag.From(tag.Id);
                if (graphicTag == GraphicTag.Empty)
                {
                    continue;
                }

                if (tag.Classes.Union(classes).Any())
                {
                    tags.Add(graphicTag);
                }
            }

            return tags;
        }

        Optional<BlendingSelectorModel> FindBlendSelector(RenderLayerModel layer)
        {
            if (layer.Match != null && FindBlendSelector(layer.Match).TryGetValue(out var m))
            {
                return m;
            }

            foreach (var l in layer.SubLayers)
            {
                if (FindBlendSelector(l).TryGetValue(out var mm))
                {
                    return mm;
                }
            }

            return Optional.Empty();
        }

        Optional<BlendingSelectorModel> FindBlendSelector(ISelectorModel selector)
        {
            if (selector is BlendingSelectorModel b)
            {
                return b;
            }

            foreach (var s in selector.ChildSelectors)
            {
                if (FindBlendSelector(s).TryGetValue(out var m))
                {
                    return m;
                }
            }

            return Optional.Empty();
        }

        SpriteTag FindBlendTile(RenderLayerModel renderLayerModel)
        {
            if (renderLayerModel.Properties.TryGetValue("blend-tile", out var blendTileName) && SpriteTag.Parse(blendTileName).TryGetValue(out var tag))
            {
                return tag;
            }

            return SpriteTag.Create("blend-layer", "dither-mask", null);
        }
    }
}