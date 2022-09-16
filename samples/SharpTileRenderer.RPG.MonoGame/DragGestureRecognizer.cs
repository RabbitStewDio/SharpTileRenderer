using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpTileRenderer.Util;
using System;

namespace SharpTileRenderer.RPG.MonoGame
{
    public class DragGestureRecognizer
    {
        enum DragState { None, AwaitMove, Moving }

        [Flags]
        public enum MouseButton { None, Left = 1, Right = 2, Middle = 4, X1 = 8, X2 = 16 }

        public event EventHandler? MouseMoving;
        public event EventHandler<DragEvent>? DragStarted;
        public event EventHandler<DragEvent>? Dragging;
        public event EventHandler<DragEvent>? DragFinished;
        public event EventHandler<DragEvent>? DragAborted;

        public class DragEvent : EventArgs
        {
            public Point DragStartPosition { get; }
            public MouseButton MouseButton { get; }
            public bool DragConfirmed { get; set; }

            public DragEvent(Point dragStartPosition, MouseButton mouseButton)
            {
                DragStartPosition = dragStartPosition;
                MouseButton = mouseButton;
            }
        }

        DragState state;
        MouseButton dragButton;
        Point dragStartPosition;
        Point dragStartPositionHold;
        Optional<Point> lastMousePosition;

        public DragGestureRecognizer()
        {
            DragThreshold = 4;
            HoldMouseWhenDragging = false;
        }

        public void ConfirmDrag(object? o, DragEvent evt)
        {
            evt.DragConfirmed = true;
        }

        public int DragThreshold { get; set; }
        public bool HoldMouseWhenDragging { get; set; }

        public void AbortDrag()
        {
            if (state == DragState.None) return;
            state = DragState.None;
            DragAborted?.Invoke(this, new DragEvent(dragStartPosition, dragButton));
        }

        MouseButton GetButtonState(in MouseState ms)
        {
            var retval = MouseButton.None;
            retval |= (ms.LeftButton == ButtonState.Pressed) ? MouseButton.Left : MouseButton.None;
            retval |= (ms.RightButton == ButtonState.Pressed) ? MouseButton.Right : MouseButton.None;
            retval |= (ms.MiddleButton == ButtonState.Pressed) ? MouseButton.Middle : MouseButton.None;
            retval |= (ms.XButton1 == ButtonState.Pressed) ? MouseButton.X1 : MouseButton.None;
            retval |= (ms.XButton2 == ButtonState.Pressed) ? MouseButton.X2 : MouseButton.None;
            return retval;
        }

        public void Update()
        {
            var ms = Mouse.GetState();
            if (lastMousePosition.TryGetValue(out var lastPos))
            {
                if (lastPos != ms.Position)
                {
                    MouseMoving?.Invoke(this, EventArgs.Empty);
                }
            }

            lastMousePosition = ms.Position;

            switch (state)
            {
                case DragState.None:
                {
                    var bs = GetButtonState(ms);
                    if (bs != MouseButton.None)
                    {
                        state = DragState.AwaitMove;
                        dragStartPosition = ms.Position;
                        dragStartPositionHold = ms.Position;
                        dragButton = bs;
                    }

                    break;
                }
                case DragState.AwaitMove:
                {
                    var bs = GetButtonState(ms);
                    if (bs == MouseButton.None)
                    {
                        state = DragState.None;
                        break;
                    }

                    var delta = ms.Position.ToVector2() - dragStartPosition.ToVector2();
                    var distance = delta.Length();
                    if (distance > DragThreshold)
                    {
                        var eventArgs = new DragEvent(dragStartPosition, dragButton);
                        DragStarted?.Invoke(this, eventArgs);
                        if (!eventArgs.DragConfirmed)
                        {
                            state = DragState.None;
                            break;
                        }

                        state = DragState.Moving;
                        Dragging?.Invoke(this, eventArgs);
                    }

                    if (HoldMouseWhenDragging)
                    {
                        Mouse.SetPosition(dragStartPositionHold.X, dragStartPositionHold.Y);
                        dragStartPosition = new Point((int)(dragStartPosition.X - delta.X), (int)(dragStartPosition.Y - delta.Y));
                    }

                    break;
                }
                case DragState.Moving:
                {
                    var bs = GetButtonState(ms);
                    if (bs == MouseButton.None)
                    {
                        DragFinished?.Invoke(this, new DragEvent(dragStartPosition, dragButton));
                        state = DragState.None;
                    }
                    else
                    {
                        Dragging?.Invoke(this, new DragEvent(dragStartPosition, dragButton));
                    }

                    if (HoldMouseWhenDragging)
                    {
                        var delta = ms.Position.ToVector2() - dragStartPosition.ToVector2();
                        Mouse.SetPosition(dragStartPositionHold.X, dragStartPositionHold.Y);
                        dragStartPosition = new Point((int)(dragStartPosition.X - delta.X), (int)(dragStartPosition.Y - delta.Y));
                    }

                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }
        }
    }
}