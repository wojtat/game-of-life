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
        HashSet<Vector2i> cells;
        
        /// <summary>
        /// Add a cell at the index x and y to cells
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddCell(int x, int y)
        {
            AddCell(new Vector2i(x, y));
        }

        /// <summary>
        /// Add a cell at the index x and y to cells
        /// </summary>
        public void AddCell(Vector2i coords)
        {
            // Don't add if it is already there
            if (cells.Contains(coords))
                return;

            cells.Add(coords);
        }

        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="Grid"/> class
        /// </summary>
        public Grid()
        {
            cells = new HashSet<Vector2i>();

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
        /// Get the four vertices representing a square at the index x and y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private Vertex[] GetVertices(Vector2i cell)
        {
            Vertex[] vertices = new Vertex[4];

            float x = cell.X;
            float y = cell.Y;

            vertices[0] = new Vertex(new Vector2f(x, y) * RealUnitSize, activeColor);
            vertices[1] = new Vertex(new Vector2f(x + 1, y) * RealUnitSize, activeColor);
            vertices[2] = new Vertex(new Vector2f(x + 1, y + 1) * RealUnitSize, activeColor);
            vertices[3] = new Vertex(new Vector2f(x, y + 1) * RealUnitSize, activeColor);

            return vertices;
        }

        /// <summary>
        /// Get all neighboring coordinates of the specified cell,
        /// regardles of their activation status
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private HashSet<Vector2i> GetNeighbors(Vector2i cell)
        {
            HashSet<Vector2i> neighbors = new HashSet<Vector2i>();

            int x = cell.X;
            int y = cell.Y;

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

        /// <summary>
        /// Get the number of active neighboring cells
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private int NumberOfNeighbors(Vector2i cell)
        {
            int count = 0;

            // Loop through all neighbors of this cell
            foreach (Vector2i neighbor in GetNeighbors(cell))
            {
                if (cells.Contains(neighbor))
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Determine whether a certain cell should be
        /// active in the next generation or not
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool ShouldBeActive(Vector2i cell)
        {
            int neighbors = NumberOfNeighbors(cell);
            
            if (cells.Contains(cell))
            {
                if (neighbors == 3 || neighbors == 2)
                    return true;
            }
            else
            {
                if (neighbors == 3)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Execute one iteration of the simulation
        /// </summary>
        public void Iterate()
        {
            Grid buffer = new Grid();

            // List of cells that we've visited
            HashSet<Vector2i> visitedCells = new HashSet<Vector2i>();

            // Loop through each cell
            foreach (Vector2i current in cells)
            {
                // Get the current cell's all neighbors
                HashSet<Vector2i> neighbors = GetNeighbors(current);

                // Loop through all the cell's neighbors
                foreach (Vector2i neighbor in neighbors)
                {
                    // It's an active neighbor, therefore we will check it 
                    // later or sooner because it's in cells
                    if (cells.Contains(neighbor)) continue;

                    // If we've visited this neighbor, continue
                    if (visitedCells.Contains(neighbor)) continue;

                    // Mark this neighbor as visited
                    visitedCells.Add(neighbor);

                    // If it should be active, add it to the buffer
                    if (ShouldBeActive(neighbor))
                        buffer.AddCell(neighbor);
                }

                // Add us to the visited cells if we aren't there already
                if (!visitedCells.Contains(current))
                    visitedCells.Add(current);
                // If we are, continue to the next cell
                else
                    continue;
                
                if (ShouldBeActive(current))
                    // We made it to the next generation
                    buffer.AddCell(current);
            }
            
            // Recreate the cells
            cells = new HashSet<Vector2i>(buffer.cells);
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
            
            foreach (Vector2i cell in cells)
            {
                // Calculate the vertices
                Vertex[] quad = GetVertices(cell);

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
            var copy = new HashSet<Vector2i>(other.cells);

            foreach (Vector2i coords in copy)
            {
                AddCell(coords.X, coords.Y);
            }
        }

        /// <summary>
        /// Takes all values from the specified grid
        /// and copies them to this grid with the specified offset
        /// </summary>
        /// <param name="other"></param>
        public void Merge(Grid other, Vector2i offset)
        {
            var copy = new HashSet<Vector2i>(other.cells);

            foreach (Vector2i coords in copy)
            {
                AddCell(coords.X + offset.X, coords.Y + offset.Y);
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