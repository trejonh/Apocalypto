using System;
using System.Collections.Generic;
using Four_Old_Dudes.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Four_Old_Dudes.Menus
{
    public class MainMenu : Menu
    {
        private LinkedList<Vector2f> _pointerPositions;
        private LinkedListNode<Vector2f> _currentNode;
        private const int ButtonX = 300, ButtonY = 100;
        public string MenuTitle { get; set; }

        public MainMenu
            (ref RenderWindow window, Sound shiftSound, Sound selectSound) : base(ref window,
            new List<MenuItem>(), shiftSound, selectSound)
        {
            MenuTitle = AssetManager.GetMessage("DefaultMenuTitle");
            SetupMenu();
        }

        private void SetupMenu()
        {
            uint screenSizeX = WinInstance.Size.X, screenSizeY = WinInstance.Size.Y;
            var font = AssetManager.LoadFont("OrangeJuice");
            var newGame = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = new Color(128, 128, 128), Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 6)) };
            var newGameText = new Text() { Position = new Vector2f(newGame.Position.X + 21, newGame.Position.Y + 10), DisplayedString = AssetManager.GetMessage("NewGame"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var loadGame = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = new Color(128, 128, 128), Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 4) - 40) };
            var loadGameText = new Text() { Position = new Vector2f(loadGame.Position.X + 14, loadGame.Position.Y + 10), DisplayedString = AssetManager.GetMessage("LoadGame"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var stats = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = new Color(128, 128, 128), Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 2) - 80) };
            var statsText = new Text() { Position = new Vector2f(stats.Position.X + 80, stats.Position.Y + 10), DisplayedString = AssetManager.GetMessage("Stats"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var exit = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = new Color(128, 128, 128), Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 1) - 40) };
            var exitText = new Text() { Position = new Vector2f(exit.Position.X + 100, exit.Position.Y + 10), DisplayedString = AssetManager.GetMessage("Exit"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var pointerTexture = AssetManager.LoadTexture("OldTimeyPointer");
            pointerTexture.Smooth = true;
            var renderWindow = WinInstance;
            Pointer = new MenuPointer(ref renderWindow,pointerTexture);
            MenuItems.Add(new MenuItem(ref renderWindow, newGameText, newGame));
            MenuItems.Add(new MenuItem(ref renderWindow, loadGameText, loadGame));
            MenuItems.Add(new MenuItem(ref renderWindow, statsText, stats));
            MenuItems.Add(new MenuItem(ref renderWindow, exitText, exit));
            Pointer.SetPosition(new Vector2f((newGame.Position.X - pointerTexture.Size.X / 2f), newGame.Position.Y));
            Pointer.SetScale(new Vector2f(0.5f, 0.5f));
            _pointerPositions = new LinkedList<Vector2f>(new[] { Pointer.GetPosition().Value, new Vector2f((loadGame.Position.X - pointerTexture.Size.X / 2f), loadGame.Position.Y), new Vector2f((stats.Position.X - pointerTexture.Size.X / 2f), stats.Position.Y), new Vector2f((exit.Position.X - pointerTexture.Size.X / 2f), exit.Position.Y) });
        }

        public override void Draw()
        {
            base.Draw();
            Pointer.Draw();
        }

        public override void OnJoyStickAxisMoved(object sender, JoystickMoveEventArgs e)
        {
            Vector2f? pointerPoition;
            if ((pointerPoition = Pointer.GetPosition()) == null)
            {
                // TODO: Log this error
                return;
            }
            if (e.Axis != (Joystick.Axis)Controller.Controller.XboxOneDirection.DPadYDir &&
                e.Axis != (Joystick.Axis)Controller.Controller.XboxOneDirection.LThumbYDir)
                return;
            if (e.Position > 5)
            {
                try
                {
                    if ((_currentNode = _currentNode.Previous) != null)
                    {
                        pointerPoition = _currentNode.Value;
                    }

                }
                catch (NullReferenceException exception)
                {
                    _currentNode = _pointerPositions.Last;
                    pointerPoition = _currentNode.Value;
                    Console.WriteLine(exception);
                }
            }
            else if (e.Position < -1)
            {
                try
                {
                    if ((_currentNode = _currentNode.Next) != null)
                    {
                        pointerPoition = _currentNode.Value;
                    }
                }
                catch (NullReferenceException exception)
                {
                    _currentNode = _pointerPositions.First;
                    pointerPoition = _currentNode.Value;
                    Console.WriteLine(exception);
                }
            }
            Pointer.SetPosition(pointerPoition.Value);
        }

        public override void OnJoyStickButtonPressed(object sender, JoystickButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void OnKeyPressed(object sender, KeyEventArgs e)
        {
            Vector2f? pointerPoition;
            if ((pointerPoition = Pointer.GetPosition()) == null)
            {
                // TODO: Log this error
                return;
            }
            switch (e.Code)
            {
                case Keyboard.Key.Up:
                    try
                    {
                        if ((_currentNode = _currentNode.Previous) != null)
                        {
                            pointerPoition = _currentNode.Value;
                        }

                    }
                    catch (NullReferenceException exception)
                    {
                        _currentNode = _pointerPositions.Last;
                        pointerPoition = _currentNode.Value;
                        Console.WriteLine(exception);
                    }
                    finally
                    {
                        ShiftSound.Play();
                    }
                    break;
                case Keyboard.Key.Down:
                    try
                    {
                        if ((_currentNode = _currentNode.Next) != null)
                        {
                            pointerPoition = _currentNode.Value;
                        }
                    }
                    catch (NullReferenceException exception)
                    {
                        _currentNode = _pointerPositions.First;
                        pointerPoition = _currentNode.Value;
                        Console.WriteLine(exception);
                    }
                    finally
                    {
                        ShiftSound.Play();
                    }
                    break;
            }
            Pointer.SetPosition(pointerPoition.Value);
        }
    }
}
