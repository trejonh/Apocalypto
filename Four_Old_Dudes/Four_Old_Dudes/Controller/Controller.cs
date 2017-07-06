using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Four_Old_Dudes.Controller
{
    public static class Controller
    {
        public enum XboxOneButtonMappings : int
        {
            A = 0,
            B = 1,
            Y = 3,
            X = 2,
            Lb = 4,
            Rb = 5,
            Start = 7,
            Select = 6,
            Lthumb = 8,
            Rthumb = 9
        }

        public enum XboxOneDirection
        {
            DPadXDir = Joystick.Axis.PovY,
            DPadYDir = Joystick.Axis.PovX,
            Triggers = Joystick.Axis.Z,
            LThumbXDir = Joystick.Axis.Y,
            LThumbYDir = Joystick.Axis.X

        }
        public static RenderWindow CurrentGameWindow { get; set; }
        
        public static void SetupEventListeners()
        {
            CurrentGameWindow.JoystickConnected += OnJoyStickConnected;
            CurrentGameWindow.KeyPressed += OnKeyPressed;
            CurrentGameWindow.KeyReleased += OnKeyReleased;
            CurrentGameWindow.JoystickButtonPressed += OnButtonPressed;
            CurrentGameWindow.JoystickButtonReleased += OnButtonReleased;
            CurrentGameWindow.JoystickDisconnected += OnJoyStickDisconnected;
            CurrentGameWindow.JoystickMoved += OnJoyStickMoved;

        }

        public static void OnJoyStickMoved(object sender, JoystickMoveEventArgs e)
        {
            Console.WriteLine("moved axis");
            Console.WriteLine(e);
        }

        public static void OnJoyStickConnected(object sender, JoystickConnectEventArgs e)
        {
            Console.WriteLine("connected");
            Console.WriteLine(e);
        }
        public static void OnJoyStickDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("connected");
            Console.WriteLine(e);
        }

        public static void OnKeyPressed(object sender, KeyEventArgs e)
        {

        }
        public static void OnKeyReleased(object sender, KeyEventArgs e)
        {
        }
        public static void OnButtonPressed(object sender, JoystickButtonEventArgs e)
        {
            Console.WriteLine("button pressed");
            Console.WriteLine(e);
        }
        public static void OnButtonReleased(object sender, JoystickButtonEventArgs e)
        {
            Console.WriteLine("button released");
            Console.WriteLine(e);
        }
    }
}