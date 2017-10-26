using System;
using SFML.System;
using SFML.Window;

namespace GameOfLife
{
    /// <summary>
    /// The arguments for mouse events
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        /// <summary>
        /// The position of mouse relative to owner window in pixels
        /// </summary>
        public Vector2i mousePos;

        /// <summary>
        /// The mouse button, which trigered the event
        /// </summary>
        public Mouse.Button button;

        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="MouseEventArgs"/> class
        /// </summary>
        public MouseEventArgs(Vector2i mousePos, Mouse.Button button)
        {
            this.mousePos = mousePos;
            this.button = button;
        }

        #endregion
    }
}
