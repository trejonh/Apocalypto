using System;
using System.Collections.Generic;
using Four_Old_Dudes.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static Four_Old_Dudes.MovingSprites.Moveable;
using static Four_Old_Dudes.MovingSprites.Animation;

namespace Four_Old_Dudes.Menus
{
    /// <summary>
    /// The main menu of Four Old Dudes
    /// </summary>
    public class MainMenu : Menu
    {
        private LinkedList<Vector2f> _pointerPositions;
        private LinkedListNode<Vector2f> _currentNode;
        private const int ButtonX = 300, ButtonY = 100;
        private bool _displayChars;
        private List<MenuItem> _charMenuItems;
        private List<MenuItem> _originalMenuItems;
        private LinkedList<Vector2f> _originalPointerPos;
        private List<Sprite> _characterStills;
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
        public MainMenu
            (ref RenderWindow window, Sound shiftSound, Sound selectSound) : base(ref window,
            new List<MenuItem>(), shiftSound, selectSound)
        {
            MenuTitle = AssetManager.GetMessage("DefaultMenuTitle");
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
            var newGame = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = fillColor, Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 6)) };
            var newGameText = new Text() { Position = new Vector2f(newGame.Position.X + 21, newGame.Position.Y + 10), DisplayedString = AssetManager.GetMessage("NewGame"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var loadGame = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = fillColor, Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 4) - 40) };
            var loadGameText = new Text() { Position = new Vector2f(loadGame.Position.X + 14, loadGame.Position.Y + 10), DisplayedString = AssetManager.GetMessage("LoadGame"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var stats = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = fillColor, Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 2) - 80) };
            var statsText = new Text() { Position = new Vector2f(stats.Position.X + 80, stats.Position.Y + 10), DisplayedString = AssetManager.GetMessage("Stats"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var exit = new RectangleShape(new Vector2f(ButtonX, ButtonY)) { FillColor = fillColor, Position = new Vector2f((screenSizeX / 2) - ButtonX / 2, screenSizeY - (ButtonY * 1) - 40) };
            var exitText = new Text() { Position = new Vector2f(exit.Position.X + 100, exit.Position.Y + 10), DisplayedString = AssetManager.GetMessage("Exit"), Color = Color.Black, Font = font, CharacterSize = 60 };
            var pointerSpite = AssetManager.LoadSprite("OldTimeyPointer");
            var renderWindow = WinInstance;
            Pointer = new MenuPointer(ref renderWindow, pointerSpite);
            var loadGameItem = new MenuItem(ref renderWindow, loadGameText, loadGame);
            var statsItem = new MenuItem(ref renderWindow, statsText, stats);
            var newGameItem = new MenuItem(ref renderWindow, newGameText, newGame);
            var exitItem = new MenuItem(ref renderWindow, exitText, exit);
            newGameItem.AddAction(NewGameFunc);
            loadGameItem.AddAction(LoadGameFunc);
            statsItem.AddAction(DisplayStats);
            exitItem.AddAction(ExitGameFunc);
            MenuItems.Add(newGameItem);
            MenuItems.Add(loadGameItem);
            MenuItems.Add(statsItem);
            MenuItems.Add(exitItem);
            _originalMenuItems = new List<MenuItem> {newGameItem, loadGameItem, statsItem, exitItem};
            Pointer.SetPosition(new Vector2f((newGame.Position.X - Pointer.Size.X / 2f), newGame.Position.Y));
            Pointer.SetScale(new Vector2f(0.5f, 0.5f));
            var vector2F = Pointer.GetPosition();
            if (vector2F == null) return;
            _pointerPositions = new LinkedList<Vector2f>(new[]
            {
                vector2F.Value,
                new Vector2f((loadGame.Position.X - Pointer.Size.X / 2f), loadGame.Position.Y),
                new Vector2f((stats.Position.X - Pointer.Size.X / 2f), stats.Position.Y),
                new Vector2f((exit.Position.X - Pointer.Size.X / 2f), exit.Position.Y)
            });
            _originalPointerPos = new LinkedList<Vector2f>(new[]
            {
                vector2F.Value,
                new Vector2f((loadGame.Position.X - Pointer.Size.X / 2f), loadGame.Position.Y),
                new Vector2f((stats.Position.X - Pointer.Size.X / 2f), stats.Position.Y),
                new Vector2f((exit.Position.X - Pointer.Size.X / 2f), exit.Position.Y)
            });
        }

        /// <summary>
        /// Draw menu items and  pointer to the window
        /// </summary>
        public override void Draw()
        {
            if (_displayChars)
                MenuItems = _charMenuItems;
            else if (_originalMenuItems != null && _originalMenuItems.Count != 0)
                MenuItems = _originalMenuItems;
            WinInstance.Clear(GameMaster.ThemeColor);
            base.Draw();
            if (_displayChars && _characterStills != null)
            {
                foreach (var still in _characterStills)
                    WinInstance.Draw(still);
            }
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
                LogManager.LogWarning("The main menu pointer has no location.");
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
                        if (_currentNode != null && (_currentNode = _currentNode.Previous) != null)
                        {
                            pointerPosition = _currentNode.Value;
                        }
                        else
                        {
                            _currentNode = _pointerPositions.Last;
                            pointerPosition = _currentNode.Value;
                        }                        
                        ShiftSound.Play();
                    break;
                case Keyboard.Key.Down:
                        if (_currentNode != null && (_currentNode = _currentNode.Next) != null)
                        {
                            pointerPosition = _currentNode.Value;
                        }
                        else
                        {
                            _currentNode = _pointerPositions.First;
                            pointerPosition = _currentNode.Value;

                        }
                        ShiftSound.Play();
                    break;
                case Keyboard.Key.Return:
                    try
                    {
                        var item = MenuItems[_itemIndex];
                        SelectSound.Play();
                        if(item.DoAction() == false)
                            LogManager.LogWarning("Failed to do menu item's action.");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        LogManager.LogWarning("No menu item found at index: "+_itemIndex);
                    }
                    break;
                case Keyboard.Key.Escape:
                    if (_displayChars)
                    {
                        _itemIndex = 4;
                        DisplayCharMenuItemFunc();
                    }
                    else
                    {
                        ExitGameFunc();
                    }
                    break;
            }
            for (var i = 0; i < MenuItems.Count; i++)
            {
                if ((Math.Abs(pointerPosition.Value.Y - MenuItems[i].Position.Y) <= 0.1))
                    _itemIndex = i;
            }
            Pointer.SetPosition(pointerPosition.Value);
        }


        private bool ExitGameFunc()
        {
            LogManager.CloseLog();
            WinInstance.Close();
            return true;
        }

        private bool NewGameFunc()
        {
            DisplayCharacters();
            return true;
        }
        private bool LoadGameFunc()
        {
            GameState.LoadGame();
            return true;
        }
        private bool DisplayStats()
        {
            Console.WriteLine("displaying stats");
            return true;
        }

        private bool DisplayCharMenuItemFunc()
        {
            switch (_itemIndex)
            {
                case 0:
                    Console.WriteLine("Mack");

                    var frame = new Dictionary<Direction, AnimationFrames>
                    {
                        { Direction.Down, new AnimationFrames(0, 2) },
                        { Direction.Left, new AnimationFrames(3, 5) },
                        { Direction.Right, new AnimationFrames(6, 8) },
                        { Direction.Up, new AnimationFrames(9, 11) }
                    };
                    GameMaster.NewGame("TestPlayer", 0, 11, frame);
                    GameMaster.IsMainMenuOpen = false;
                    break;
                case 1:
                    Console.WriteLine("Rob");
                    break;
                case 2:
                    Console.WriteLine("Doug");
                    break;
                case 3:
                    Console.WriteLine("Lou");
                    break;
                case 4:
                    _displayChars = false;
                    _itemIndex = 0;
                    _pointerPositions = _originalPointerPos;
                    _currentNode = _pointerPositions.First;
                    Pointer.Move(_originalPointerPos.First.Value);
                    break;
            }
            return true;
        }

        private void DisplayCharacters()
        {
            var screenSize = WinInstance.Size;
            _charMenuItems = new List<MenuItem>();
            var names = new[]
            {
                AssetManager.GetMessage("Mack"), AssetManager.GetMessage("Rob"),
                AssetManager.GetMessage("Doug"),AssetManager.GetMessage("Lou")
            };
            var font = AssetManager.LoadFont("OrangeJuice");
            for (var i = 0; i < 5; i++)
            {
                Shape shape;
                Text text;
                if (i <= 3)
                {
                    if (i == 0)
                        shape = new CircleShape(64.0f)
                        {
                            OutlineThickness = 2,
                            FillColor = Color.Transparent,
                            Position = new Vector2f(((float)screenSize.X / 4) + 50, ((float)screenSize.Y / 16))
                        };
                    else
                        shape = new CircleShape(64.0f)
                        {
                            OutlineThickness = 3,
                            FillColor = Color.Transparent,
                            Position = new Vector2f(((float)screenSize.X / 4) + 50, ((float)screenSize.Y / 16) + (i * 128) + (i * 32))
                        };
                    text = new Text(names[i], font)
                    {
                        CharacterSize = 60,
                        Color = Color.Black,
                        Position = new Vector2f(shape.Position.X + 200, shape.Position.Y + 32)
                    };
                }
                else
                {
                    shape = new RectangleShape(new Vector2f(ButtonX, ButtonY))
                    {
                        FillColor = new Color(128, 128, 128),
                        Position = new Vector2f(screenSize.X / 1.5f + 50, ((float) screenSize.Y / 12) + (i * 128) + 20)
                    };
                    text = new Text(AssetManager.GetMessage("Back"), font)
                    {
                        Color = Color.Black,
                        CharacterSize = 60,
                        Position = new Vector2f(shape.Position.X + 90, shape.Position.Y + 15)
                    };
                }
                var renderWindow = WinInstance;
                _charMenuItems.Add(new MenuItem(ref renderWindow, text, shape, DisplayCharMenuItemFunc));
            }
            _displayChars = true;
            var newPos = new LinkedList<Vector2f>();
            foreach (var t in _charMenuItems)
            {
                var pos = t.Position;
                newPos.AddLast(new Vector2f((pos.X - Pointer.Size.X / 2f), pos.Y));
            }
            var scale = new Vector2f(2.5f, 2.5f);
            try
            {
                var mack = AssetManager.LoadSprite("MackStill");
                mack.Scale = scale;
                mack.Position = new Vector2f(_charMenuItems[0].Position.X + 24, _charMenuItems[0].Position.Y + 24);
                var doug = AssetManager.LoadSprite("MackStill");
                doug.Scale = scale;
                doug.Position = new Vector2f(_charMenuItems[1].Position.X + 24, _charMenuItems[1].Position.Y + 24);
                var lou = AssetManager.LoadSprite("MackStill");
                lou.Scale = scale;
                lou.Position = new Vector2f(_charMenuItems[2].Position.X + 24, _charMenuItems[2].Position.Y + 24);
                var rob = AssetManager.LoadSprite("MackStill");
                rob.Scale = scale;
                rob.Position = new Vector2f(_charMenuItems[3].Position.X + 24, _charMenuItems[3].Position.Y + 24);
                _characterStills = new List<Sprite>(new[] {
                mack,doug,lou,rob
                });
            }
            catch (NullReferenceException)
            {
                LogManager.LogError("Could not load character stills.");
            }
            _pointerPositions = newPos;
            Pointer.Move(_pointerPositions.First.Value);
            _currentNode = _pointerPositions.First;
            _itemIndex = 0;
        }
    }
}