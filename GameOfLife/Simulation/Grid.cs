using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.System;

namespace GameOfLife
{
    /// <summary>
    /// Holds all of the cells of this simulation
    /// </summary>
    public class Grid : IComponent
    {
        /// <summary>
        /// Determines how much the zoom should change
        /// after mouse wheel has moved
        /// </summary>
        public float zoomMultiplier;

        /// <summary>
        /// Indicates if the user is dragging the grid
        /// </summary>
        public bool isBeingDragged;

        /// <summary>
        /// The color used to draw the active cells
        /// </summary>
        public Color activeColor;
        
        /// <summary>
        /// The base size of one cell
        /// </summary>
        float unitSize;

        /// <summary>
        /// The size of one cell with the zoom factor accounted for
        /// </summary>
        public float RealUnitSize => unitSize / zoomFactor;

        /// <summary>
        /// The multiplier determining the zoom
        /// </summary>
        public float zoomFactor = 1;

        /// <summary>
        /// The offset of the grid used for scrolling
        /// </summary>
        public Vector2f offset;

        /// <summary>
        /// The position and size of the grid view
        /// </summary>
        public FloatRect rectangle;

        /// <summary>
        /// All of the cells that are active
        /// </summary>
        Dictionary<Vector2i, Cell> cells;

        #region Cell

        /// <summary>
        /// The living or dead cell
        /// </summary>
        class Cell
        {
            /// <summary>
            /// The parent grid of this cell that created it
            /// </summary>
            private Grid parent;

            /// <summary>
            /// The position of the cell
            /// </summary>
            public Vector2i position;

            /// <summary>
            /// Determines if the cell is living or not
            /// </summary>
            public bool isActive;

            /// <summary>
            /// The list of all the neighbors this cell has
            /// </summary>
            List<Cell> neighbors;

            /// <summary>
            /// The list of all the neighbors this cell has
            /// </summary>
            public List<Cell> Neighbors { get { CountNeighbors(); return neighbors; } }

            #region Constructor

            public Cell(int x, int y, Grid parent, bool isActive = true)
            {
                position = new Vector2i(x, y);

                this.isActive = isActive;
                this.parent = parent;

                neighbors = new List<Cell>(8);
            }

            #endregion

            /// <summary>
            /// Counts the number of neighboring cells that are active / alive
            /// </summary>
            /// <returns>The number of active neighbors</returns>
            public int CountNeighbors()
            {
                neighbors = new List<Cell>(8);

                int activeNeighbors = 0;

                // The pos of the checked square
                Vector2i pos;

                // Loop through all the neighbors of this cell
                for (int xOff = -1; xOff <= 1; xOff++)
                {
                    // Set the x coord of the checked square
                    pos.X = position.X + xOff;

                    for (int yOff = -1; yOff <= 1; yOff++)
                    {
                        // Set the y coord of the checked square
                        pos.Y = position.Y + yOff;

                        // If it's us, continue
                        if (pos == position)
                            continue;
                        
                        // If we have an active neighbor there
                        if (parent.cells.ContainsKey(pos) && parent.cells[pos].isActive)
                        {
                            // Increment the counter
                            activeNeighbors++;

                            // Add the cell to the neighbors list variable
                            neighbors.Add(parent.cells[pos]);
                        }
                        else
                        {
                            // Add an inactive cell to the neighbors
                            neighbors.Add(new Cell(pos.X, pos.Y, parent, false));
                        }
                    }
                }

                return activeNeighbors;
            }

            /// <summary>
            /// Determines if this cell should be active
            /// in the next iteration considering its neighbors
            /// </summary>
            public bool ShouldBeActive()
            {
                // Number of active neighbors
                int count = CountNeighbors();

                // Apply the classic Game of Life rules
                if (isActive)
                {
                    if (count < 2 || count > 3)
                        return false;
                }
                else
                {
                    if (count == 3)
                        return true;
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Get the four vertices representing a square at the index x and y
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public Vertex[] cellVertices()
            {
                Vertex[] vertices = new Vertex[4];

                float x = position.X;
                float y = position.Y;

                vertices[0] = new Vertex(new Vector2f(x, y) * parent.RealUnitSize, parent.activeColor);
                vertices[1] = new Vertex(new Vector2f(x + 1, y) * parent.RealUnitSize, parent.activeColor);
                vertices[2] = new Vertex(new Vector2f(x + 1, y + 1) * parent.RealUnitSize, parent.activeColor);
                vertices[3] = new Vertex(new Vector2f(x, y + 1) * parent.RealUnitSize, parent.activeColor);

                return vertices;
            }

            #region Overrides

            public static bool operator ==(Cell a, Cell b) => /*a == null ? false : */a.Equals(b);
            public static bool operator !=(Cell a, Cell b) => !a.Equals(b);
            public override bool Equals(object obj)
            {
                if (!(obj is Cell)) return false;
                Cell other = (Cell)obj;

                return this.position == other.position;
            }
            public override int GetHashCode()
            {
                return position.GetHashCode();
            }

            public override string ToString()
            {
                return position.ToString();
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// Inactivate a cell at the index x and y,
        /// does remove the cell from cells
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RemoveCell(int x, int y)
        {
            cells.Remove(new Vector2i(x, y));
        }

        /// <summary>
        /// Add a cell at the index x and y to cells
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddCell(int x, int y)
        {
            // Don't add if it is already there
            if (cells.ContainsKey(new Vector2i(x, y)))
                return;

            cells.Add(new Vector2i(x, y), new Cell(x, y, this));
        }

        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="Grid"/> class
        /// </summary>
        public Grid()
        {
            cells = new Dictionary<Vector2i, Cell>();

            // Default values
            unitSize = 10;
            zoomMultiplier = 1.2f;
            activeColor = Color.Green;
        }

        /// <summary>
        /// Constructor with specified size and position
        /// of the grid's view
        /// </summary>
        /// <param name="rect"></param>
        public Grid(FloatRect rect) : this()
        {
            rectangle = rect;
        }

        #endregion

        /// <summary>
        /// Execute one iteration of the simulation
        /// </summary>
        public void Iterate()
        {
            Grid buffer = new Grid();

            // List of cells that we've visited
            List<Cell> visitedCells = new List<Cell>();

            // Loop through each cell
            foreach (KeyValuePair<Vector2i, Cell> pair in cells)
            {
                // Get the current cell
                Cell current = pair.Value;

                // Get the current cell's neighbors
                List<Cell> neighbors = current.Neighbors;

                // Loop through all the cell's neighbors
                foreach (Cell neighbor in neighbors)
                {
                    // If it's an active neighbor, it is in cells too,
                    // therefore we will check it later or sooner
                    if (neighbor.isActive) continue;

                    // If we've visited this neighbor, continue
                    if (visitedCells.Contains(neighbor)) continue;

                    // Mark this neighbor as visited
                    visitedCells.Add(neighbor);

                    // If it should be active, add it to the buffer
                    if (neighbor.ShouldBeActive())
                        buffer.AddCell(neighbor.position.X, neighbor.position.Y);
                }

                // Add us to the visited cells if we aren't there already
                if (!visitedCells.Contains(current))
                    visitedCells.Add(current);
                // If we are, continue to the next cell
                else
                    continue;
                
                if (current.ShouldBeActive())
                    // We made it to the next generation
                    buffer.AddCell(current.position.X, current.position.Y);
            }
            
            // Recreate the cells
            cells = new Dictionary<Vector2i, Cell>();

            foreach (var pair in buffer.cells)
            {
                cells.Add(pair.Key, new Cell(pair.Key.X, pair.Key.Y, this));
            }
        }

        /// <summary>
        /// Draw all of the cells that have been created
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void Draw(RenderTarget target, RenderStates states)
        {
            // The size of the render target
            Vector2f size = new Vector2f(target.Size.X, target.Size.Y);

            View view = null;

            if (rectangle != default(FloatRect))
            {
                // Create the view 
                view = new View(rectangle);

                // Set the view's viewport
                view.Viewport = new FloatRect(rectangle.Left / size.X,
                                              rectangle.Top / size.Y,
                                              rectangle.Width / size.X,
                                              rectangle.Height / size.Y
                                             );
            }
            else
            {
                view = new View(new FloatRect(new Vector2f(0, 0), size));
            }
            // Move the grid according to the offset
            view.Move(offset);

            // Change the render target's view
            target.SetView(view);

            VertexArray vertices = new VertexArray(PrimitiveType.Quads, (uint)cells.Count * 4);
            
            foreach (KeyValuePair<Vector2i, Cell> pair in cells)
            {
                if (!pair.Value.isActive)
                    continue;

                // Calculate the vertices
                Vertex[] quad = pair.Value.cellVertices();

                // Add the vertices
                for (int i = 0; i < 4; i++)
                {
                    vertices.Append(quad[i]);
                }
            }

            target.Draw(vertices, states);

            target.SetView(target.DefaultView);
        }

        public void Update()
        {
        }

        /// <summary>
        /// Takes all values from the specified grid
        /// and copies them to this grid
        /// </summary>
        /// <param name="other"></param>
        public void Merge(Grid other)
        {
            Dictionary<Vector2i, Cell> copy = new Dictionary<Vector2i, Cell>(other.cells);

            foreach (KeyValuePair<Vector2i, Cell> pair in copy)
            {
                AddCell(pair.Key.X, pair.Key.Y);
            }
        }

        /// <summary>
        /// Takes all values from the specified grid
        /// and copies them to this grid with the specified offset
        /// </summary>
        /// <param name="other"></param>
        public void Merge(Grid other, Vector2i offset)
        {
            Dictionary<Vector2i, Cell> copy = new Dictionary<Vector2i, Cell>(other.cells);

            foreach (KeyValuePair<Vector2i, Cell> pair in copy)
            {
                AddCell(pair.Key.X + offset.X, pair.Key.Y + offset.Y);
            }
        }

        /// <summary>
        /// Loads a new grid from the specified text file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Grid Load(string path)
        {
            Grid g = new Grid();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] pos = line.Split(new char[] { ',' });
                    g.AddCell(Convert.ToInt32(pos[0]), Convert.ToInt32(pos[1]));
                }
            }

            return g;
        }
    }
}