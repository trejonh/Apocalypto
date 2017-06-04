using System;
using SFML.Graphics;
namespace Four_Old_Dudes.Menus
{
    /// <summary>
    /// Representaive of an on screen item for a menu
    /// </summary>
    public class MenuItem : Core.Drawable
    {
        protected Text ItemText { get; set; }
        protected Shape ItemShape { get; set; }
        private readonly Func<bool> _action;

        /// <summary>
        /// Default constructor setting all properties to null
        /// </summary>
        public MenuItem()
        {
            ItemShape = null;
            ItemText = null;
            _action = null;
        }

        /// <summary>
        /// Create a new menu item with a refernce to the window to draw to
        /// </summary>
        /// <param name="window">Reference to the window in which to draw to</param>
        public MenuItem(ref RenderWindow window) : base(ref window)
        {
            ItemShape = null;
            ItemText = null;
        }

        /// <summary>
        /// Create a new menu item with no action to invoke when selected
        /// </summary>
        /// <param name="window">Reference to the window to draw to</param>
        /// <param name="text">The text of the item</param>
        /// <param name="shape">The shape to draw</param>
        public MenuItem(ref RenderWindow window, Text text, Shape shape) : base(ref window)
        {
            ItemShape = shape;
            ItemText = text;
            _action = null;
        }

        /// <summary>
        /// Create a new menu item with an action to invoke when selected
        /// </summary>
        /// <param name="window">Reference to the window to draw to</param>
        /// <param name="text">The text of the item</param>
        /// <param name="shape">The shape to draw</param>
        /// <param name="action">The function to call when selected</param>
        public MenuItem(ref RenderWindow window, Text text, Shape shape, Func<bool> action) : base(ref window)
        {
            ItemShape = shape;
            ItemText = text;
            _action = action;
        }

        /// <summary>
        /// Draw the menu item to the screen
        /// </summary>
        public override void Draw()
        {
            if(ItemShape == null || ItemText == null)
                return;
            WinInstance.Draw(ItemShape);
            WinInstance.Draw(ItemText);
        }

        /// <summary>
        /// Invoke the action
        /// </summary>
        /// <returns>Result of action if not null</returns>
        public bool DoAction()
        {
            return _action != null && _action();
        }
    }
}
