using SFML.Graphics;
namespace Four_Old_Dudes.System
{
    public abstract class Drawable
    {
        protected RenderWindow _winInstance { get; set; }
        public abstract void Draw();
    }
}
