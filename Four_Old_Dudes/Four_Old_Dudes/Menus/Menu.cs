using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using Drawable = Four_Old_Dudes.Core.Drawable;

namespace Four_Old_Dudes.Menus
{
    /// <summary>
    /// An abstract class to represent a menu
    /// </summary>
    public abstract class Menu : Drawable
    {
        /// <summary>
        /// List of menu items to draw
        /// </summary>
        protected List<MenuItem> MenuItems { get; set; }
        /// <summary>
        /// The pointer for the menu
        /// </summary>
        protected MenuPointer Pointer { get; set; }
        /// <summary>
        /// Sound to play when shifting through menu options
        /// </summary>
        protected Sound ShiftSound { get; set; }
        /// <summary>
        /// Sound to play when option is selected
        /// </summary>
        protected Sound SelectSound { get; set; }

        /// <summary>
        /// Construct new menu mapping KeyPressed, JoystickMoved, and JoystickButtonPressed to functions
        /// </summary>
        protected Menu()
        {
            MenuItems = new List<MenuItem>();
            if (WinInstance == null)
                return;
            WinInstance.KeyPressed += OnKeyPressed;
            WinInstance.JoystickButtonPressed += OnJoyStickButtonPressed;
            WinInstance.JoystickMoved += OnJoyStickAxisMoved;
        }

        /// <summary>
        /// Construct new menu mapping KeyPressed, JoystickMoved, and JoystickButtonPressed to functions
        /// </summary>
        /// <param name="window">Reference to window to draw to and map functions to</param>
        /// <param name="menuItems">Menu items to draw to window</param>
        /// <param name="shiftSound">Sound to play when shifting through menu options</param>
        /// <param name="selectSound">Sound to play when slecting a menu option</param>
        protected Menu(ref RenderWindow window,List<MenuItem> menuItems, Sound shiftSound, Sound selectSound) : base (ref window)
        {
            MenuItems = menuItems;
            ShiftSound = shiftSound;
            SelectSound = selectSound;
            WinInstance.KeyPressed += OnKeyPressed;
            WinInstance.JoystickButtonPressed += OnJoyStickButtonPressed;
            WinInstance.JoystickMoved += OnJoyStickAxisMoved;
        }

        public void AddMenuSelectionAction()
        {
            WinInstance.KeyPressed += OnKeyPressed;
            WinInstance.JoystickButtonPressed += OnJoyStickButtonPressed;
            WinInstance.JoystickMoved += OnJoyStickAxisMoved;
        }

        /// <summary>
        /// Add menu item to exisiting list
        /// </summary>
        /// <param name="item">MenuItem to add</param>
        public void AddMenuItem(MenuItem item) => MenuItems.Add(item);

        /// <summary>
        /// Draw menu items to window
        /// </summary>
        public override void Draw() => MenuItems.ForEach(delegate(MenuItem item) { item.Draw(); });

        /// <summary>
        /// Remove function call backs from window events
        /// </summary>
        public void DestroyMenu()
        {
            WinInstance.KeyPressed -= OnKeyPressed;
            WinInstance.JoystickButtonPressed -= OnJoyStickButtonPressed;
            WinInstance.JoystickMoved -= OnJoyStickAxisMoved;
        }

        /// <summary>
        /// Event call back when key is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void OnKeyPressed(object sender, KeyEventArgs e);

        /// <summary>
        /// Event call back when joystick button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void OnJoyStickButtonPressed(object sender, JoystickButtonEventArgs e);

        /// <summary>
        /// Event call back when joystick axis is moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void OnJoyStickAxisMoved(object sender, JoystickMoveEventArgs e);
    }
}
