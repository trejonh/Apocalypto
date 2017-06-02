using Four_Old_Dudes.Utils;
using System;
using System.IO;

namespace Four_Old_Dudes
{
    class GameRunner
    {
        private const int Screenx = 1280, Screeny = 720;
        static void Main()
        {
            //_filePath = Directory.GetParent(_filePath).FullName;
            AssetManager.LoadAssets();
            Console.ReadLine();
            /*RenderWindow window = new RenderWindow(new VideoMode(Screenx, Screeny), "Four Old Dudes!");
            MainMenu.InitOptions();
            window.SetActive();
            window.Closed += OnClosed;
           // window.KeyPressed += MainMenu.OnKeyPressed;
            new Controller.Controller(ref window, null);
            var view = new View(new FloatRect(0, 50, 300, 200)); 
            var xml = new Map("map.tmx",view);
            view.Center = new Vector2f(0, Screeny/25);
            view.Size = new Vector2f(300, 300);
            var x = 500;
            var y = 500;
            while (window.IsOpen())
            {
                //smMap.Update(ref window, clock.ElapsedTime.AsSeconds());
               // window.Clear(Color.White);
               // MainMenu.DrawMenuOptions(ref window);
               // smMap.Render(ref window);
                window.Clear(new Color(135,250,255));
                //window.Draw(xml);
                window.Draw(xml);
                window.Display();
                window.DispatchEvents();
            }*/
        }

        /*private static void OnClosed(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Close();
        }
        private class MainMenu
        {
            private static RectangleShape _newGame, _loadGame, _stats, _exit;
            private static Text _newGameText, _loadGameText, _statsText, _exitText;
            private const int X = 300, Y = 100;
            private static Sprite _pointerSprite;
            private static Texture _pointerTexture;
            private static LinkedList<Vector2f> _pointerPositions;
            private static LinkedListNode<Vector2f> _currentNode;

            public static void InitOptions()
            {
                _newGame = new RectangleShape(new Vector2f(X, Y)) {FillColor = new Color(128, 128, 128), Position = new Vector2f((Screenx / 2)-X/2, Screeny - (Y*6) )};
                _newGameText = new Text() {Position = new Vector2f(_newGame.Position.X + 21, _newGame.Position.Y + 10), DisplayedString = "New Game", Color = Color.Black, Font = new Font("orange_juice.ttf"), CharacterSize = 60};
                _loadGame = new RectangleShape(new Vector2f(X, Y)) {FillColor = new Color(128, 128, 128), Position = new Vector2f((Screenx / 2) - X / 2, Screeny - (Y * 4)-40)};
                _loadGameText = new Text() {Position = new Vector2f(_loadGame.Position.X + 14, _loadGame.Position.Y + 10), DisplayedString = "Load Game", Color = Color.Black, Font = new Font("orange_juice.ttf"), CharacterSize = 60 };
                _stats = new RectangleShape(new Vector2f(X, Y)) {FillColor = new Color(128, 128, 128), Position = new Vector2f((Screenx / 2) - X / 2, Screeny - (Y * 2)-80) };
                _statsText = new Text() {Position = new Vector2f(_stats.Position.X + 80, _stats.Position.Y+10), DisplayedString = "Stats", Color = Color.Black, Font = new Font("orange_juice.ttf"), CharacterSize = 60 };
                _exit = new RectangleShape(new Vector2f(X, Y)) {FillColor = new Color(128, 128, 128), Position = new Vector2f((Screenx / 2) - X / 2, Screeny - (Y * 1)-40) };
                _exitText = new Text() {Position = new Vector2f(_exit.Position.X+100, _exit.Position.Y+10), DisplayedString = "Exit", Color = Color.Black, Font = new Font("orange_juice.ttf"), CharacterSize = 60 };
                _pointerTexture = new Texture("pointer.png") {Smooth = true};
                _pointerSprite = new Sprite(_pointerTexture)
                    {
                        Position = new Vector2f((_newGame.Position.X - _pointerTexture.Size.X/2), _newGame.Position.Y),
                        Scale = new Vector2f(0.5f,0.5f)
                    };
                _pointerPositions = new LinkedList<Vector2f>( new[]{_pointerSprite.Position, new Vector2f((_loadGame.Position.X - _pointerTexture.Size.X / 2), _loadGame.Position.Y), new Vector2f((_stats.Position.X - _pointerTexture.Size.X / 2), _stats.Position.Y), new Vector2f((_exit.Position.X - _pointerTexture.Size.X / 2), _exit.Position.Y) });
                _currentNode = _pointerPositions.First;
            }

            public static void DrawMenuOptions(ref RenderWindow window)
            {
                window.Draw(_newGame);
                window.Draw(_newGameText);
                window.Draw(_loadGame);
                window.Draw(_loadGameText);
                window.Draw(_stats);
                window.Draw(_statsText);
                window.Draw(_exit);
                window.Draw(_exitText);
                window.Draw(_pointerSprite);
            }

            public static void OnKeyPressed(object sender, KeyEventArgs e)
            {
                if (e.Code == Keyboard.Key.Up)
                {
                    try
                    {
                        if ((_currentNode = _currentNode.Previous) != null)
                        {
                            _pointerSprite.Position = _currentNode.Value;
                        }

                    }
                    catch (NullReferenceException exception)
                    {
                        _currentNode = _pointerPositions.Last;
                        _pointerSprite.Position = _currentNode.Value;
                        Console.WriteLine(exception);
                    }
                }
                else if (e.Code == Keyboard.Key.Down)
                {
                    try
                    {
                        if ((_currentNode = _currentNode.Next) != null)
                        {
                            _pointerSprite.Position = _currentNode.Value;
                        }
                    }
                    catch (NullReferenceException exception)
                    {
                        _currentNode = _pointerPositions.First;
                        _pointerSprite.Position = _currentNode.Value;
                        Console.WriteLine(exception);
                    }
                }
            }

            public static void OnJoyStickButtonPressed(object sender, JoystickButtonEvent e)
            {
                
            }
        }*/
    }
}
