using Four_Old_Dudes.Maps;
using Four_Old_Dudes.Menus;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static Four_Old_Dudes.MovingSprites.Moveable;
using static Four_Old_Dudes.MovingSprites.Animation;
using System.Collections.Generic;
using System.Xml.Linq;
using System;
using System.Linq;

namespace Four_Old_Dudes.Utils
{
    public class GameMaster
    {
        private const int Screenx = 1080, Screeny = 720;
        private static RenderWindow _window;
        public static Time Delta;
        public static bool IsMainMenuOpen { get; set; }
        public static World GameWorld { get; set; }
        public static bool IsGamePaused { get; set; }
        private bool IsPauseMenuCreated { get; }
        private bool HasMainBeenDestroyed { get; }
        private PauseMenu _pauseMenu;

        public GameMaster()
        {
            //_filePath = Directory.GetParent(_filePath).FullName;
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            IsMainMenuOpen = true;
            _window = new RenderWindow(new VideoMode(Screenx, Screeny), AssetManager.GetMessage("GameTitle"));
            _window.SetActive(true);
            _window.SetFramerateLimit(60);
            var mainMenu = new MainMenu(ref _window, AssetManager.LoadSound("ShiftThroughMenu"), AssetManager.LoadSound("ShiftThroughMenu"));
            _window.Closed += Window_Closed;
            IsGamePaused = false;
            HasMainBeenDestroyed = false;
            var gameClock = new Clock();
            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                Delta = gameClock.Restart();
                if (IsMainMenuOpen)
                    mainMenu.Draw();
                else if(IsGamePaused == false)
                {
                    GameWorld?.Draw();
                }
                else if (IsGamePaused && IsPauseMenuCreated)
                {
                    Pause();
                    _pauseMenu.AddMenuSelectionAction();
                    var gameWorld = GameWorld;
                    _pauseMenu.SetWorld(ref gameWorld);
                    _pauseMenu.Draw();
                }
                else if (IsGamePaused && IsPauseMenuCreated == false)
                {
                    CreatePauseMenu();
                    IsPauseMenuCreated = true;
                }
                if (IsMainMenuOpen == false && HasMainBeenDestroyed == false)
                {
                    mainMenu.DestroyMenu();
                    HasMainBeenDestroyed = true;
                }
                _window.Display();
            }
        }

        private void CreatePauseMenu()
        {
            _pauseMenu = new PauseMenu(ref _window, AssetManager.LoadSound("ShiftThroughMenu"), AssetManager.LoadSound("ShiftThroughMenu"));
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

        public static void Unpause()
        {
            GameWorld.UnpauseWorld();
            IsGamePaused = false;
        }

        public static void Pause()
        {
            GameWorld.Pause();
            IsGamePaused = true;
        }

        public static void LoadGame(XElement world,XElement player, XElement enemies)
        {
            var currentMap = int.Parse(world.FirstAttribute.Value);
            var ene = enemies.Descendants("enemy").ToArray();
            if(GameWorld == null)
                GameWorld = new World(ref _window);
            GameWorld.LoadGame(currentMap, player, ene);
        }
    }
}
