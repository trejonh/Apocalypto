using System;
using SFML.Graphics;
namespace Four_Old_Dudes.Core
{
    /// <summary>
    /// Represent a drawable object
    /// </summary>
    public abstract class Drawable
    {
        /// <summary>
        /// The window to draw to
        /// </summary>
        protected RenderWindow WinInstance { get; set; }
        /// <summary>
        /// Draw to window
        /// </summary>
        public abstract void Draw();
        /// <summary>
        /// Construct new drawable
        /// </summary>
        protected Drawable()
        {
            WinInstance = null;
        }
        /// <summary>
        /// Construct new drawable
        /// </summary>
        /// <param name="window">Reference to window to draw to</param>
        protected Drawable(ref RenderWindow window)
        {
            WinInstance = window;
        }
    }
}
