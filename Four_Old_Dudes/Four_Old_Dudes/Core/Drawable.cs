using SFML.Graphics;
namespace Four_Old_Dudes.Core
{
    public abstract class Drawable
    {
        protected RenderWindow WinInstance { get; set; }
        public abstract void Draw();

        protected Drawable()
        {
            WinInstance = null;
        }

        protected Drawable(ref RenderWindow window)
        {
            WinInstance = window;
        }
    }
}
