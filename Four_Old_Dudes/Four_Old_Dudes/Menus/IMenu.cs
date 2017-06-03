using OpenTK;
using SFML.Graphics;
using SFML.Window;

namespace Four_Old_Dudes.Menus
{
    public interface IMenu
    {
        string Name { get; set; }
        int ScreenSizeX { get; set; }
        int ScreenSizeY { get; set; }
        void DrawMenu(ref RenderWindow window);
        void CreateMenu(ref RenderWindow window);
        void DestroyMenu(ref RenderWindow window);
        void OnKeyPressed(object sender, KeyEventArgs e);
        void OnJoyStickButtonPressed(object sender, JoystickButtonEventArgs e);
        void OnJoyStickAxisMoved(object sender, JoystickMoveEventArgs e);

    }
}