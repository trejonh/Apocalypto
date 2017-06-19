
using Four_Old_Dudes.MovingSprites;
using Four_Old_Dudes.Utils;
using System;
using System.Linq;
using System.Threading;

namespace Four_Old_Dudes.Maps
{
    public class World
    {
        private Player _worldPlayer;
        private GameMap _worldMap;
        private bool _isRunning = true;
        private Thread _collisionThread;
        public World()
        {
            _worldPlayer = null;
            _worldPlayer = null;
        }

        public World(ref Player player, ref GameMap gameMap)
        {
            _worldMap = gameMap;
            _worldPlayer = player;
            var ts = new ThreadStart(CollisionDetection);
            _collisionThread = new Thread(ts);
            _collisionThread.Priority = ThreadPriority.AboveNormal;
            _collisionThread.IsBackground = true;
            _collisionThread.Start();
        }
        
        private void CollisionDetection()
        {
            if (_worldMap.FloorObjects == null || _worldMap.FloorObjects.Count == 0)
            {
                _isRunning = false;
                LogManager.LogError("Could not find floor objects for collision detection.");
                _worldPlayer.IsGroundUnderMe = false;
            }
            try
            {
                while (_isRunning)
                {
                    var currPosition = _worldPlayer.Position;
                    var closeTiles = _worldMap.FloorObjects.Where(tiles => Math.Abs(tiles.Position.X - currPosition.X) <= 50);
                    var closestTile = closeTiles.Where(tile => tile.Position.X < currPosition.X && tile.Position.Y < currPosition.Y).Max();
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
    }
}
