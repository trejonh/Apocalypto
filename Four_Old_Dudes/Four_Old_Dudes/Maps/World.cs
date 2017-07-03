
using Four_Old_Dudes.MovingSprites;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
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
        public World()
        {
            _worldPlayer = null;
            _worldPlayer = null;
        }

        public World(ref RenderWindow window, string mapName, string playerName,int firstPlayerFrame, int lastPlayerFrame)
        {
            _winInstance = window;
            _worldMap = AssetManager.LoadGameMap(mapName, window.GetView());
            _worldPlayer = AssetManager.LoadPlayer(playerName, window, firstPlayerFrame, lastPlayerFrame);
            _worldPlayer.SetPosition(_worldMap.PlayerInitialPosition);
            _enemiesOnMap = SpawnEnemies(playerName,firstPlayerFrame,lastPlayerFrame);
            var ts = new ThreadStart(CollisionDetection);
            _collisionThread = new Thread(ts);
            _collisionThread.Priority = ThreadPriority.AboveNormal;
            _collisionThread.IsBackground = true;
            _collisionThread.Start();
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
                    var closeTiles = _worldMap.FloorObjects.Where(tiles => Math.Abs(tiles.Position.X - currPosition.X) <= 100);
                    var closestTile = closeTiles.Where(tile => tile.Position.X <= currPosition.X && tile.Position.Y >= currPosition.Y).Max();
                    _worldPlayer.Ground = closestTile.Position;
                    var dx = Math.Abs(closestTile.Position.X - currPosition.X);
                    if (dx > closestTile.Size.X)
                        _worldPlayer.IsGroundUnderMe = false;
                    else
                        _worldPlayer.IsGroundUnderMe = true;
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
            
        }

        public void AddPlayerAnimation(Direction direction, int firstFrame, int lastFrame)
        {
            _worldPlayer.AddAnimation(direction, firstFrame, lastFrame);
        }
    }
}
