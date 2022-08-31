using System;

namespace SharpTileRenderer.TexturePack.Operations
{
    public interface ITextureOperations<TTexture>: IDisposable
    {
        TextureCoordinateRect ToNormalized(IntDimension context, TextureCoordinateRect src);
        TextureCoordinateRect ToNative(IntDimension context, TextureCoordinateRect src);
        ITextureAtlasBuilder<TTexture> CreateAtlasBuilder();
    }

    public interface ITextureOperations<TTexture, TColor> : ITextureOperations<TTexture>
    {
        BoundedTextureData<TColor> ExtractData(TTexture srcTexture, TextureCoordinateRect rect);
        BoundedTextureData<TColor> CombineMask(BoundedTextureData<TColor> color, BoundedTextureData<TColor> mask);
        BoundedTextureData<TColor> CombineBlend(BoundedTextureData<TColor> color, BoundedTextureData<TColor> mask);
        TTexture ApplyTextureData(TTexture texture, BoundedTextureData<TColor> result);
        TTexture ApplyTextureData(TTexture texture, BoundedTextureData<TColor> result, TextureCoordinatePoint offset);

        BoundedTextureData<TColor> CreateClearTexture(IntDimension size);
        TextureCoordinateRect TileAreaForCardinalDirection(IntDimension ts, TextureQuadrantIndex dir);

        TTexture Clip(string name, TTexture texture, TextureCoordinateRect clipRegion);

        /// <summary>
        ///  Create a new texture that has been cleared.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="textureSize"></param>
        /// <param name="clearToTransparentBlack"></param>
        /// <returns></returns>
        TTexture CreateTexture(string name,
                               IntDimension textureSize,
                               bool clearToTransparentBlack = true);

        void MakeDebugVisible(BoundedTextureData<TColor> b);
    }
}
