using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Four_Old_Dudes.Menus
{
    class MainMenu:IMenu
    {

        private RectangleShape _newGame, _loadGame, _stats, _exit;
        private Text _newGameText, _loadGameText, _statsText, _exitText;
        private static Sprite _pointerSprite;
        private static Texture _pointerTexture;
        private static LinkedList<Vector2f> _pointerPositions;
        private static LinkedListNode<Vector2f> _currentNode;
        private const int ButtonX = 300, ButtonY = 100;
        public string Name { get; set; }

        public int ScreenSizeX { get; set;}

        public int ScreenSizeY { get; set; }

        public MainMenu(string menuName)
        {
            Name = string.IsNullOrEmpty(menuName) ? "Main Menu" : menuName;
            ScreenSizeX = 0;
            ScreenSizeY = 0;
        }
        public MainMenu(string menuName, int screeny, int screenx)
        {
            Name = string.IsNullOrEmpty(menuName) ? "Main Menu" : menuName;
            ScreenSizeX = screenx;
            ScreenSizeY = screeny;
        }
        public void DrawMenu(ref RenderWindow window)
        {
            window.Draw(_newGame);
            window.Draw(_newGameText);
            window.Draw(_loadGame);
            window.Draw(_loadGameText);
            window.Draw(_pointerSprite);
            window.Draw(_stats);
            window.Draw(_statsText);
            window.Draw(_exit);
            window.Draw(_exitText);
        }

        public void CreateMenu(ref RenderWindow window)
        {
            _newGame = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = new Color(128, 128, 128), Position = new Vector2f((ScreenSizeX / 2) - ButtonX / 2, ScreenSizeY - (ButtonY * 6)) };
            _newGameText = new Text() { Position = new Vector2f(_newGame.Position.X + 21, _newGame.Position.Y + 10), DisplayedString = "New Game", Color = Color.Black, Font = new Font("orange_juice.ttf"), CharacterSize = 60 };
            _loadGame = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = new Color(128, 128, 128), Position = new Vector2f((ScreenSizeX / 2) - ButtonX / 2, ScreenSizeY - (ButtonY * 4) - 40) };
            _loadGameText = new Text() { Position = new Vector2f(_loadGame.Position.X + 14, _loadGame.Position.Y + 10), DisplayedString = "Load Game", Color = Color.Black, Font = new Font(@"assets/fonts/orange_juice.ttf"), CharacterSize = 60 };
            _stats = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = new Color(128, 128, 128), Position = new Vector2f((ScreenSizeX / 2) - ButtonX / 2, ScreenSizeY - (ButtonY * 2) - 80) };
            _statsText = new Text() { Position = new Vector2f(_stats.Position.X + 80, _stats.Position.Y + 10), DisplayedString = "Stats", Color = Color.Black, Font = new Font(@"assets/fonts/orange_juice.ttf"), CharacterSize = 60 };
            _exit = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = new Color(128, 128, 128), Position = new Vector2f((ScreenSizeX / 2) - ButtonX / 2, ScreenSizeY - (ButtonY * 1) - 40) };
            _exitText = new Text() { Position = new Vector2f(_exit.Position.X + 100, _exit.Position.Y + 10), DisplayedString = "Exit", Color = Color.Black, Font = new Font(@"assets/fonts/orange_juice.ttf"), CharacterSize = 60 };
            _pointerTexture = new Texture(@"assets/misc/pointer.png") { Smooth = true };
            _pointerSprite = new Sprite(_pointerTexture)
            {
                Position = new Vector2f((_newGame.Position.X - _pointerTexture.Size.X / 2f), _newGame.Position.Y),
                Scale = new Vector2f(0.5f, 0.5f)
            };
            _pointerPositions = new LinkedList<Vector2f>(new[] { _pointerSprite.Position, new Vector2f((_loadGame.Position.X - _pointerTexture.Size.X / 2f), _loadGame.Position.Y), new Vector2f((_stats.Position.X - _pointerTexture.Size.X / 2f), _stats.Position.Y), new Vector2f((_exit.Position.X - _pointerTexture.Size.X / 2f), _exit.Position.Y) });
            _currentNode = _pointerPositions.First;
            window.KeyPressed += OnKeyPressed;
            window.JoystickMoved += OnJoyStickAxisMoved;
            window.JoystickButtonPressed += OnJoyStickButtonPressed;
        }

        public void DestroyMenu(ref RenderWindow window)
        {
            window.KeyPressed -= OnKeyPressed;
            window.JoystickMoved -= OnJoyStickAxisMoved;
            window.JoystickButtonPressed -= OnJoyStickButtonPressed;
        }

        public void OnKeyPressed(object sender, KeyEventArgs e)
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

        public void OnJoyStickButtonPressed(object sender, JoystickButtonEventArgs e)
        {

        }

        public void OnJoyStickAxisMoved(object sender, JoystickMoveEventArgs e)
        {
            if ( e.Axis != (Joystick.Axis) Controller.Controller.XboxOneDirection.DPadYDir &&
                e.Axis != (Joystick.Axis) Controller.Controller.XboxOneDirection.LThumbYDir)
                return;
                if (e.Position>5) { 
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
            else if (e.Position<-1)
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
    }
}
