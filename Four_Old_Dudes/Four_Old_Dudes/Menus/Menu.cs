using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using Drawable = Four_Old_Dudes.Core.Drawable;

namespace Four_Old_Dudes.Menus
{
    public abstract class Menu : Drawable
    {
        protected List<MenuItem> MenuItems { get; set; }
        protected MenuPointer Pointer { get; set; }
        protected Sound ShiftSound { get; set; }
        protected Sound SelectSound { get; set; }

        protected Menu()
        {
            MenuItems = new List<MenuItem>();
            WinInstance.KeyPressed += OnKeyPressed;
            WinInstance.JoystickButtonPressed += OnJoyStickButtonPressed;
            WinInstance.JoystickMoved += OnJoyStickAxisMoved;
        }

        protected Menu(ref RenderWindow window,List<MenuItem> menuItems, Sound shiftSound, Sound selectSound) : base (ref window)
        {
            MenuItems = menuItems;
            ShiftSound = shiftSound;
            SelectSound = selectSound;
            WinInstance.KeyPressed += OnKeyPressed;
            WinInstance.JoystickButtonPressed += OnJoyStickButtonPressed;
            WinInstance.JoystickMoved += OnJoyStickAxisMoved;
        }

        public void AddMenuItem(MenuItem item) => MenuItems.Add(item);

        public override void Draw() => MenuItems.ForEach(delegate(MenuItem item) { item.Draw(); });

        public void DestroyMenu()
        {
            WinInstance.KeyPressed -= OnKeyPressed;
            WinInstance.JoystickButtonPressed -= OnJoyStickButtonPressed;
            WinInstance.JoystickMoved -= OnJoyStickAxisMoved;
        }
        public abstract void OnKeyPressed(object sender, KeyEventArgs e);
        public abstract void OnJoyStickButtonPressed(object sender, JoystickButtonEventArgs e);
        public abstract void OnJoyStickAxisMoved(object sender, JoystickMoveEventArgs e);
    }
}
