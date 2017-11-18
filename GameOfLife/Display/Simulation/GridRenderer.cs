using System;
using GameOfLife.Logic;
using SFML.Graphics;
using SFML.System;

namespace GameOfLife
{
    /// <summary>
    /// Renders all the cells of a grid
    /// </summary>
    public class GridRenderer : Transformable, IComponent
    {
        /// <summary>
        /// The grid to render
        /// </summary>
        Grid grid;

        /// <summary>
        /// The display size of one cell
        /// </summary>
        public float unitSize;

        /// <summary>
        /// The color of an active cell
        /// </summary>
        public Color activeColor;

        /// <summary>
        /// The color of an inactive cell
        /// </summary>
        public Color inactiveColor;

        public bool isDragging;

        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="GridRenderer"/> class
        /// </summary>
        public GridRenderer(Grid grid)
        {
            this.grid = grid;

            // Default values
            unitSize = 10;
            activeColor = Color.Green;
            inactiveColor = new Color(100, 100, 100);
        }

        #endregion

        /// <summary>
        /// Get the four vertices representing a square at the index x and y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private Vertex[] GetVertices(Cell cell)
        {
            Vertex[] vertices = new Vertex[4];

            float x = cell.position.X;
            float y = cell.position.Y;

            vertices[0] = new Vertex(new Vector2f(x, y) * unitSize, activeColor);
            vertices[1] = new Vertex(new Vector2f(x + 1, y) * unitSize, activeColor);
            vertices[2] = new Vertex(new Vector2f(x + 1, y + 1) * unitSize, activeColor);
            vertices[3] = new Vertex(new Vector2f(x, y + 1) * unitSize, activeColor);

            return vertices;
        }
        
        /// <summary>
        /// Draw all of the cells that have been created
        /// </summary>
        /// <param name="target"></param>
        /// <param name="states"></param>
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            
            // TODO: Draw the empty cells first

            VertexArray activeCells = new VertexArray(PrimitiveType.Quads, (uint)grid.Cells.Count * 4);
            
            foreach (Cell cell in grid.Cells)
            {
                IntRect windowViewPort = target.GetViewport(target.GetView());
                IntRect range = ViewCellRange(target.DefaultView, windowViewPort.Width, windowViewPort.Height);

                // If the cell isn't in the view, don't draw it
                if (!range.Contains(cell.position.X, cell.position.Y))
                    continue;
                
                foreach (Vertex vert in GetVertices(cell))
                {
                    activeCells.Append(vert);
                }
            }

            target.Draw(activeCells, states);
        }

        private IntRect ViewCellRange(View simulationView, int width, int height)
        {
            FloatRect range = simulationView.Viewport;

            // Set the window range
            range.Height *= height;
            range.Width *= width;
            range.Top *= height;
            range.Left *= width;

            // Account for the position
            range.Top -= Position.Y;
            range.Left -= Position.X;

            // Account for the scale
            range.Top /= Scale.Y * unitSize;
            range.Left /= Scale.X * unitSize;
            range.Width /= Scale.X * unitSize;
            range.Height /= Scale.Y * unitSize;

            return new IntRect((int)range.Left - 1, (int)range.Top - 1, (int)range.Width + 1, (int)range.Height + 1);
        }

        public void Update()
        {
        }
    }
}