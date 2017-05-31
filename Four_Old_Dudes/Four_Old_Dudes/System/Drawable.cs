using SFML.Window;
namespace Four_Old_Dudes.System
{
    public abstract class Drawable
    {
        protected Window _winInstance { get; set; }
        public abstract void Draw();
    }
}
