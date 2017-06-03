using System;
using SFML.Graphics;
using  Four_Old_Dudes.Core;
namespace Four_Old_Dudes.Menus
{
    public class MenuItem : Core.Drawable
    {
        protected Text ItemText { get; set; }
        protected Shape ItemShape { get; set; }
        private readonly Func<bool> _action;

        public MenuItem()
        {
            ItemShape = null;
            ItemText = null;
            _action = null;
        }

        public MenuItem(ref RenderWindow window) : base(ref window)
        {
            ItemShape = null;
            ItemText = null;
        }

        public MenuItem(ref RenderWindow window, Text text, Shape shape) : base(ref window)
        {
            ItemShape = shape;
            ItemText = text;
            _action = null;
        }
        public MenuItem(ref RenderWindow window, Text text, Shape shape, Func<bool> action) : base(ref window)
        {
            ItemShape = shape;
            ItemText = text;
            _action = action;
        }

        public override void Draw()
        {
            if(ItemShape == null || ItemText == null)
                return;
            WinInstance.Draw(ItemShape);
            WinInstance.Draw(ItemText);
        }

        public bool DoAction()
        {
            return _action != null && _action();
        }
    }
}
