using System;
using Four_Old_Dudes.Players;
using SFML.Graphics;
using SFML.Window;

namespace Four_Old_Dudes.Controller
{
    class Controller
    {
        public  enum XboxOneButtonMappings
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
            RThumbYDir = Joystick.Axis.X

        }
        public bool JoyStickAvailable { get; } = Joystick.IsConnected(0);
        public RenderWindow CurrentGameWindow { get; }
        public Playable User { get; private set; }

        public Controller(ref RenderWindow gameWindow, Playable player)
        {
            CurrentGameWindow = gameWindow;
            User = player;
            if (CurrentGameWindow != null)
                SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            CurrentGameWindow.JoystickConnected += OnJoyStickConnected;
            CurrentGameWindow.KeyPressed += OnKeyPressed;
            CurrentGameWindow.KeyReleased += OnKeyReleased;
            CurrentGameWindow.JoystickButtonPressed += OnButtonPressed;
            CurrentGameWindow.JoystickButtonReleased += OnButtonReleased;
            CurrentGameWindow.JoystickDisconnected += OnJoyStickDisconnected;
            CurrentGameWindow.JoystickMoved += OnJoyStickMoved;

        }

        private void OnJoyStickMoved(object sender, JoystickMoveEventArgs e)
        {
            Console.WriteLine("moved axis");
            Console.WriteLine(e);
        }

        static void OnJoyStickConnected(object sender, JoystickConnectEventArgs e)
        {
            Console.WriteLine("connected");
            Console.WriteLine(e);
        }
        static void OnJoyStickDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("connected");
            Console.WriteLine(e);
        }

        static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            Console.WriteLine("key pressed");
            Console.WriteLine(e);
        }
        static void OnKeyReleased(object sender, KeyEventArgs e)
        {
            Console.WriteLine("key released");
            Console.WriteLine(e);
        }
        static void OnButtonPressed(object sender, JoystickButtonEventArgs e)
        { 
            Console.WriteLine("button pressed");
            Console.WriteLine(e);
        }
        static void OnButtonReleased(object sender, JoystickButtonEventArgs e)
        {
            Console.WriteLine("button released");
            Console.WriteLine(e);
        }
    }
}
