using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace GameOfLife
{
    /// <summary>
    /// Represents a class wrapper that controlls one <see cref="RenderWindow"/>
    /// </summary>
    public class Screen
    {
        #region Window properties

        /// <summary>
        /// The desired target FPS of the application
        /// </summary>
        public float DesiredFPS { get; set; }

        /// <summary>
        /// The width of the <see cref="Window"/>
        /// </summary>
        public uint Width { get; protected set; }

        /// <summary>
        /// The height of the <see cref="Window"/>
        /// </summary>
        public uint Height { get; protected set; }

        /// <summary>
        /// The title of the window
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// The color that will be used to
        /// clear the previous frame of the window
        /// </summary>
        public Color clearColor;

        /// <summary>
        /// The window that will be displayed
        /// </summary>
        public RenderWindow Window { get; protected set; }

        /// <summary>
        /// The components that will be drawn
        /// and updated in their order of
        /// appearence in this <see cref="List{IComponent}"/>
        /// </summary>
        public List<IComponent> components;

        #endregion

        /// <summary>
        /// The mouse controller of this screen
        /// </summary>
        public MouseController Mouse { get; protected set; }

        /// <summary>
        /// The instance of time
        /// </summary>
        public GameTime Time { get; protected set; }

        #region Events

        /// <summary>
        /// Invoked when the update happens
        /// </summary>
        public event Action Tick;

        /// <summary>
        /// Invoke the Tick event
        /// </summary>
        private void OnTick()
        {
            Tick?.Invoke();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor, taking in the size
        /// of the window and its title
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        /// <param name="title">The title</param>
        public Screen(uint width, uint height, string title)
        {
            Time = new GameTime();
            components = new List<IComponent>();

            // Window properties
            Width = width;
            Height = height;
            Title = title;

            // Create the window
            Window = new RenderWindow(new VideoMode(Width, Height), Title);

            // Handle the Closed event
            Window.Closed += (s, e) => (s as RenderWindow).Close();

            // Reset the view on window resize
            Window.Resized += (s, e) => (s as RenderWindow).SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
            
            // Default values
            clearColor = new Color(20, 20, 150);
            DesiredFPS = 60;

            Mouse = new MouseController(this);
        }

        #endregion
        
        /// <summary>
        /// Runs the window and returns
        /// when the window closes
        /// </summary>
        public void Run()
        {
            // Start the time
            Time.Start();
            
            // The loop
            while (Window.IsOpen)
            {
                // Dispatch the events
                Window.DispatchEvents();

                // Let the time know
                Time.Update();
                // Wait for the desired time of our update
                Time.WaitUntil(1 / DesiredFPS);

                // If the time since last update is greater
                // than the desired framerate
                if (Time.DeltaTime >= 1 / DesiredFPS)
                {
                    // Clear the window
                    Window.Clear(clearColor);

                    // Update the screen
                    Update();

                    // And draw 
                    Draw();

                    // Display the window
                    Window.Display();
                }
            }
        }

        /// <summary>
        /// Update the frame
        /// </summary>
        void Update()
        {
            // Invoke the tick / update
            OnTick();
            
            // Update the components in order of appearence
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Update();
            }
        }

        /// <summary>
        /// Draw the frame
        /// </summary>
        void Draw()
        {
            // Draw the components in order of appearence
            for (int i = 0; i < components.Count; i++)
            {
                Window.Draw(components[i]);
            }
        }
    }
}
