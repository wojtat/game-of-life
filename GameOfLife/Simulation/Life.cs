using System;
using System.IO;
using SFML.Graphics;
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
            grid = new Grid();
            screen = new Screen(640, 400, "Game of Life");
            screen.Time.iterationsPerSecond = 1;

            screen.Mouse.StartDrag += (s, e) =>
            {
                if (e.button == Mouse.Button.Left)
                    grid.isBeingDragged = true;

            };
            screen.Mouse.EndDrag += (s, e) =>
            {
                if (e.button == Mouse.Button.Left)
                    grid.isBeingDragged = false;
            };
            screen.Mouse.Moved += (s, e) =>
            {
                if (grid.isBeingDragged)
                {
                    grid.offset.X -= e.Offset.X;
                    grid.offset.Y -= e.Offset.Y;
                }
            };
            screen.Mouse.Clicked += (s, e) =>
            {
                if (e.button == Mouse.Button.Right)
                {
                    Vector2f pos = new Vector2f(e.mousePos.X, e.mousePos.Y);
                    pos += grid.offset;
                    pos /= grid.RealUnitSize;
                    grid.AddCell((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y));
                }
            };

            screen.Window.MouseWheelMoved += (s, e) =>
            {
                grid.zoomFactor *= (float)Math.Pow(grid.zoomMultiplier, -e.Delta);

                Vector2f offset = new Vector2f(e.X - screen.Width / 2, e.Y - screen.Height / 2);

                grid.offset += (e.Delta < 0) ? -offset : offset;
            };
            // TEMPORARY
            screen.Window.KeyPressed += (s, e) =>
            {
                if (e.Code == Keyboard.Key.Left)
                    screen.Time.iterationsPerSecond *= 0.8f;
                if (e.Code == Keyboard.Key.Right)
                    screen.Time.iterationsPerSecond *= 1.25f;
            };

            // <TEST>
            Grid glider = Grid.Load(@"glider.txt");
            //Grid eater = Grid.Load("eater.txt");

            grid.AddCell(0, 1);
            grid.AddCell(0, 0);
            grid.AddCell(1, 0);
            grid.AddCell(1, 1);

            //grid.Merge(glider);
            //grid.Merge(eater, new Vector2i(6, 6));
            // </TEST>

            // Hook the Iterate() method to the iteration event
            screen.Time.Iteration += grid.Iterate;

            // Increase the generation number
            screen.Time.Iteration += () =>
            {
                Generation++;

                // Add a glider to the scene
                if (Generation % 14 == 0)
                {
                    //grid.Merge(glider);
                    //grid.Merge(glider, new Vector2i(40, 0));
                }
            };

            screen.components.Add(grid);
        }

        #endregion

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
