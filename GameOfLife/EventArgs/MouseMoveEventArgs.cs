using System;
using SFML.System;

namespace GameOfLife
{
    /// <summary>
    /// The arguments for mouse events
    /// </summary>
    public class MouseMoveEventArgs : EventArgs
    {
        /// <summary>
        /// The old position of mouse relative to
        /// the owner window in pixels
        /// </summary>
        public Vector2i oldPos;

        /// <summary>
        /// The new position of mouse relative to
        /// the owner window in pixels
        /// </summary>
        public Vector2i newPos;

        /// <summary>
        /// The offset (delta) of 
        /// the old and new position of the mouse
        /// </summary>
        public Vector2i Offset => newPos - oldPos;

        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="MouseEventArgs"/> class
        /// </summary>
        public MouseMoveEventArgs(Vector2i oldPos, Vector2i newPos)
        {
            this.oldPos = oldPos;
            this.newPos = newPos;
        }

        #endregion
    }
}
