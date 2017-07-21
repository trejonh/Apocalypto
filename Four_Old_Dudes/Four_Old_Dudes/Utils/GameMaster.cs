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
using SFML.Audio;
using Four_Old_Dudes.Misc;

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
        public static Color ThemeColor { get; } = new Color(148,0,211);
        public Music ThemeMusic { get; }
        public static bool IsThemePlaying { get; private set; }

        /// <summary>
        /// Initialize the game
        /// </summary>
        public GameMaster()
        {
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            ThemeMusic = AssetManager.LoadMusic("ThemeMusic");
            IsMainMenuOpen = true;
            _window = new RenderWindow(new VideoMode(Screenx, Screeny), AssetManager.GetMessage("GameTitle"));
            _window.SetActive(true);
            _window.SetFramerateLimit(60);
            var mainMenu = new MainMenu(ref _window, AssetManager.LoadSound("ShiftThroughMenu"), AssetManager.LoadSound("ShiftThroughMenu"));
            _window.Closed += Window_Closed;
            IsGamePaused = false;
            HasMainBeenDestroyed = false;
            var gameClock = new Clock();
            var showSplash = true;
            var splash = new SplashScreen(ref _window);
            var splashDisplayTime = 0.0f;
            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                Delta = gameClock.Restart();
                if (showSplash)
                {
                    splash.Draw();
                    splashDisplayTime += Delta.AsSeconds();
                    if (splashDisplayTime >= SplashScreen.MaxDisplayTime)
                    {
                        showSplash = false;
                        splash.Stop();
                        ThemeMusic.Play();
                        IsThemePlaying = true;
                    }
                }
                else
                {
                    if (IsMainMenuOpen)
                    {
                        if (ThemeMusic != null && ThemeMusic.Status != SoundStatus.Playing)
                        {
                            ThemeMusic.Play();
                            IsThemePlaying = true;
                        }
                            mainMenu.Draw();
                    }
                    if (IsThemePlaying)
                    {
                        if (ThemeMusic != null && ThemeMusic.Status != SoundStatus.Playing)
                            ThemeMusic.Play();
                    }
                    else
                    {
                        if (ThemeMusic != null && ThemeMusic.Status == SoundStatus.Playing)
                            ThemeMusic.Stop();
                    }

                    if (IsGamePaused == false)
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
                }
                _window.Display();
            }
        }

        /// <summary>
        /// Create the pause menu
        /// </summary>
        private void CreatePauseMenu()
        {
            _pauseMenu = new PauseMenu(ref _window, AssetManager.LoadSound("ShiftThroughMenu"), AssetManager.LoadSound("ShiftThroughMenu"));
        }

        /// <summary>
        /// Create a new game enviroment
        /// </summary>
        /// <param name="playerName"></param>
        public static void NewGame(string playerName)
        {
            if (GameWorld == null)
                GameWorld = new World(ref _window);
            GameWorld.NewGame(playerName);
        }

        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Window_Closed(object sender, System.EventArgs e)
        {
            LogManager.CloseLog();
            _window.Close();
        }

        /// <summary>
        /// Unpause the game
        /// </summary>
        public static void Unpause()
        {
            GameWorld.UnpauseWorld();
            IsGamePaused = false;
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        public static void Pause()
        {
            GameWorld.Pause();
            IsGamePaused = true;
        }

        /// <summary>
        /// Load a game from a save file
        /// </summary>
        /// <param name="world">The world element</param>
        /// <param name="player">The player element</param>
        /// <param name="enemies">The enemies last on the map</param>
        /// <param name="items">The items last on the map</param>
        public static void LoadGame(XElement world,XElement player, XElement enemies, XElement items)
        {
            var worldAttr = world.Attributes().ToDictionary(attr => attr.Name, attr => attr.Value);
            var currentMap = int.Parse(worldAttr["currentMap"]);
            var score = long.Parse(worldAttr["score"]);
            var lives = int.Parse(worldAttr["lives"]);
            var ene = enemies.Descendants("enemy").ToArray();
            var item = items.Descendants("item").ToArray();
            if(GameWorld == null)
                GameWorld = new World(ref _window);
            GameWorld.LoadGame(currentMap, score, lives, player, ene,item);
        }

        /// <summary>
        /// Play the theme music
        /// </summary>
        public static void PlayTheme()
        {
            if (IsThemePlaying == false)
                IsThemePlaying = true;
        }

        /// <summary>
        /// Stop the theme music
        /// </summary>
        public static void StopTheme()
        {
            if (IsThemePlaying == true)
                IsThemePlaying = false;
        }
    }
}
