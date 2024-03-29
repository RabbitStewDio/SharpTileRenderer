﻿using SharpTileRenderer.TexturePack.Operations;
using SharpTileRenderer.TexturePack.Tiles;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.TexturePack.Atlas
{
    public static class TextureAtlasBuilder
    {
        public const int DefaultMaxTextureSize = 4096;
        public const int DefaultPadding = 2;
        
    }
    
    /// <summary>
    ///   Merges textures into one or more texture atlases. This class arranges
    ///   textures into bands and splits those bands into lines. It works best
    ///   with textures of uniform sizes.
    /// </summary>
    public class TextureAtlasBuilder<TTexture, TColor>
        where TTexture : ITexture<TTexture>
    {
        readonly TreeNode root;
        readonly ITextureOperations<TTexture, TColor> textureOperations;
        readonly int padding;

        public TTexture Texture { get; }

        public TextureAtlasBuilder(ITextureOperations<TTexture, TColor> textureOperations,
                                   TTexture texture,
                                   int padding = TextureAtlasBuilder.DefaultPadding)
        {
            root = new TreeNode(texture.Bounds);
            this.Texture = texture;
            this.textureOperations = textureOperations;
            this.padding = padding;
        }

        public bool Insert(TTexture tile, [MaybeNullWhen(false)] out TTexture result)
        {
            if (!tile.Valid)
            {
                result = tile;
                return true;
            }

            var res = root.Insert(tile, padding);
            if (res != null)
            {
                if (res.Harvest(textureOperations, Texture, out result))
                {
                    return true;
                }
            }

            result = default!;
            return false;
        }
        
        class TreeNode
        {
            bool HasTexture { get; set; }
            TTexture? Texture { get; set; }
            TextureCoordinateRect CellBounds { get; }
            TreeNode? Left { get; set; }
            TreeNode? Right { get; set; }

            public TreeNode(TextureCoordinateRect bounds)
            {
                CellBounds = bounds;
            }

            public TreeNode? Insert(TTexture tex, int padding)
            {
                if (Left != null && Right != null)
                {
                    // Is not a leaf node. Therefore delegate to the appropriate detail node.
                    var maybeLeft = Left.Insert(tex, padding);
                    if (maybeLeft != null)
                    {
                        return maybeLeft;
                    }

                    return Right.Insert(tex, padding);
                }

                if (HasTexture)
                {
                    // There is already something here 
                    return null;
                }

                var texBounds = tex.Bounds;
                if (texBounds.Width > CellBounds.Width ||
                    texBounds.Height > CellBounds.Height)
                {
                    // does not fit into the available space
                    return null;
                }

                if (texBounds.Width == CellBounds.Width &&
                    texBounds.Height == CellBounds.Height)
                {
                    HasTexture = true;
                    Texture = tex;
                    return this;
                }

                if ((CellBounds.Width - texBounds.Width) > (CellBounds.Height - texBounds.Height))
                {
                    // vertical split 
                    Left = new TreeNode(new TextureCoordinateRect(CellBounds.X, CellBounds.Y, texBounds.Width, CellBounds.Height));
                    Right = new TreeNode(new TextureCoordinateRect(CellBounds.X + padding + texBounds.Width, CellBounds.Y,
                                                                   CellBounds.Width - texBounds.Width - padding, CellBounds.Height));
                }
                else
                {
                    Left = new TreeNode(new TextureCoordinateRect(CellBounds.X, CellBounds.Y, CellBounds.Width, texBounds.Height));
                    Right = new TreeNode(new TextureCoordinateRect(CellBounds.X, CellBounds.Y + padding + texBounds.Height,
                                                                   CellBounds.Width,
                                                                   CellBounds.Height - texBounds.Height - padding));
                }

                return Left.Insert(tex, padding);
            }

            public bool Harvest(ITextureOperations<TTexture, TColor> ops,
                                TTexture targetTexture,
                                [MaybeNullWhen(false)] out TTexture result)
            {
                if (HasTexture)
                {
                    Debug.Assert(Texture != null);
                    var textureBounds = Texture.Bounds;
                    // Debug.Log("Extract " + Texture.Name + " via " + textureBounds);
                    var localTextureBounds = new TextureCoordinateRect
                        (0, 0, textureBounds.Width, textureBounds.Height);
                    var srcData = ops.ExtractData(Texture, localTextureBounds);
                    ops.MakeDebugVisible(srcData);
                    ops.ApplyTextureData(targetTexture, srcData,
                                         new TextureCoordinatePoint(CellBounds.X, CellBounds.Y));

                    result = ops.Clip(targetTexture.Name + ":" + Texture.Name, targetTexture, CellBounds);
                    // Debug.Log("Harvest: " + result.Name + ":" + Texture.Name + " " + result.Bounds + " @ " + textureBounds);
                    return true;
                }

                if (Left != null && Right != null)
                {
                    if (Left.Harvest(ops, targetTexture, out var l))
                    {
                        result = l;
                        return true;
                    }

                    return Right.Harvest(ops, targetTexture, out result);
                }

                result = default;
                return false;
            }
        }
    }
}
