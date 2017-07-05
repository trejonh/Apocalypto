﻿
using Four_Old_Dudes.Extensions;
using Four_Old_Dudes.MovingSprites;
using Four_Old_Dudes.Utils;
using MoreLinq;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Four_Old_Dudes.MovingSprites.Moveable;

namespace Four_Old_Dudes.Maps
{
    public class World : Core.Drawable
    {
        private Player _worldPlayer;
        private GameMap _worldMap;
        private bool _isRunning = true;
        private Thread _collisionThread;
        private RenderWindow _winInstance;
        private List<Enemy> _enemiesOnMap;
        private View _worldView;
        private RoundedRectangle _healthBar;
        private Text _healthText;
        public Color BGColor { get; set; }
        public World()
        {
            _worldPlayer = null;
            _worldPlayer = null;
        }

        public World(ref RenderWindow window, string mapName, string playerName,int firstPlayerFrame, int lastPlayerFrame)
        {
            _winInstance = window;
            _worldView = new View(new FloatRect(new Vector2f(0.0f,0.0f), new Vector2f(window.Size.X,window.Size.Y)));
            _worldMap = AssetManager.LoadGameMap(mapName, window.GetView());
            _worldPlayer = AssetManager.LoadPlayer(playerName, window, firstPlayerFrame, lastPlayerFrame);
            _worldPlayer.SetPosition(_worldMap.PlayerInitialPosition);
            _enemiesOnMap = SpawnEnemies(playerName,firstPlayerFrame,lastPlayerFrame);
            _worldView.Center = _worldPlayer.Position;
            BGColor = _worldMap.BGColor;
            var ts = new ThreadStart(CollisionDetection);
            _collisionThread = new Thread(ts);
            _collisionThread.Priority = ThreadPriority.AboveNormal;
            _collisionThread.IsBackground = true;
            _collisionThread.Start();
            var fillColor = new Color(138,7,7,125);
            var font = AssetManager.LoadFont("OrangeJuice");
            _healthBar = new RoundedRectangle(new Vector2f(200.0f, 75.0f),5,4) { FillColor = fillColor, Position = new Vector2f(_worldPlayer.Position.X,100), OutlineThickness = 3};
            _healthText = new Text() { Position = _healthBar.Position, DisplayedString = AssetManager.GetMessage("Health"), Color = Color.Black, Font = font, CharacterSize = 60 };

        }

        private List<Enemy> SpawnEnemies(string enemyName, int firstFrame, int lastFrame)
        {
            var enemyObjs = _worldMap.EnemySpawns;
            var enemies = new List<Enemy>();
            if (enemyObjs == null || enemyObjs.Count == 0)
                return enemies;
            foreach (var enemy in enemyObjs)
            {
                var en = AssetManager.LoadEnemy(enemyName, _winInstance, _worldPlayer,firstFrame, lastFrame);
                en.SetPosition(enemy.Position);
                en.AddAnimation(Direction.Down, 0, 2);
                en.AddAnimation(Direction.Left, 3, 5);
                en.AddAnimation(Direction.Right, 6, 8);
                en.AddAnimation(Direction.Up, 9, 11);
                enemies.Add(en);
            }
            return enemies;
        }
        
        private void CollisionDetection()
        {
            if (_worldMap.FloorObjects == null || _worldMap.FloorObjects.Count == 0)
            {
                _isRunning = false;
                LogManager.LogError("Could not find floor objects for collision detection.");
                _worldPlayer.IsGroundUnderMe = true;
            }
            try
            {
                while (_isRunning)
                {
                    var currPosition = _worldPlayer.Position;
                    var pWidth = _worldPlayer.Width;
                    var dy = 0.0f;
                    var closeTiles = _worldMap.FloorObjects.Where(tiles => Math.Abs(tiles.Position.X - currPosition.X) <= 100);
                    var floor = new List<Vector2f>();
                    foreach(var tile in closeTiles)
                    {
                        floor.Add(tile.Position);
                    }
                    if(floor.Count > 0)
                    {
                        var list = SortByDistance(currPosition, floor);
                        var closestTile = closeTiles.Where(tile => tile.Position.Equals(list[0])).First();
                        var bottomPosition = currPosition.Y - _worldPlayer.Height;
                        if (closestTile != null)
                        {
                            _worldPlayer.Ground = closestTile.Position;
                            var dx = Math.Abs(_worldPlayer.Ground.X - currPosition.X);
                            Console.WriteLine("Dx: {0}", dx);
                            if (dx < (closestTile.Size.X * 0.73f) && bottomPosition <= _worldPlayer.Ground.Y)
                                _worldPlayer.IsGroundUnderMe = true;
                            else if (dx >= (closestTile.Size.X * 0.73f))
                                _worldPlayer.IsGroundUnderMe = false;
                        }
                        else
                        {
                            _worldPlayer.IsGroundUnderMe = false;
                        }
                    }
                    else
                    {
                        _worldPlayer.IsGroundUnderMe = false;
                    }
                   /*if(dx < 0.0f)
                    {

                    }
                    else //left of tile
                    {

                    }*/
                    if(_enemiesOnMap.Count != 0)
                    {
                        foreach (var enemy in _enemiesOnMap)
                        {
                            var edgeTiles = _worldMap.FloorObjects
                                                    .Where(tile => tile.Position.Y >= (enemy.Position.Y - enemy.Height) && Math.Abs(tile.Position.X - enemy.Position.X) < enemy.Width * 10);
                            if (edgeTiles != null)
                            {
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
                            }
                            else
                            {
                                enemy.IsNearEdge = true;
                            }
                            if (_worldPlayer.IsIntersecting(enemy.Position))
                            {
                                enemy.Attack(_worldPlayer);
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
        
        public void SetPlayer(Player player)
        {
            StopCollisionDetection();
            _worldPlayer = player;
            StartCollisonDetection();
        }

        public void SetMap(GameMap map)
        {
            StopCollisionDetection();
            _worldMap = map;
            StartCollisonDetection();
        }

        public void SetupNewWorld(Player player, GameMap map)
        {
            StopCollisionDetection();
            _worldMap = map;
            _worldPlayer = player;
            StartCollisonDetection();
        }

        public override void Draw()
        {
            _worldView.Center = _worldPlayer.Position;
            _winInstance.SetView(_worldView);
            _healthBar.Position = new Vector2f(_worldPlayer.Position.X,_worldView.Center.Y-350);
            _healthText.Position = new Vector2f(_healthBar.Position.X + 15, _healthBar.Position.Y);
            _winInstance.Draw(_worldMap);
            if (_worldMap.FloorObjects != null)
            {
                foreach (var floor in _worldMap.FloorObjects)
                    _winInstance.Draw(floor);
            }
            if (_enemiesOnMap != null)
            {
                foreach (var enemy in _enemiesOnMap)
                    enemy.Update();
            }
            _worldPlayer.Update();
            _winInstance.Draw(_healthBar);
            _winInstance.Draw(_healthText);
            
        }

        public void AddPlayerAnimation(Direction direction, int firstFrame, int lastFrame)
        {
            _worldPlayer.AddAnimation(direction, firstFrame, lastFrame);
        }

        private List<Vector2f> SortByDistance(Vector2f src, List<Vector2f> lst)
        {
            var output = new List<Vector2f>();
            output.Add(lst[NearestPoint(src, lst)]);
            lst.Remove(output[0]);
            int x = 0;
            for (int i = 0; i < lst.Count + x; i++)
            {
                output.Add(lst[NearestPoint(output[output.Count - 1], lst)]);
                lst.Remove(output[output.Count - 1]);
                x++;
            }
            return output;
        }

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
    }
}
