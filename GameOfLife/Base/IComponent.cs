using SFML.Graphics;

namespace GameOfLife
{
    /// <summary>
    /// Represents a class that contains
    /// a draw and an update method
    /// </summary>
    public interface IComponent : Drawable
    {
        void Update();
    }
}
