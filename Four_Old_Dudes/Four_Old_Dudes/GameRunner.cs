﻿using Four_Old_Dudes.Utils;
using Four_Old_Dudes.Menus;
using SFML.Graphics;
using SFML.Window;

namespace Four_Old_Dudes
{
    class GameRunner
    {
        private const int Screenx = 1080, Screeny = 720;
        static void Main()
        {
            //_filePath = Directory.GetParent(_filePath).FullName;
            LogManager.InitLogFile();
            AssetManager.LoadAssets();
            var window = new RenderWindow(new VideoMode(Screenx, Screeny), AssetManager.GetMessage("GameTitle"));
            window.SetActive(true);
            var menu = new MainMenu(ref window, AssetManager.LoadSound("ShiftThroughMenu"), AssetManager.LoadSound("ShiftThroughMenu"));
            while (window.IsOpen)
            {
                window.Clear();
                menu.Draw();
                window.Display();
                window.DispatchEvents();
            }
        }
    }
}
