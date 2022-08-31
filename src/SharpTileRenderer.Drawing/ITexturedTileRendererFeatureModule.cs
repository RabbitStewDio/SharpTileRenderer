using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.Util;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Drawing
{
    public interface ITexturedTileRendererFeatureModule<TTexture> : IFeatureModule
    {
        public bool CreateRendererForData<TEntity, TClassification>(ITileDataSetProducer<TEntity> dataSet,
                                                                    Func<RenderLayerModel, ITileResolver<SpriteTag, TexturedTile<TTexture>>> ts,
                                                                    Optional<string> feature,
                                                                    [MaybeNullWhen(false)] out IRenderLayerProducer<TClassification> c)
            where TClassification : struct, IEntityClassification<TClassification>;
    }
}