using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using SharpTileRenderer.Drawing.Rendering;
using SharpTileRenderer.Drawing.ViewPorts;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.TexturePack.Tiles;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SpriteTag = SharpTileRenderer.TileMatching.SpriteTag;

namespace SharpTileRenderer.Drawing.Monogame
{
    public class SpriteBatchTileRenderer<TEntity, TTile> : ITileRenderer<TEntity>
        where TTile : ITexturedTile<XnaTexture>
    {
        static readonly ILogger logger = SLog.ForContext<SpriteBatchTileRenderer<TEntity, TTile>>();
        readonly string name;
        readonly GraphicsDeviceManager graphicsDeviceManager;
        readonly RasterizerState enableScissorTest;
        readonly ITileResolver<SpriteTag, TTile> tileRepository;
        readonly HashSet<SpriteTag> warnedItems;
        SpriteBatch? spriteBatch;

        public SpriteBatchTileRenderer(string name,
                                       GraphicsDeviceManager graphicsDeviceManager,
                                       ITileResolver<SpriteTag, TTile> tileRepository,
                                       SpriteBatch? sb = null)
        {
            this.name = name;
            this.graphicsDeviceManager = graphicsDeviceManager ?? throw new ArgumentNullException(nameof(graphicsDeviceManager));
            this.tileRepository = tileRepository ?? throw new ArgumentNullException(nameof(tileRepository));
            graphicsDeviceManager.DeviceCreated += OnDeviceCreated;
            graphicsDeviceManager.DeviceReset += OnDeviceCreated;
            graphicsDeviceManager.DeviceDisposing += OnDeviceDisposed;
            this.enableScissorTest = RasterizerState.CullCounterClockwise.Copy();
            this.enableScissorTest.ScissorTestEnable = true;
            this.warnedItems = new HashSet<SpriteTag>();

            this.spriteBatch = MaybeCreateSpriteBatch(sb);
        }

        SpriteBatch? MaybeCreateSpriteBatch(SpriteBatch? sb)
        {
            if (sb != null)
            {
                if (sb.GraphicsDevice == graphicsDeviceManager.GraphicsDevice)
                {
                    return sb;
                }
            }

            var device = graphicsDeviceManager.GraphicsDevice;
            return device != null ? new SpriteBatch(device) : null;
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
            RenderBatch(vp, renderInstructionBuffer);
            return new ValueTask(Task.CompletedTask);
        }

        public void RenderBatch(IViewPort vp, List<ScreenRenderInstruction<TEntity>> renderInstructionBuffer)
        {
            var bounds = vp.PixelBounds;
            var sb = SpriteBatch;
            using var state = sb.GraphicsDevice.SaveState();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, enableScissorTest, null,
                     Matrix.CreateTranslation(bounds.X, bounds.Y, 0));
            sb.GraphicsDevice.ScissorRectangle = new Rectangle((int)bounds.X, (int)bounds.Y, (int)Math.Ceiling(bounds.Width), (int)Math.Ceiling(bounds.Height));
            var drawCount = sb.GraphicsDevice.Metrics.DrawCount;
            var spriteCount = sb.GraphicsDevice.Metrics.SpriteCount;

            foreach (var ri in renderInstructionBuffer)
            {
                var renderPos = ri.RenderPosition;
                var spriteTag = ri.RenderInstruction.Tag;
                if (tileRepository.TryFind(spriteTag, out var tile) && tile.Texture.TryGetValue(out var texture))
                {
                    var textureBounds = texture.Bounds;
                    var anchor = tile.Anchor;
                    var destPos = new Rectangle((int)(Math.Floor(renderPos.X - anchor.X)),
                                                (int)(Math.Floor(renderPos.Y - anchor.Y)),
                                                textureBounds.Width,
                                                textureBounds.Height);
                    var tint = Color.White;
                    sb.Draw(texture.Texture, destPos, textureBounds.ToXna(), tint);
                }
                else if (!warnedItems.Contains(spriteTag))
                {
                    logger.Warning("Unable to locate tile for {SpriteTag}", spriteTag);
                    warnedItems.Add(spriteTag);
                }
            }

            sb.End();

            logger.Debug("{Name} - DrawCount: {DrawCount} - SpriteCount: {SpriteCount}", name, 
                         (sb.GraphicsDevice.Metrics.DrawCount - drawCount), 
                         (sb.GraphicsDevice.Metrics.SpriteCount - spriteCount));
        }
    }
}