using System.Collections.Generic;
using SFML.System;

namespace GameOfLife.Logic
{
    /// <summary>
    /// Represents an individual active cell of the simulation
    /// </summary>
    public struct Cell
    {
        /// <summary>
        /// The position of the <see cref="Cell"/>
        /// </summary>
        public Vector2i position;

        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="Cell"/> class
        /// </summary>
        public Cell(int x, int y)
        {
            position = new Vector2i(x, y);
        }

        /// <summary>
        /// Default constructor for the <see cref="Cell"/> class
        /// </summary>
        public Cell(Vector2i position)
        {
            this.position = position;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Get the hash code using the <see cref="position"/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash += position.X * 13;
            hash += position.Y;
            return hash;
        }

        /// <summary>
        /// True if their positions are the same, otherwise false
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Cell))
                return false;

            return ((Cell)obj).position.Equals(position);
        }

        #endregion

        /// <summary>
        /// Get all neighboring coordinates of the specified cell,
        /// regardles of their activation status
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public HashSet<Vector2i> GetNeighbors()
        {
            HashSet<Vector2i> neighbors = new HashSet<Vector2i>();

            int x = position.X;
            int y = position.Y;

            // Add all of the neighboring coordinates
            neighbors.Add(new Vector2i(x - 1, y - 1));
            neighbors.Add(new Vector2i(x - 1, y));
            neighbors.Add(new Vector2i(x - 1, y + 1));
            neighbors.Add(new Vector2i(x, y - 1));
            neighbors.Add(new Vector2i(x, y + 1));
            neighbors.Add(new Vector2i(x + 1, y - 1));
            neighbors.Add(new Vector2i(x + 1, y));
            neighbors.Add(new Vector2i(x + 1, y + 1));

            return neighbors;
        }

        public static implicit operator Cell (Vector2i position) => new Cell(position);
    }
}
