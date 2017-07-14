
using Four_Old_Dudes.Misc;
using Four_Old_Dudes.MovingSprites;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private RenderWindow _winInstance;
        public List<Enemy> EnemiesOnMap { get; private set; }
        private readonly View _worldView;
        private HealthBar _healthBar;
        public int CurrentMap;
        public Color BgColor { get; set; }
        private bool IsInitialMapLoad { get; set; }
        private Text InitLoadText { get; set; }

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
            _winInstance = window;
            _worldView = new View(new FloatRect(new Vector2f(0.0f,0.0f), new Vector2f(window.Size.X,window.Size.Y)));
        }

        /// <summary>
        /// Spawn enemies based off their locations in the map
        /// </summary>
        /// <param name="enemyName">Name of the enemy to spawn</param>
        /// <param name="firstFrame">First frame of the enemy</param>
        /// <param name="lastFrame">Last frame of the enemy</param>
        /// <returns>List of enemies</returns>
        private List<Enemy> SpawnEnemies(string enemyName, int firstFrame, int lastFrame)
        {
            var enemyObjs = WorldMap.EnemySpawns;
            var enemies = new List<Enemy>();
            if (enemyObjs == null || enemyObjs.Count == 0)
                return enemies;
            foreach (var enemy in enemyObjs)
            {
                var en = AssetManager.LoadEnemy(enemyName, _winInstance, WorldPlayer,firstFrame, lastFrame);
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
                    if (GameMaster.IsGamePaused || IsInitialMapLoad) continue;
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
        public void StopCollisionDetection()
        {
            if (_isRunning == false)
            {
                LogManager.LogWarning("Collision detection already stopped");
                return;
            }
            _isRunning = false;
            _collisionThread.Abort();
        }

        /// <summary>
        /// Start collision detection thread
        /// </summary>
        public void StartCollisonDetection()
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
        }
        
        /// <summary>
        /// Set a new player for the world
        /// </summary>
        /// <param name="player">The new player to add</param>
        public void SetPlayer(Player player)
        {
            StopCollisionDetection();
            WorldPlayer = player;
            StartCollisonDetection();
        }

        /// <summary>
        /// Set a new map for the world
        /// </summary>
        /// <param name="map">The new map</param>
        public void SetMap(GameMap map)
        {
            StopCollisionDetection();
            WorldMap = map;
            StartCollisonDetection();
        }

        /// <summary>
        /// Setup a new world entity
        /// </summary>
        /// <param name="player">The new player</param>
        /// <param name="map">The new map</param>
        public void SetupNewWorld(Player player, GameMap map)
        {
            StopCollisionDetection();
            WorldMap = map;
            WorldPlayer = player;
            StartCollisonDetection();
        }

        /// <summary>
        /// Draw all of the world entities to the window
        /// </summary>
        public override void Draw()
        {
            if (IsInitialMapLoad)
                InitialMapLoad();
            else
            {
                if (WorldMap.BgMusic != null && WorldMap.BgMusic.Status != SFML.Audio.SoundStatus.Playing)
                    WorldMap.BgMusic.Play();
                _worldView.Center = WorldPlayer.Position;
                _winInstance.SetView(_worldView);
                _healthBar.SetPosition(new Vector2f(WorldPlayer.Position.X, _worldView.Center.Y - 350));
                _winInstance.Clear(WorldMap.BgColor);
                _winInstance.Draw(WorldMap);
                if (WorldMap.FloorObjects != null)
                {
                    foreach (var floor in WorldMap.FloorObjects)
                        _winInstance.Draw(floor);
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
                WorldPlayer.Update();
                _healthBar.UpdateHealth(WorldPlayer.Health);
                _healthBar.Draw();
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
            WorldPlayer = AssetManager.LoadPlayer(playerName, _winInstance, firstPlayerFrame, lastPlayerFrame);
            WorldPlayer.SetPosition(WorldMap.PlayerInitialPosition);
            WorldPlayer.SetAnimationFrames(frames);
            EnemiesOnMap = SpawnEnemies(playerName,firstPlayerFrame,lastPlayerFrame);
            _worldView.Center = WorldPlayer.Position;
            BgColor = WorldMap.BgColor;
            var ts = new ThreadStart(CollisionDetection);
            _collisionThread = new Thread(ts)
            {
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true
            };
            _collisionThread.Start();
            _healthBar = new HealthBar(ref _winInstance, WorldPlayer.Position);
            var font = AssetManager.LoadFont("OrangeJuice");
            InitLoadText = new Text() { Position = new Vector2f(0,0), DisplayedString = AssetManager.GetMessage(WorldMap.Name), Color = Color.Black, Font = font, CharacterSize = 60 };
            IsInitialMapLoad = true;
            WorldPlayer.ResetWaitTime();
            foreach (var enemy in EnemiesOnMap)
                enemy.ResetWaitTime();
        }
        public void InitialMapLoad()
        {
            var currentCenter = _worldView.Center;
            if(Math.Abs(WorldMap.EndOfMap.X - currentCenter.X) <= _worldView.Size.X / 2.0f)
            {
                IsInitialMapLoad = false;
                return;
            }
            if (_winInstance == null) return;
            InitLoadText.Position = new Vector2f(currentCenter.X, _winInstance.Size.Y/2);
            _worldView.Move(new Vector2f(150 * GameMaster.Delta.AsSeconds(),0.0f));
            _winInstance.SetView(_worldView);
            _winInstance.Clear(WorldMap.BgColor);
            _winInstance.Draw(WorldMap);
            if (WorldMap.FloorObjects != null)
            {
                foreach (var floor in WorldMap.FloorObjects)
                    _winInstance.Draw(floor);
            }
            _winInstance.Draw(InitLoadText);
        }
        public void UnpauseWorld()
        {
            WorldPlayer.AddControls();
        }

        public void Pause()
        {
            WorldPlayer.RemoveControls();
            WorldPlayer.ResetWaitTime();
            foreach (var enemy in EnemiesOnMap)
                enemy.ResetWaitTime();
        }

        public void LoadGame(int currentMap, XElement player, XElement[] ene)
        {
            WorldMap = AssetManager.LoadGameMap(currentMap, _worldView);
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
                    var pos = eneXML
                }
            }
        }
    }
}
