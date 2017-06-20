using Four_Old_Dudes.Utils;
using Four_Old_Dudes.MovingSprites;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using Four_Old_Dudes.Maps;

namespace Four_Old_Dudes
{
    class GameRunner
    {
        private const int Screenx = 1080, Screeny = 720;
        private static  RenderWindow _window;
        public static Time Delta;
        static void Main()
        {
            //_filePath = Directory.GetParent(_filePath).FullName;
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            _window = new RenderWindow(new VideoMode(Screenx, Screeny), AssetManager.GetMessage("GameTitle"));
            _window.SetActive(true);
            _window.SetFramerateLimit(60);
            //var menu = new MainMenu(ref window, AssetManager.LoadSound("ShiftThroughMenu"), AssetManager.LoadSound("ShiftThroughMenu"));
            _window.Closed += Window_Closed;
            //player.SetPosition(new Vector2f(200,400));
            var world = new World(ref _window, "DefaultMap","TestPlayer", 0, 11);
            world.AddPlayerAnimation(Moveable.Direction.Down, 0, 2);
            world.AddPlayerAnimation(Moveable.Direction.Left, 3, 5);
            world.AddPlayerAnimation(Moveable.Direction.Right, 6, 8);
            world.AddPlayerAnimation(Moveable.Direction.Up, 9, 11);
            var clock = new Clock();
            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                _window.Clear();
                Delta = clock.Restart();
                world.Draw();
                _window.Display();
            }
        }

        private static void Window_Closed(object sender, System.EventArgs e)
        {
            LogManager.CloseLog();
            _window.Close();
        }
    }
}
