using System.Collections.Generic;
using SFML.System;

namespace GameOfLife.Logic
{
    /// <summary>
    /// Holds all of the cells
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// All of the cells that are active
        /// </summary>
        HashSet<Cell> cells;

        /// <summary>
        /// All of the cells that are active
        /// </summary>
        public HashSet<Cell> Cells => cells;

        /// <summary>
        /// The generation number of this grid
        /// </summary>
        public int Generation { get; protected set; } = 1;
        
        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="Grid"/> class
        /// </summary>
        public Grid()
        {
            cells = new HashSet<Cell>();
        }

        #endregion

        #region Public Methods

        #region Cells Manipulation

        /// <summary>
        /// Add a cell to the current cells
        /// if it is not already added
        /// </summary>
        /// <param name="position"></param>
        public void Add(Vector2i position)
        {
            if (cells.Contains(position))
                return;

            cells.Add(position);
        }

        /// <summary>
        /// Add a cell to the current cells
        /// if it is not already added
        /// </summary>
        /// <param name="position"></param>
        public void Add(int x, int y) => Add(new Vector2i(x, y));

        /// <summary>
        /// Add all the cells
        /// if they are not already added
        /// </summary>
        /// <param name="positions"></param>
        public void Add(IEnumerable<Vector2i> positions)
        {
            foreach (Vector2i pos in positions)
                Add(pos);
        }

        /// <summary>
        /// Remove a cell from the current cells
        /// if it is there
        /// </summary>
        /// <param name="position"></param>
        public void Remove(Vector2i position)
        {
            if (!cells.Contains(position))
                return;

            cells.Remove(position);
        }

        /// <summary>
        /// Remove a cell from the current cells
        /// if it is there
        /// </summary>
        /// <param name="position"></param>
        public void Remove(int x, int y) => Remove(new Vector2i(x, y));

        /// <summary>
        /// Remove specified cells from the current cells
        /// if they are there
        /// </summary>
        /// <param name="positions"></param>
        public void Remove(IEnumerable<Vector2i> positions)
        {
            foreach (Vector2i pos in positions)
                Remove(pos);
        }

        /// <summary>
        /// Empty the grid
        /// </summary>
        public void Clear() => cells = new HashSet<Cell>();

        #endregion

        /// <summary>
        /// Execute one iteration of the simulation
        /// </summary>
        public void Iterate()
        {
            var buffer = new HashSet<Cell>();
            var visited = new HashSet<Vector2i>();

            // Loop through each cell
            foreach (Cell cell in cells)
            {
                Vector2i current = cell.position;

                // Get all the current cell's neighbors
                var neighbors = cell.GetNeighbors();

                // Loop through all the cell's neighbors
                foreach (Vector2i neighbor in neighbors)
                {
                    // It's an active neighbor, therefore we will check it 
                    // later or sooner because it's in cells
                    if (cells.Contains(neighbor)) continue;

                    // If we've visited this neighbor, continue
                    if (visited.Contains(neighbor)) continue;

                    // Mark this neighbor as visited
                    visited.Add(neighbor);

                    // If it should be active, add it to the buffer
                    if (ShouldBeActive(neighbor))
                        buffer.Add(neighbor);
                }

                // Add us to the visited cells if we aren't there already
                if (!visited.Contains(current))
                    visited.Add(current);
                // If we are, continue to the next cell
                else
                    continue;

                if (ShouldBeActive(current))
                    // We made it to the next generation
                    buffer.Add(cell);
            }

            // Set the cells
            cells = buffer;

            Generation++;
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Get the number of active neighboring cells
        /// </summary>
        /// <param name="cell"></param>
        int NumberOfNeighbors(Cell cell)
        {
            int count = 0;

            // Loop through all neighbors of this cell
            foreach (Vector2i neighbor in cell.GetNeighbors())
                if (cells.Contains(neighbor))
                    count++;

            return count;
        }

        /// <summary>
        /// Determine whether a certain cell should be
        /// active in the next generation or not
        /// </summary>
        /// <param name="cell"></param>
        bool ShouldBeActive(Cell cell)
        {
            int neighbors = NumberOfNeighbors(cell);
            
            if (cells.Contains(cell))
                return (neighbors == 3 || neighbors == 2);
            else
                return (neighbors == 3);
        }

        #endregion
    }
}
