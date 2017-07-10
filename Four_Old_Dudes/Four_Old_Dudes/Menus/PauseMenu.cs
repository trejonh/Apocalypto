
using System;
using System.Collections.Generic;
using Four_Old_Dudes.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Four_Old_Dudes.Menus
{
    public class PauseMenu : Menu
    {
        private LinkedList<Vector2f> _pointerPositions;
        private LinkedListNode<Vector2f> _currentNode;
        private const int ButtonX = 300, ButtonY = 100;
        private int _itemIndex;
        /// <summary>
        /// The menu title
        /// </summary>
        public string MenuTitle { get; set; }

        /// <summary>
        /// Construct a new main menu
        /// </summary>
        /// <param name="window">Reference to the window to draw to</param>
        /// <param name="shiftSound">Sound to play when shifting through menu options</param>
        /// <param name="selectSound">Sound to play when selecting a menu option</param>
        public PauseMenu(ref RenderWindow window, Sound shiftSound, Sound selectSound) : base(ref window,
            new List<MenuItem>(), shiftSound, selectSound)
        {
            MenuTitle = AssetManager.GetMessage("PauseMenu");
            _itemIndex = 0;
            SetupMenu();
        }

        /// <summary>
        /// Set up main menu
        /// </summary>
        private void SetupMenu()
        {
            uint screenSizeX = WinInstance.Size.X, screenSizeY = WinInstance.Size.Y;
            var fillColor = new Color(128, 128, 128);
            var font = AssetManager.LoadFont("OrangeJuice");
            var resumeGame = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = fillColor, Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 6)) };
            var resumeGameText = new Text() { Position = new Vector2f(resumeGame.Position.X + 50, resumeGame.Position.Y + 10), DisplayedString = AssetManager.GetMessage("ResumeGame"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var saveGame = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = fillColor, Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 4) - 40) };
            var saveGameText = new Text() { Position = new Vector2f(saveGame.Position.X + 14, saveGame.Position.Y + 10), DisplayedString = AssetManager.GetMessage("SaveGame"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var exit = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = fillColor, Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 2) - 40) };
            var exitText = new Text() { Position = new Vector2f(exit.Position.X + 100, exit.Position.Y + 10), DisplayedString = AssetManager.GetMessage("Exit"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var pointerSpite = AssetManager.LoadSprite("OldTimeyPointer");
            var renderWindow = WinInstance;
            Pointer = new MenuPointer(ref renderWindow, pointerSpite);
            MenuItems.Add(new MenuItem(ref renderWindow, resumeGameText, resumeGame));
            MenuItems.Add(new MenuItem(ref renderWindow, saveGameText, saveGame));
            var exitItem = new MenuItem(ref renderWindow, exitText, exit);
            exitItem.AddAction(ExitGameFunc);
            MenuItems.Add(exitItem);
            Pointer.SetPosition(new Vector2f((resumeGame.Position.X - Pointer.Size.X / 2f), resumeGame.Position.Y));
            Pointer.SetScale(new Vector2f(0.5f, 0.5f));
            var vector2F = Pointer.GetPosition();
            if (vector2F != null)
                _pointerPositions = new LinkedList<Vector2f>(new[]
                {
                    vector2F.Value,
                    new Vector2f((saveGame.Position.X - Pointer.Size.X / 2f), saveGame.Position.Y),
                    new Vector2f((exit.Position.X - Pointer.Size.X / 2f), exit.Position.Y)
                });
        }

        /// <summary>
        /// Draw menu items and  pointer to the window
        /// </summary>
        public override void Draw()
        {
            base.Draw();
            Pointer.Draw();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    LogManager.LogWarning(exception.Message);
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
                    LogManager.LogWarning(exception.Message);
                }
            }
            Pointer.SetPosition(pointerPoition.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnJoyStickButtonPressed(object sender, JoystickButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnKeyPressed(object sender, KeyEventArgs e)
        {
            Vector2f? pointerPosition;
            if ((pointerPosition = Pointer.GetPosition()) == null)
            {
                LogManager.LogWarning("No position for the pointer found on the main menu");
                return;
            }
            switch (e.Code)
            {
                case Keyboard.Key.Up:
                    try
                    {
                        if ((_currentNode = _currentNode.Previous) != null)
                        {
                            pointerPosition = _currentNode.Value;
                            _itemIndex--;
                        }

                    }
                    catch (NullReferenceException exception)
                    {
                        _currentNode = _pointerPositions.Last;
                        _itemIndex = 3;
                        pointerPosition = _currentNode.Value;
                        LogManager.LogWarning(exception.Message);
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
                            pointerPosition = _currentNode.Value;
                            _itemIndex++;
                        }
                    }
                    catch (NullReferenceException exception)
                    {
                        _currentNode = _pointerPositions.First;
                        _itemIndex = 0;
                        pointerPosition = _currentNode.Value;
                        LogManager.LogWarning(exception.Message);
                    }
                    finally
                    {
                        ShiftSound.Play();
                    }
                    break;
                case Keyboard.Key.Return:
                    try
                    {
                        var item = MenuItems[_itemIndex];
                        SelectSound.Play();
                        if (item.DoAction() == false)
                            LogManager.LogWarning("Failed to do menu item's action.");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        LogManager.LogWarning("No menu item found at index: " + _itemIndex);
                    }
                    break;
            }
            Pointer.SetPosition(pointerPosition.Value);
        }


        private bool ExitGameFunc()
        {
            LogManager.CloseLog();
            WinInstance.Close();
            return true;
        }
    }
}
