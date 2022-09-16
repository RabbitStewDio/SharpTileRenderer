using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using SharpTileRenderer.Drawing.Layers;
using SharpTileRenderer.Drawing.Monogame;
using SharpTileRenderer.Drawing.ViewPorts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpTileRenderer.Strategy.MonoGame
{
    public class GameUI : DrawableGameComponent
    {
        readonly DragGestureRecognizer dragRecognizer;
        readonly RenderComponent renderComponent;
        readonly Desktop desktop;
        readonly HorizontalStackPanel stack;
        readonly List<(bool, ILayer, CheckBox)> layers;

        public GameUI(Game game, RenderComponent renderComponent) : base(game)
        {
            this.DrawOrder = 20000;
            this.UpdateOrder = 10000;

            this.dragRecognizer = new DragGestureRecognizer();
            this.dragRecognizer.DragStarted += OnDragStarted;
            this.dragRecognizer.Dragging += OnDragging;
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

        VirtualMapCoordinate dragStartPosition;

        void OnDragStarted(object? sender, DragGestureRecognizer.DragEvent e)
        {
            if (desktop.MouseInsideWidget != null)
            {
                Console.WriteLine("Not dragging inside widget");
                return;
            }

            e.DragConfirmed = true;
            dragStartPosition = renderComponent.ViewPort!.Focus;
            Console.WriteLine("Drag Started: " + dragStartPosition);
        }

        void OnDragging(object? sender, DragGestureRecognizer.DragEvent e)
        {
            var vp = renderComponent.ViewPort;
            if (vp == null) return;

            var currentFocus = vp.Focus;
            var delta = Mouse.GetState().Position - e.DragStartPosition;
            var targetScreenPos = vp.PixelBounds.Center + delta.ToScreenPosition();
            var scrollTarget = vp.ScreenSpaceNavigator.TranslateViewToWorld(vp, targetScreenPos).VirtualCoordinate;
            var mapDelta = scrollTarget - currentFocus;
            vp.Focus = dragStartPosition - mapDelta;
            Console.WriteLine("Drag : " + vp.Focus);
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
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = new SolidBrush(Color.DarkBlue)
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
            dragRecognizer.Update();

//            renderComponent.ViewPort.Focus += new VirtualMapCoordinate(0.01f, 0);
            
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