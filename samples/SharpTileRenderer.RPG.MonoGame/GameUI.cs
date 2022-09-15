using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.UI;
using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.Drawing.Monogame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpTileRenderer.RPG.MonoGame
{
    public class GameUI : DrawableGameComponent
    {
        readonly RenderComponent renderComponent;
        readonly Desktop desktop;
        readonly HorizontalStackPanel stack;
        readonly List<(bool, ILayer, CheckBox)> layers;
        
        public GameUI(Game game, RenderComponent renderComponent) : base(game)
        {
            this.DrawOrder = 20000;
            this.UpdateOrder = 10000;
            
            game.IsMouseVisible = true;
            MyraEnvironment.Game = game;
            this.renderComponent = renderComponent;

            this.layers = new List<(bool, ILayer, CheckBox)>();
            foreach (var l in renderComponent.Layers)
            {
                layers.Add((true, l, new CheckBox()));
            }
            
            stack = new HorizontalStackPanel();
            stack.GridColumn = 0;
            stack.GridRow = 0;

            desktop = new Desktop();
            desktop.HasExternalTextInput = true;
            desktop.Root = CreateUI();


            // Provide that text input
            Game.Window.TextInput += OnWindowOnTextInput;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            Game.Window.TextInput -= OnWindowOnTextInput;
        }

        void OnWindowOnTextInput(object? s, TextInputEventArgs a)
        {
            desktop.OnChar(a.Character);
        }

        Widget CreateUI()
        {
            var grid = new Grid()
            {
                RowSpacing = 8,
                ColumnSpacing = 8,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.MouseEntered += OnMouseEntered;
            grid.MouseLeft += OnMouseLeft;
            grid.Widgets.Add(stack);

            for (var index = 0; index < layers.Count; index++)
            {
                var (enabled, l, checkBox) = layers[index];
                checkBox.Text = $"{index} - {l.Name}";
                checkBox.IsChecked = enabled;
                checkBox.Click += OnClick(index);
                stack.Widgets.Add(checkBox);
            }

            return grid;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            desktop.Render();
        }

        EventHandler OnClick(int index)
        {
            return (_, _) =>
            {
                var (_, layer, cb) = this.layers[index];
                this.layers[index] = (cb.IsChecked, layer, cb);
                var array = layers.Where(t => t.Item1).Select(t => t.Item2).ToArray();
                renderComponent.SetLayers(array);
            };
        }

        public static bool MouseActive { get; private set; }
        
        void OnMouseLeft(object? sender, EventArgs e)
        {
            MouseActive = false;
        }

        void OnMouseEntered(object? sender, EventArgs e)
        {
            MouseActive = true;
        }
    }
}