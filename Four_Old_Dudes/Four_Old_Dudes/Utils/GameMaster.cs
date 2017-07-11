using Four_Old_Dudes.Maps;
using Four_Old_Dudes.Menus;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static Four_Old_Dudes.MovingSprites.Moveable;
using static Four_Old_Dudes.MovingSprites.Animation;
using System.Collections.Generic;

namespace Four_Old_Dudes.Utils
{
    public class GameMaster
    {
        private const int Screenx = 1080, Screeny = 720;
        private static RenderWindow _window;
        public static Time Delta;
        private readonly Clock _gameClock;
        public static bool IsMainMenuOpen { get; set; }
        private readonly MainMenu _mainMenu;
        public static World GameWorld { get; set; }

        public GameMaster()
        {
            //_filePath = Directory.GetParent(_filePath).FullName;
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            IsMainMenuOpen = true;
            _window = new RenderWindow(new VideoMode(Screenx, Screeny), AssetManager.GetMessage("GameTitle"));
            _window.SetActive(true);
            _window.SetFramerateLimit(60);
            _mainMenu = new MainMenu(ref _window, AssetManager.LoadSound("ShiftThroughMenu"), AssetManager.LoadSound("ShiftThroughMenu"));
            _window.Closed += Window_Closed;
            /*var world = new World(ref _window, "DefaultMap","TestPlayer", 0, 11);
            world.AddPlayerAnimation(Moveable.Direction.Down, 0, 2);
            world.AddPlayerAnimation(Moveable.Direction.Left, 3, 5);
            world.AddPlayerAnimation(Moveable.Direction.Right, 6, 8);
            world.AddPlayerAnimation(Moveable.Direction.Up, 9, 11);*/
            _gameClock = new Clock();
            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                Delta = _gameClock.Restart();
                if (IsMainMenuOpen)
                    _mainMenu.Draw();
                else
                {
                    GameWorld?.Draw();
                }
                _window.Display();
            }
        }

        public static void NewGame(string playerName, int firstFrame, int lastFrame, Dictionary<Direction, AnimationFrames> frames)
        {
            if (GameWorld == null)
                GameWorld = new World(ref _window);
            GameWorld.NewGame(playerName, firstFrame, lastFrame, frames);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            LogManager.CloseLog();
            _window.Close();
        }
    }
}
