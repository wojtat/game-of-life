using System;
using GameOfLife.Logic;
using SFML.System;
using SFML.Window;

namespace GameOfLife
{
    /// <summary>
    /// The Game of Life
    /// </summary>
    public class Life
    {
        /// <summary>
        /// The current generation of the simulation
        /// </summary>
        public int Generation { get; protected set; }

        /// <summary>
        /// The grid renderer to draw the <see cref="grid"/> with all the cells
        /// </summary>
        public GridRenderer gridRenderer;

        /// <summary>
        /// The grid with all the cells
        /// </summary>
        public Grid grid;

        /// <summary>
        /// The screen to draw to
        /// </summary>
        public Screen screen;

        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="Life"/> class
        /// </summary>
        public Life()
        {
            gridRenderer = new GridRenderer(grid = new Grid());
            screen = new Screen(640, 400, "Game of Life");
            screen.Time.iterationsPerSecond = 1;
            
            screen.Mouse.StartDrag += (s, e) =>
            {
                if (e.button == Mouse.Button.Left)
                    gridRenderer.isDragging = true;

            };
            screen.Mouse.EndDrag += (s, e) =>
            {
                if (e.button == Mouse.Button.Left)
                    gridRenderer.isDragging = false;
            };
            screen.Mouse.Moved += (s, e) =>
            {
                if (gridRenderer.isDragging)
                {
                    Vector2f newPos = new Vector2f(gridRenderer.Position.X + e.Offset.X, gridRenderer.Position.Y + e.Offset.Y);

                    gridRenderer.Position = newPos;
                }
            };
            screen.Mouse.Clicked += (s, e) =>
            {
                if (e.button == Mouse.Button.Right)
                {
                    Vector2f pos = new Vector2f(e.mousePos.X, e.mousePos.Y);
                    pos -= gridRenderer.Position;
                    pos = new Vector2f(pos.X / gridRenderer.Scale.X, pos.Y / gridRenderer.Scale.Y);
                    pos /= gridRenderer.unitSize;
                    grid.Add((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y));
                }
            };
            screen.Window.MouseWheelMoved += (s, e) =>
            {
                if (e.Delta > 0)
                    for (int i = 0; i < e.Delta; i++)
                        gridRenderer.Scale *= 1.2f;
                else
                    for (int i = 0; i < -e.Delta; i++)
                        gridRenderer.Scale /= 1.2f;
            };

            grid.Add(0, 0);
            grid.Add(0, 1);
            grid.Add(1, 1);
            grid.Add(1, -1);
            
            screen.Window.KeyPressed += (s, e) =>
            {
                if (e.Code == Keyboard.Key.Left)
                    screen.Time.iterationsPerSecond *= 0.8f;
                if (e.Code == Keyboard.Key.Right)
                    screen.Time.iterationsPerSecond *= 1.25f;
            };

            // Hook the Iterate() method to the iteration event
            screen.Time.Iteration += grid.Iterate;

            // Increase the generation number
            screen.Time.Iteration += () => Generation++;

            screen.components.Add(gridRenderer);
        }

        #endregion

        /// <summary>
        /// Start the simulation
        /// </summary>
        public void Start()
        {
            screen.Run();

            // Possibly save the grid here or do something
        }

        #region Main

        /// <summary>
        /// The entry point of the application
        /// </summary>
        /// <param name="args">Command prompt arguments</param>
        static void Main(string[] args)
        {
            Life l = new Life();
            l.Start();
        }

        #endregion
    }
}
