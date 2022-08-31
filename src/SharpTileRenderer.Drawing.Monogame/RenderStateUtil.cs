using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SharpTileRenderer.Drawing.Monogame
{
    public static class RenderStateUtil
    {
        public readonly struct GraphicsState: IDisposable
        {
            readonly GraphicsDevice device;
            readonly RasterizerState rasterizerState;
            readonly Rectangle scissorRectangle;
            readonly BlendState blendState;

            public GraphicsState(GraphicsDevice device)
            {
                this.device = device;
                this.rasterizerState = device.RasterizerState;
                this.scissorRectangle = device.ScissorRectangle;
                this.blendState = device.BlendState;
            }

            public void RestoreState()
            {
                device.RasterizerState = rasterizerState;
                device.ScissorRectangle = scissorRectangle;
                device.BlendState = blendState;
            }

            public void Dispose()
            {
                RestoreState();
            }
        }

        public static GraphicsState SaveState(this GraphicsDevice d)
        {
            return new GraphicsState(d);
        }

        public static RasterizerState Copy(this RasterizerState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            return new RasterizerState()
            {
                ScissorTestEnable = state.ScissorTestEnable,
                CullMode = state.CullMode,
                DepthBias = state.DepthBias,
                SlopeScaleDepthBias = state.SlopeScaleDepthBias,
                DepthClipEnable = state.DepthClipEnable,
                FillMode = state.FillMode,
                MultiSampleAntiAlias = state.MultiSampleAntiAlias
            };
        }
    }
}