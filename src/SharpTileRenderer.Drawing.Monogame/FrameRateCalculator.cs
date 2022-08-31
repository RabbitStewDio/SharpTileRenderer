using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace SharpTileRenderer.Drawing.Monogame
{
    public class FrameRateCalculator : GameComponent
    {
        class BeginHandler : ComponentBase
        {
            readonly FrameRateCalculator calc;

            public BeginHandler(FrameRateCalculator calc)
            {
                this.calc = calc;
                this.DrawOrder = int.MinValue;
                this.UpdateOrder = int.MinValue;
            }

            public override void Draw(GameTime gameTime)
            {
                calc.RecordDraw();
                calc.BeginTime();
                base.Draw(gameTime);
            }

            public override void Update(GameTime gameTime)
            {
                calc.RecordUpdate(gameTime);
                calc.BeginTime();
                base.Update(gameTime);
            }
        }

        class EndHandler : ComponentBase
        {
            readonly FrameRateCalculator calc;

            public EndHandler(FrameRateCalculator calc)
            {
                this.calc = calc;
            }

            public override void Draw(GameTime gameTime)
            {
                calc.EndTime();
            }

            public override void Update(GameTime gameTime)
            {
                calc.EndTime();
            }
        }

        static readonly TimeSpan second = TimeSpan.FromSeconds(1.0);
        readonly Stopwatch usedTime;
        readonly BeginHandler beginHandler;
        readonly EndHandler endHandler;
        TimeSpan elapsedTime;
        int frameCounter;
        double relativeCpuTime;
        int updateCounter;

        public FrameRateCalculator(Game game) : base(game)
        {
            this.beginHandler = new BeginHandler(this);
            this.endHandler = new EndHandler(this);

            game.Components.ComponentAdded += OnComponentAdded;
            game.Components.ComponentRemoved += OnComponentRemoved;
            this.elapsedTime = TimeSpan.Zero;
            this.usedTime = new Stopwatch();
        }

        void OnComponentRemoved(object? sender, GameComponentCollectionEventArgs e)
        {
            if (e.GameComponent != this) return;
            Game.Components.Remove(beginHandler);
            Game.Components.Remove(endHandler);
            this.elapsedTime = TimeSpan.Zero;
            this.usedTime.Reset();
        }

        void OnComponentAdded(object? sender, GameComponentCollectionEventArgs e)
        {
            if (e.GameComponent != this) return;
            Game.Components.Add(beginHandler);
            Game.Components.Add(endHandler);
        }

        public int FrameRate { get; private set; }

        public int UpdateRate { get; private set; }

        public void BeginTime() => this.usedTime.Start();

        public void RecordDraw()
        {
            this.frameCounter += 1;
            this.Game.Window.Title = ToString();
        }

        public void EndTime() => this.usedTime.Stop();

        public override string ToString() => $"Updates: {this.UpdateRate} Draw: {(object)this.FrameRate} - %CPU: {(this.relativeCpuTime * 100.0)}";

        public void RecordUpdate(GameTime time)
        {
            this.elapsedTime += time.ElapsedGameTime;
            if (this.elapsedTime > second)
            {
                this.relativeCpuTime = this.usedTime.Elapsed.TotalSeconds / this.elapsedTime.TotalSeconds;
                this.FrameRate = this.frameCounter;
                this.UpdateRate = this.updateCounter;
                // reset elapsed time towards zero, but preserve fractional seconds.
                this.elapsedTime -= TimeSpan.FromSeconds(Math.Floor(elapsedTime.TotalSeconds));
                this.usedTime.Reset();
                this.frameCounter = 0;
                this.updateCounter = 0;
            }

            this.updateCounter += 1;
        }
    }
}