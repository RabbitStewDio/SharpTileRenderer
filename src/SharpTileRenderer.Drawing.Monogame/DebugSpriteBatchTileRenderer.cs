using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.TexturePack.Tiles;
using SharpTileRenderer.TileMatching;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTileRenderer.Drawing.Monogame
{
    public class DebugSpriteBatchTileRenderer<TEntity, TTile> : ITileRenderer<TEntity>
        where TTile : ITexturedTile<XnaTexture>
    {
        readonly GraphicsDeviceManager graphicsDeviceManager;
        readonly RasterizerState enableScissorTest;
        readonly ITileResolver<SpriteTag, TTile> tileRepository;
        readonly SpriteFont font;
        readonly ITileRenderer<TEntity>? parent;
        SpriteBatch? spriteBatch;

        public DebugSpriteBatchTileRenderer(ITileRenderer<TEntity>? parent, 
                                            GraphicsDeviceManager graphicsDeviceManager,
                                            ITileResolver<SpriteTag, TTile> tileRepository,
                                            SpriteFont font)
        {
            this.parent = parent;
            this.graphicsDeviceManager = graphicsDeviceManager ?? throw new ArgumentNullException(nameof(graphicsDeviceManager));
            this.tileRepository = tileRepository ?? throw new ArgumentNullException(nameof(tileRepository));
            this.font = font;
            graphicsDeviceManager.DeviceCreated += OnDeviceCreated;
            graphicsDeviceManager.DeviceReset += OnDeviceCreated;
            graphicsDeviceManager.DeviceDisposing += OnDeviceDisposed;
            this.enableScissorTest = RasterizerState.CullCounterClockwise.Copy();
            this.enableScissorTest.ScissorTestEnable = true;

            var device = graphicsDeviceManager.GraphicsDevice;
            if (device != null)
            {
                this.spriteBatch = new SpriteBatch(device);
            }
        }

        void OnDeviceDisposed(object? sender, EventArgs e)
        {
            this.spriteBatch = null;
        }

        void OnDeviceCreated(object? sender, EventArgs e)
        {
            this.spriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);
        }

        SpriteBatch SpriteBatch => spriteBatch ?? throw new InvalidOperationException();

        public ValueTask RenderBatchAsync(IViewPort vp, List<ScreenRenderInstruction<TEntity>> renderInstructionBuffer, CancellationToken cancellationToken)
        {
            parent?.RenderBatch(vp, renderInstructionBuffer);
            RenderBatch(vp, renderInstructionBuffer);
            return new ValueTask(Task.CompletedTask);
        }

        public void RenderBatch(IViewPort vp, List<ScreenRenderInstruction<TEntity>> renderInstructionBuffer)
        {
            parent?.RenderBatch(vp, renderInstructionBuffer);
            
            var bounds = vp.PixelBounds;
            var sb = SpriteBatch;
            using var state = sb.GraphicsDevice.SaveState();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, enableScissorTest, null,
                     Matrix.CreateTranslation(bounds.X, bounds.Y, 0));
            sb.GraphicsDevice.ScissorRectangle = new Rectangle((int)bounds.X, (int)bounds.Y, (int)Math.Ceiling(bounds.Width), (int)Math.Ceiling(bounds.Height));

            foreach (var ri in renderInstructionBuffer)
            {
                var renderPos = ri.RenderPosition;
                var spriteTag = ri.RenderInstruction.Tag;
                var tint = Color.Red;
                var origin = new Vector2(renderPos.X, renderPos.Y);
                if (tileRepository.TryFind(spriteTag, out var tile) && tile.Texture.TryGetValue(out _))
                {
                    tint = Color.White;
                }
                else
                {
                    tileRepository.TryFind(spriteTag, out _);
                }

                sb.DrawString(font, $"{ri.RenderInstruction.MapPosition.X},{ri.RenderInstruction.MapPosition.Y}", origin, tint);
            }

            sb.End();
        }
    }
}