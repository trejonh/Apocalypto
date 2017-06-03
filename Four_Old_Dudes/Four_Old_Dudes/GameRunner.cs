using SFML.Graphics;
using SFML.Window;
using System;
using Four_Old_Dudes.Players;
using SFML.System;
using Tiled.SFML;

namespace Four_Old_Dudes
{
    class GameRunner
    {
        private const int Screenx = 1280, Screeny = 720;
        public static Time Delta;
        static void Main()
        {
            RenderWindow window = new RenderWindow(new VideoMode(Screenx, Screeny), "Four Old Dudes!");
            /*IMenu menu = new MainMenu(null);
            menu.ScreenSizeX = Screenx;
            menu.ScreenSizeY = Screeny;*/
            window.SetActive();
            window.Closed += OnClosed;
            var sprite = new SpriteAnimated(new Texture(@"assets/sprites/player.png"),32,32 ,60,window, RenderStates.Default,0,11);
            var player = new Playable("hero", ref sprite, new SFML.Window.Vector2f(64, Screeny-160)) {Speed = 100f};
            player.AddAnimation(Playable.Direction.Down, 0, 2);
            player.AddAnimation(Playable.Direction.Left, 3, 5);
            player.AddAnimation(Playable.Direction.Right, 6, 8);
            player.AddAnimation(Playable.Direction.Up, 9, 11);
            player.SetDirection(Playable.Direction.Right);
            player.Play();
            player.Stop();
           // new Controller.Controller(ref window,player);
           var map = new Map(@"assets/maps/map.tmx",window.GetView());
            //menu.CreateMenu(ref window);
            var timer = new Clock();
            while (window.IsOpen())
            {
                Delta = timer.Restart();
                //window.Clear(new Color(135,250,255));
                window.Clear();
                window.Draw(map);
                player.Update();
                window.Display();
                window.DispatchEvents();
            }
        }

        private static void OnClosed(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Close();
        }
        
    }
}
