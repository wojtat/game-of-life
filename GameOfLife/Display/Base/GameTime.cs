using System;
using SFML.System;

namespace GameOfLife
{
    /// <summary>
    /// Represents the time that this application uses
    /// </summary>
    public class GameTime
    {
        /// <summary>
        /// The clock used by this class
        /// </summary>
        Clock clock;

        /// <summary>
        /// The number of steps per second of the simulation
        /// </summary>
        public float iterationsPerSecond;

        /// <summary>
        /// Gets the total elapsed time 
        /// from the creation of this instance
        /// </summary>
        public Time ElapsedTime => clock.ElapsedTime;

        /// <summary>
        /// Gets the elapsed time from the instantiation
        /// of this time instance in seconds
        /// </summary>
        public float ElapsedSeconds => ElapsedTime.AsSeconds();

        /// <summary>
        /// The time in seconds from the last update
        /// </summary>
        public float DeltaTime => ElapsedSeconds - lastUpdateTime;

        /// <summary>
        /// The absolute time of the last
        /// draw call in seconds
        /// </summary>
        float lastUpdateTime;

        /// <summary>
        /// The absolute time from the last
        /// iteration in seconds
        /// </summary>
        float lastIterateTime;

        #region Constructor

        /// <summary>
        /// Default constructor for the <see cref="GameTime"/> class
        /// </summary>
        public GameTime()
        {
            clock = new Clock();

            lastUpdateTime = 0;
            lastIterateTime = 0;

            // Default values
            iterationsPerSecond = 5;
        }

        #endregion

        /// <summary>
        /// Starts off the time from the beginning
        /// </summary>
        public void Start()
        {
            lastIterateTime = (lastUpdateTime = 0);
            clock.Restart();
        }

        /// <summary>
        /// Is invoked periodically according to the
        /// <see cref="iterationsPerSecond"/> variable
        /// </summary>
        public event Action Iteration;

        /// <summary>
        /// Lets the time know that a window cycle has started
        /// </summary>
        public void Update()
        {
            // Set the last update time
            lastUpdateTime = ElapsedSeconds;

            if (ElapsedSeconds - lastIterateTime > 1 / iterationsPerSecond)
            {
                // Invoke the event
                lastIterateTime = ElapsedSeconds;
                Iteration();
            }
        }

        /// <summary>
        /// Waits until <see cref="DeltaTime"/> value is 
        /// equal or greater than the specified value, then returns
        /// </summary>
        /// <param name="deltaTime">The value of delta to wait until</param>
        public void WaitUntil(float deltaTime)
        {
            while (DeltaTime < deltaTime) { }
        }
    }
}
