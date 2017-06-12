using Four_Old_Dudes.Utils;
using Four_Old_Dudes.MovingSprites;
using SFML.Graphics;
using SFML.Window;

namespace Four_Old_Dudes
{
    class GameRunner
    {
        private const int Screenx = 1080, Screeny = 720;
        private static  RenderWindow _window;
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
            var world = GameWorld.GameWorld.GetInstance();
            var map = AssetManager.LoadGameMap("DefaultMap", _window.GetView());
            var player = new Player(AssetManager.LoadTexture("TestPlayer"), 32, 32, 60, _window, RenderStates.Default, 0, 11);
            player.AddAnimation(Moveable.Direction.Down, 0, 2);
            player.AddAnimation(Moveable.Direction.Left, 3, 5);
            player.AddAnimation(Moveable.Direction.Right, 6, 8);
            player.AddAnimation(Moveable.Direction.Up, 9, 11);
            player.SetDirection(Moveable.Direction.Right);
            if(map != null)
                player.SetPosition(map.PlayerInitialPosition);
            //player.SetPosition(new Vector2f(200,400));
            while (_window.IsOpen)
            {
                _window.Clear();
                if(map!=null)
                    _window.Draw(map);
                world.Step(1.0f/60.0f,6,2);
                player.Update();
                _window.Display();
                _window.DispatchEvents();
            }
        }

        private static void Window_Closed(object sender, System.EventArgs e)
        {
            LogManager.CloseLog();
            _window.Close();
        }
    }
}
