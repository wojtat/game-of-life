using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Window;

namespace GameOfLife
{
    /// <summary>
    /// Represents a user's mouse and provides
    /// useful information about it
    /// </summary>
    public class MouseController
    {
        /// <summary>
        /// The screen this mouse controller is related to
        /// </summary>
        Screen parent;

        /// <summary>
        /// A dictionary of all mouse buttons 
        /// with bool values determining 
        /// if a certain button is being dragged
        /// </summary>
        Dictionary<Mouse.Button, bool> isDragging;

        /// <summary>
        /// The position of the mouse cursor 
        /// relative to the window on the last frame
        /// </summary>
        Vector2i lastPosition;

        #region Public Properties

        /// <summary>
        /// Determines if the mouse cursor
        /// is hovering directly over the window
        /// </summary>
        public bool IsOverWindow { get; protected set; }

        /// <summary>
        /// The current position of the mouse cursor
        /// relative to the parent screen
        /// </summary>
        public Vector2i Position { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="MouseController"/> class
        /// </summary>
        public MouseController(Screen parent)
        {
            this.parent = parent;

            isDragging = new Dictionary<Mouse.Button, bool>();

            // Hook the update mrrthod to the screen's tick
            parent.Tick += Update;

            // Listen out for mouse left and entered events
            parent.Window.MouseEntered += (s, e) => IsOverWindow = true;
            parent.Window.MouseLeft += (s, e) => IsOverWindow = false;

            // Listen out for other mouse events
            parent.Window.MouseButtonPressed += (s, e) =>
            {
                if (!IsOverWindow)
                    return;

                // Invoke the Pressed event
                OnPressed(e.Button);
            };
            parent.Window.MouseButtonReleased += (s, e) =>
            {
                // Check if the released button has also been dragged
                if (IsDragging(e.Button))
                {
                    OnEndDrag(e.Button);
                }
                else
                {
                    OnClicked(e.Button);
                }

                if (!IsOverWindow)
                    return;

                // Invoke the Released event
                OnReleased(e.Button);
            };
            parent.Window.MouseMoved += (s, e) =>
            {
                // Get the buttons as an array
                var values = (Mouse.Button[])Enum.GetValues(typeof(Mouse.Button));

                // Check for any pressed buttons...
                foreach (var button in values)
                {
                    if (button == Mouse.Button.ButtonCount) continue;

                    if (IsPressed(button) && !IsDragging(button))
                    {
                        // and invoke StartDrag
                        OnStartDrag(button);
                    }
                }

                if (!IsOverWindow)
                    return;

                // Invoke the Moved event
                OnMoved();
            };
        }

        #endregion

        /// <summary>
        /// Called when the global update happens
        /// </summary>
        private void Update()
        {
            lastPosition = Position;

            Position = Mouse.GetPosition(parent.Window);
        }

        #region Events

        /// <summary>
        /// Invoked when any mouse button is pressed
        /// </summary>
        public event EventHandler<MouseEventArgs> Pressed;

        /// <summary>
        /// Invoked when any mouse button is released
        /// </summary>
        public event EventHandler<MouseEventArgs> Released;

        /// <summary>
        /// Invoked when the user starts dragging 
        /// the mouse with any mouse button
        /// </summary>
        public event EventHandler<MouseEventArgs> StartDrag;

        /// <summary>
        /// Invoked when the user stops dragging 
        /// the mouse with any mouse button
        /// </summary>
        public event EventHandler<MouseEventArgs> EndDrag;

        /// <summary>
        /// Invoked when the mouse has moved
        /// </summary>
        public event EventHandler<MouseMoveEventArgs> Moved;

        /// <summary>
        /// Invoked when the mouse is pressed 
        /// and then released at the same
        /// position without it moving
        /// </summary>
        public event EventHandler<MouseEventArgs> Clicked;

        /// <summary>
        /// Invoke the Pressed event
        /// </summary>
        private void OnPressed(Mouse.Button button)
        {
            Pressed?.Invoke(parent.Window, new MouseEventArgs(Position, button));
        }

        /// <summary>
        /// Invoke the Released event
        /// </summary>
        private void OnReleased(Mouse.Button button)
        {
            Released?.Invoke(parent.Window, new MouseEventArgs(Position, button));
        }

        /// <summary>
        /// Invoke the StartDrag event
        /// </summary>
        private void OnStartDrag(Mouse.Button button)
        {
            isDragging[button] = true;
            StartDrag?.Invoke(parent.Window, new MouseEventArgs(Position, button));
        }

        /// <summary>
        /// Invoke the EndDrag event
        /// </summary>
        private void OnEndDrag(Mouse.Button button)
        {
            isDragging[button] = false;
            EndDrag?.Invoke(parent.Window, new MouseEventArgs(Position, button));
        }

        /// <summary>
        /// Invoke the Moved event
        /// </summary>
        private void OnMoved()
        {
            Moved?.Invoke(parent.Window, new MouseMoveEventArgs(lastPosition, Position));
        }

        /// <summary>
        /// Invoke the Clicked event
        /// </summary>
        private void OnClicked(Mouse.Button button)
        {
            Clicked?.Invoke(parent.Window, new MouseEventArgs(Position, button));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determine whether or not the specified mouse button is pressed
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsPressed(Mouse.Button button) => Mouse.IsButtonPressed(button);

        /// <summary>
        /// Determines whether the user is using
        /// the specified button to drag
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsDragging(Mouse.Button button)
        {
            if (isDragging.ContainsKey(button))
                return isDragging[button];
            return false;
        }

        #endregion
    }
}
