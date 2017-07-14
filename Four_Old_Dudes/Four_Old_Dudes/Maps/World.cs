﻿
using Four_Old_Dudes.Misc;
using Four_Old_Dudes.MovingSprites;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using static Four_Old_Dudes.MovingSprites.Animation;
using static Four_Old_Dudes.MovingSprites.Moveable;

namespace Four_Old_Dudes.Maps
{
    /// <summary>
    /// An instance of the game world
    /// </summary>
    public class World : Core.Drawable
    {
        public  Player WorldPlayer { get; private set; }
        public GameMap WorldMap { get; private set; }
        private bool _isRunning = true;
        private Thread _collisionThread;
        private Thread _mapProgressionThread;
        private Thread _loadingThread;
        private bool _madeItToEnd;
        private bool _loading;
        private bool _dispControllerTime;
        public int NumberOfPlayerLives { get; set; } = 3;
       // private RenderWindow WinInstance;
        public List<Enemy> EnemiesOnMap { get; private set; }
        private readonly View _worldView;
        private HealthBar _healthBar;
        private ScoreDisplay _scoreDisp;
        public int CurrentMap;
        public Color BgColor { get; set; }
        private bool IsInitialMapLoad { get; set; }
        private Text InitLoadText { get; set; }
        private Text _loadingText { get; set; }
        private Text _livesText { get; set; }
        public long Score { get; private set; }
        private bool _displayLives;
        private float _timeToDisp;
        private float _maxTimeToDisp = 4.5f;
        private bool _localPause;
        private int _countDown = 4;
        private Text _countDownText;
        private System.Timers.Timer _countDownTimer;
        private bool _timerStarted;

        /// <summary>
        /// Create an empty world
        /// </summary>
        public World()
        {
            WorldPlayer = null;
            WorldMap = null;
        }

        /// <summary>
        /// Create a full world
        /// </summary>
        /// <param name="window">A window to draw the world's entities.</param>
        public World(ref RenderWindow window)
        {
            WinInstance = window;
            _worldView = new View(new FloatRect(new Vector2f(0.0f,0.0f), new Vector2f(window.Size.X,window.Size.Y)));
        }

        /// <summary>
        /// Spawn enemies based off their locations in the map
        /// </summary>
        /// <param name="firstFrame">First frame of the enemy</param>
        /// <param name="lastFrame">Last frame of the enemy</param>
        /// <returns>List of enemies</returns>
        private List<Enemy> SpawnEnemies(int firstFrame, int lastFrame)
        {
            var enemyObjs = WorldMap.EnemySpawns;
            var enemies = new List<Enemy>();
            if (enemyObjs == null || enemyObjs.Count == 0)
                return enemies;
            foreach (var enemy in enemyObjs)
            {
                var en = AssetManager.LoadEnemy(enemy.Name, WinInstance, WorldPlayer,firstFrame, lastFrame);
                en.SetPosition(enemy.Position);
                en.AddAnimation(Direction.Down, 0, 2);
                en.AddAnimation(Direction.Left, 3, 5);
                en.AddAnimation(Direction.Right, 6, 8);
                en.AddAnimation(Direction.Up, 9, 11);
                enemies.Add(en);
            }
            return enemies;
        }
        
        /// <summary>
        /// Detect sprite collisions and ground collisions
        /// </summary>
        private void CollisionDetection()
        {
            if (WorldMap.FloorObjects == null || WorldMap.FloorObjects.Count == 0)
            {
                _isRunning = false;
                LogManager.LogError("Could not find floor objects for collision detection.");
                WorldPlayer.IsGroundUnderMe = true;
            }
            try
            {
                while (_isRunning)
                {
                    if (GameMaster.IsGamePaused || IsInitialMapLoad || _localPause) continue;
                    var currPosition = WorldPlayer.Position;
                    /*
                     * Find the ground tiles closest to the player
                     * Calculate the distance between the player and said tile
                     * if that distance is greater than the size of tile  then 
                     * the player needs to fall
                     */
                    if (WorldMap.FloorObjects != null || WorldMap.FloorObjects.Count != 0)
                    {
                        var closeTiles =
                            WorldMap.FloorObjects.Where(tiles => Math.Abs(tiles.Position.X - currPosition.X) <= 100);
                        var floor = closeTiles.Select(tile => tile.Position).ToList();
                        if (floor.Count > 0)
                        {
                            var list = SortByDistance(currPosition, floor);
                            var closestTile = closeTiles.First(tile => tile.Position.Equals(list[0]));
                            var bottomPosition = currPosition.Y - WorldPlayer.Height;
                            if (closestTile != null)
                            {
                                WorldPlayer.Ground = closestTile.Position;
                                var dx = Math.Abs(WorldPlayer.Ground.X - currPosition.X);
                                if (dx < (closestTile.Size.X * 0.73f) && bottomPosition <= WorldPlayer.Ground.Y)
                                    WorldPlayer.IsGroundUnderMe = true;
                                else if (dx >= (closestTile.Size.X * 0.73f))
                                    WorldPlayer.IsGroundUnderMe = false;
                            }
                            else
                            {
                                WorldPlayer.IsGroundUnderMe = false;
                            }
                        }
                        else
                        {
                            WorldPlayer.IsGroundUnderMe = false;
                        }
                    }
                    else
                    {
                        LogManager.LogError("No floor found on: "+WorldMap.Name);
                    }
                    if(WorldPlayer.IsGroundUnderMe == false && WorldPlayer.TimeFalling >= (2.5f * 0.92f))
                    {
                        // if player is falling for too long, reset them
                        NumberOfPlayerLives--;
                        Pause();
                        WorldPlayer.SetPosition(WorldMap.PlayerInitialPosition);
                        _displayLives = true;
                        _dispControllerTime = true;
                        UnpauseWorld();
                    }
                    if (EnemiesOnMap.Count == 0) continue;
                    {
                        /*
                         * Detect if the enemy is near an edge, if so then change its direction
                         */
                        foreach (var enemy in EnemiesOnMap)
                        {
                            var edgeTiles = WorldMap.FloorObjects
                                .Where(tile => tile.Position.Y >= (enemy.Position.Y - enemy.Height) && Math.Abs(tile.Position.X - enemy.Position.X) < enemy.Width * 10);
                                var leftEdge = edgeTiles.Count(tiles => tiles.Position.X <= enemy.Position.X);
                                var rightEdge = edgeTiles.Count(tiles => tiles.Position.X > enemy.Position.X);
                                if (leftEdge <= 1)
                                {
                                    enemy.IsNearEdge = true;
                                    enemy.TurnRight = true;
                                    enemy.TurnLeft = false;

                                }
                                else if (rightEdge <= 1)
                                {
                                    enemy.IsNearEdge = true;
                                    enemy.TurnRight = false;
                                    enemy.TurnLeft = true;
                                }
                                else
                                {
                                    enemy.IsNearEdge = false;
                                    enemy.TurnRight = false;
                                    enemy.TurnLeft = false;
                                }
                            if (WorldPlayer.IsIntersecting(enemy.Position))
                            {
                                enemy.Attack(WorldPlayer);
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                LogManager.LogWarning("Aborting collision detection");
            }
            finally
            {
                LogManager.LogWarning("Aborting collision detection");
            }
        }

        /// <summary>
        /// Stop the collision detection thread
        /// </summary>
        public void StopWorld()
        {
            if (_isRunning == false)
            {
                LogManager.LogWarning("All threads for detection already stopped");
                return;
            }
            _isRunning = false;
            _collisionThread.Abort();
            _mapProgressionThread.Abort();
        }

        /// <summary>
        /// Start collision detection thread
        /// </summary>
        public void StartWorld()
        {
            if (_isRunning)
            {
                LogManager.LogWarning("Collision detection already started");
                return;
            }
            _isRunning = true;
            var ts = new ThreadStart(CollisionDetection);
            _collisionThread = new Thread(ts);
            _collisionThread.Priority = ThreadPriority.AboveNormal;
            _collisionThread.IsBackground = true;
            _collisionThread.Start();
            var ts1 = new ThreadStart(DetectMapProgression);
            _mapProgressionThread = new Thread(ts);
            _mapProgressionThread.Priority = ThreadPriority.Normal;
            _collisionThread.IsBackground = true;
            _mapProgressionThread.Start();
        }

        /// <summary>
        /// Set a new player for the world
        /// </summary>
        /// <param name="player">The new player to add</param>
        public void SetPlayer(Player player)
        {
            StopWorld();
            WorldPlayer = player;
            StartWorld();
        }

        /// <summary>
        /// Set a new map for the world
        /// </summary>
        /// <param name="map">The new map</param>
        public void SetMap(GameMap map)
        {
            StopWorld();
            WorldMap = map;
            StartWorld();
        }

        /// <summary>
        /// Setup a new world entity
        /// </summary>
        /// <param name="player">The new player</param>
        /// <param name="map">The new map</param>
        public void SetupNewWorld(Player player, GameMap map)
        {
            StopWorld();
            WorldMap = map;
            WorldPlayer = player;
            StartWorld();
        }

        /// <summary>
        /// Draw all of the world entities to the window
        /// </summary>
        public override void Draw()
        {
            if (IsInitialMapLoad)
                InitialMapLoad();
            else if (_madeItToEnd && _loading == false)
            {
                WorldMap.BgMusic.Stop();
                PrepareNextMap();
            }
            else if(_madeItToEnd && _loading)
            {
                if (GameMaster.IsThemePlaying == false)
                    GameMaster.PlayTheme();
                WinInstance.Clear(GameMaster.ThemeColor);
                WinInstance.Draw(_loadingText);
            }
            else if(_madeItToEnd == false && _loading == false && IsInitialMapLoad ==false)
            {
                if (GameMaster.IsThemePlaying)
                    GameMaster.StopTheme();
                if (WorldMap.BgMusic != null && WorldMap.BgMusic.Status != SFML.Audio.SoundStatus.Playing)
                    WorldMap.BgMusic.Play();
                _worldView.Center = WorldPlayer.Position;
                WinInstance.SetView(_worldView);
                _healthBar.SetPosition(new Vector2f((_worldView.Center.X - _worldView.Size.X/2.0f) + 30, _worldView.Center.Y - 350));
                _scoreDisp.SetPosition(new Vector2f((_worldView.Center.X + _worldView.Size.X / 2.0f), _worldView.Center.Y - 350));
                WinInstance.Clear(WorldMap.BgColor);
                WinInstance.Draw(WorldMap);
                if (WorldMap.FloorObjects != null)
                {
                    foreach (var floor in WorldMap.FloorObjects)
                        WinInstance.Draw(floor);
                }
                if (EnemiesOnMap != null)
                {
                    var center = _worldView.Center;
                    var size = new Vector2f(_worldView.Size.X / 2, _worldView.Size.Y / 2);
                    var leftX = center.X - size.X;
                    var rightX = center.X + size.X;
                    var topY = center.Y - size.Y;
                    var bottomY = center.Y + size.Y;
                    foreach (var enemy in EnemiesOnMap)
                    {
                        var enePos = enemy.Position;
                        if ((enePos.Y < topY)
                            || (enePos.Y > bottomY)
                            || (enePos.X > rightX)
                            || (enePos.X < leftX))
                        {
                            enemy.Stop();
                        }
                        else
                        {
                            enemy.Play();
                        }
                        enemy.Update();
                    }
                }
                if (_displayLives && IsInitialMapLoad == false)
                {
                    DisplayLives();
                    _timeToDisp += GameMaster.Delta.AsSeconds();
                    if(_timeToDisp >= _maxTimeToDisp)
                    {
                        _displayLives = false;
                        _dispControllerTime = false;
                        _timerStarted = false;
                        _countDownTimer.Stop();
                        _timeToDisp = 0.0f;
                    }
                }
                if(_dispControllerTime && IsInitialMapLoad == false)
                {
                    InitDisplayCountDownTimer();
                    if(_countDownText != null)
                        WinInstance.Draw(_countDownText);
                }
                WorldPlayer.Update();
                _healthBar.UpdateHealth(WorldPlayer.Health);
                _scoreDisp.UpdateScore(Score);
                _healthBar.Draw();
                _scoreDisp.Draw();
            }
            
        }

        /// <summary>
        /// Add a player animation
        /// </summary>
        /// <param name="direction">The direction of the frame</param>
        /// <param name="firstFrame">The first frame of the animation</param>
        /// <param name="lastFrame">The last frame of the animation</param>
        public void AddPlayerAnimation(Direction direction, int firstFrame, int lastFrame)
        {
            WorldPlayer.AddAnimation(direction, firstFrame, lastFrame);
        }

        /// <summary>
        /// Sort the list of positions by proximity to target
        /// </summary>
        /// <param name="target">Position target</param>
        /// <param name="listOfPositions">List of positions surrounding target</param>
        /// <returns>List of position with closet position being first</returns>
        private List<Vector2f> SortByDistance(Vector2f target, List<Vector2f> listOfPositions)
        {
            var output = new List<Vector2f>();
            output.Add(listOfPositions[NearestPoint(target, listOfPositions)]);
            listOfPositions.Remove(output[0]);
            int x = 0;
            for (int i = 0; i < listOfPositions.Count + x; i++)
            {
                output.Add(listOfPositions[NearestPoint(output[output.Count - 1], listOfPositions)]);
                listOfPositions.Remove(output[output.Count - 1]);
                x++;
            }
            return output;
        }

        /// <summary>
        /// Find the nearest point
        /// </summary>
        /// <param name="srcPt">The point of origin</param>
        /// <param name="lookIn">The points surrounding the origin</param>
        /// <returns></returns>
        private int NearestPoint(Vector2f srcPt, List<Vector2f> lookIn)
        {
           var smallestDistance = new KeyValuePair<double, int>();
            for (int i = 0; i < lookIn.Count; i++)
            {
                double distance = Math.Sqrt(Math.Pow(srcPt.X - lookIn[i].X, 2) + Math.Pow(srcPt.Y - lookIn[i].Y, 2));
                if (i == 0)
                {
                    smallestDistance = new KeyValuePair<double, int>(distance, i);
                }
                else
                {
                    if (distance < smallestDistance.Key)
                    {
                        smallestDistance = new KeyValuePair<double, int>(distance, i);
                    }
                }
            }
            return smallestDistance.Value;
        }
        public void NewGame(string playerName, int firstPlayerFrame, int lastPlayerFrame, Dictionary<Direction, AnimationFrames> frames)
        {
            CurrentMap = 0;
            WorldMap = AssetManager.LoadGameMap(CurrentMap, _worldView);
            WorldPlayer = AssetManager.LoadPlayer(playerName, WinInstance, firstPlayerFrame, lastPlayerFrame);
            WorldPlayer.SetPosition(WorldMap.PlayerInitialPosition);
            WorldPlayer.SetAnimationFrames(frames);
            EnemiesOnMap = SpawnEnemies(firstPlayerFrame,lastPlayerFrame);
            _worldView.Center = WorldPlayer.Position;
            BgColor = WorldMap.BgColor;
            var ts = new ThreadStart(CollisionDetection);
            _collisionThread = new Thread(ts)
            {
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true
            };
            _collisionThread.Start();
            var win = WinInstance;
            _healthBar = new HealthBar(ref win, WorldPlayer.Position);
            _scoreDisp = new ScoreDisplay(ref win, WorldPlayer.Position);
            var font = AssetManager.LoadFont("OrangeJuice");
            InitLoadText = new Text() { Position = new Vector2f(0,0), DisplayedString = AssetManager.GetMessage(WorldMap.Name), Color = Color.Black, Font = font, CharacterSize = 60 };
            IsInitialMapLoad = true;
            WorldPlayer.ResetWaitTime();
            foreach (var enemy in EnemiesOnMap)
                enemy.ResetWaitTime();
            _displayLives = true;
            _dispControllerTime = true;
        }
        public void InitialMapLoad()
        {
            if (GameMaster.IsThemePlaying == false)
                GameMaster.PlayTheme();
            var currentCenter = _worldView.Center;
            if(Math.Abs(WorldMap.EndOfMap.X - currentCenter.X) <= _worldView.Size.X / 2.0f)
            {
                IsInitialMapLoad = false;
                GameMaster.StopTheme();
                return;
            }
            if (WinInstance == null) return;
            InitLoadText.Position = new Vector2f(currentCenter.X, WinInstance.Size.Y/2);
            _worldView.Move(new Vector2f(150 * GameMaster.Delta.AsSeconds(),0.0f));
            WinInstance.SetView(_worldView);
            WinInstance.Clear(WorldMap.BgColor);
            WinInstance.Draw(WorldMap);
            if (WorldMap.FloorObjects != null)
            {
                foreach (var floor in WorldMap.FloorObjects)
                    WinInstance.Draw(floor);
            }
            WinInstance.Draw(InitLoadText);
        }
        public void UnpauseWorld()
        {
            WorldPlayer.AddControls();
            _localPause = false;
        }

        public void Pause()
        {
            WorldPlayer.RemoveControls();
            WorldPlayer.ResetWaitTime();
            foreach (var enemy in EnemiesOnMap)
                enemy.ResetWaitTime();
            _localPause = true;
        }

        public void LoadGame(int currentMap, long score, int lives, XElement player, XElement[] ene)
        {
            WorldMap = AssetManager.LoadGameMap(currentMap, _worldView);
            Score = score;
            NumberOfPlayerLives = lives;
            var playerAttr = player.Attributes().ToDictionary(attr => attr.Name.LocalName, attr => attr.Value);
            WorldPlayer = AssetManager.LoadPlayer(playerAttr["name"], WinInstance, 0,11);
            WorldPlayer.Health = float.Parse(playerAttr["health"]);
            WorldPlayer.SetAnimationFrames(new Dictionary<Direction, AnimationFrames>
            {
                { Direction.Down, new AnimationFrames(0, 2) },
                { Direction.Left, new AnimationFrames(3, 5) },
                { Direction.Right, new AnimationFrames(6, 8) },
                { Direction.Up, new AnimationFrames(9, 11) }
            });
            WorldPlayer.SetDirection(Direction.Right);
            var pos = playerAttr["position"].Split(',');
            WorldPlayer.SetPosition(new Vector2f(float.Parse(pos[0]),float.Parse(pos[1])));
            foreach (var enemy in WorldMap.EnemySpawns)
            {
                foreach (var eneXML in ene)
                {
                    var enePos = eneXML.FirstAttribute.Value.Split(',');
                    var floatPos = new[] { float.Parse(enePos[0]), float.Parse(enePos[1])};
                    if (enemy.Position.X != floatPos[0] && enemy.Position.Y != floatPos[1])
                        WorldMap.EnemySpawns.Remove(enemy);
                }
            }
            EnemiesOnMap = SpawnEnemies(0, 11);
            _worldView.Center = WorldPlayer.Position;
            BgColor = WorldMap.BgColor;
            var win = WinInstance;
            _healthBar = new HealthBar(ref win, WorldPlayer.Position);
            _scoreDisp = new ScoreDisplay(ref win, WorldPlayer.Position);
            var font = AssetManager.LoadFont("OrangeJuice");
            InitLoadText = new Text() { Position = new Vector2f(0, 0), DisplayedString = AssetManager.GetMessage(WorldMap.Name), Color = Color.Black, Font = font, CharacterSize = 60 };
            IsInitialMapLoad = true;
            WorldPlayer.ResetWaitTime();
            foreach (var enemy in EnemiesOnMap)
                enemy.ResetWaitTime();
            var ts = new ThreadStart(CollisionDetection);
            _collisionThread = new Thread(ts)
            {
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true
            };
            _collisionThread.Start();
            GameMaster.IsMainMenuOpen = false;
            _displayLives = true;
            _dispControllerTime = true;
        }

        private void DetectMapProgression()
        {
            try
            {
                while (_isRunning)
                {
                    if (_localPause) continue;
                    if(WorldMap.EndOfMap.X == WorldPlayer.Position.X &&
                        WorldMap.EndOfMap.Y == WorldPlayer.Position.Y)
                    {
                        _madeItToEnd = true;
                        _isRunning = false;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // no action required ...
            }
            finally
            {
                LogManager.LogWarning("Stopping map progression thread.");
            }
        }

        private void PrepareNextMap()
        {
            CurrentMap++;
            _loading = true;
            if (_loadingText == null)
            {
                var font = AssetManager.LoadFont("OrangeJuice");
                _loadingText = new Text() { Position = new Vector2f(WinInstance.Size.X/2, WinInstance.Size.Y/2), DisplayedString = AssetManager.GetMessage("Loading"), CharacterSize = 60, Color = Color.Black, Font = font};
            }
            WinInstance.SetView(WinInstance.DefaultView);
            var ts = new ThreadStart(LoadNewMap);
            _loadingThread = new Thread(ts) { Priority = ThreadPriority.Highest, IsBackground = true };
            _loadingThread.Start();
        }

        private void LoadNewMap()
        {
            try
            {
                WorldMap = AssetManager.LoadGameMap(CurrentMap, _worldView);
                WorldPlayer.SetPosition(WorldMap.PlayerInitialPosition);
                EnemiesOnMap = SpawnEnemies(0, 11);
                if (InitLoadText == null)
                {
                    var font = AssetManager.LoadFont("OrangeJuice");
                    InitLoadText = new Text() { Position = new Vector2f(0, 0), DisplayedString = AssetManager.GetMessage(WorldMap.Name), Color = Color.Black, Font = font, CharacterSize = 60 };
                }
                else
                    InitLoadText.DisplayedString = AssetManager.GetMessage(WorldMap.Name);
                IsInitialMapLoad = true;
                _loading = false;
                _madeItToEnd = false;
                _displayLives = true;
            }
            catch (Exception ex)
            {
                LogManager.LogError("Failed to load new map. Error mess:\n"+ex.Message);
            }
            finally
            {
                LogManager.Log("New Map Loaded!");
            }
        }

        private void DisplayLives()
        {
            if (_displayLives == false) return;
            if(_livesText == null)
            {
                var font = AssetManager.LoadFont("OrangeJuice");
                _livesText = new Text() { Color = Color.Black, Font = font, CharacterSize = 45, Position = new Vector2f(WorldPlayer.Position.X - 45, WorldPlayer.Position.Y - 80 ), DisplayedString = AssetManager.GetMessage("Lives")+NumberOfPlayerLives};
            }
            _livesText.Position = new Vector2f(WorldPlayer.Position.X - 45, WorldPlayer.Position.Y - 80);
            _livesText.DisplayedString = AssetManager.GetMessage("Lives") + NumberOfPlayerLives;
            WinInstance.Draw(_livesText);
        }
        private void InitDisplayCountDownTimer()
        {
            if (_timerStarted == true) return;
            if (_countDownTimer == null)
            {
                _countDownTimer = new System.Timers.Timer(1000);
                _countDownTimer.Elapsed += DisplayCountdown;
                _countDownTimer.AutoReset = true;
                _countDownTimer.Enabled = true;
            }
            else
                _countDownTimer.Start();
            _timerStarted = true;
        }
        private void DisplayCountdown(object source, ElapsedEventArgs e)
        {
            if (_dispControllerTime == false) return;
            if (_countDown < 0)
                _countDown = 4;
            if (_countDownText == null)
            {
                _countDown = 4;
                var font = AssetManager.LoadFont("OrangeJuice");
                _countDownText = new Text() { Position = new Vector2f(_worldView.Center.X, _worldView.Center.Y - 200.0f), Font = font, Color = Color.Black, CharacterSize = 120, DisplayedString = ""+_countDown};
            }
            if (_countDown == 0)
                _countDownText.DisplayedString = AssetManager.GetMessage("Go");
            else
                _countDownText.DisplayedString = "" + _countDownText;
            _countDown--;
        }
    }
}
