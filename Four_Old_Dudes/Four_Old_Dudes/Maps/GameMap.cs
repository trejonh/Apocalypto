using System;
using System.Collections.Generic;
using System.Linq;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using Tiled.SFML;
using SFML.Audio;
using static System.Windows.Media.ColorConverter;
using Object = Tiled.SFML.Object;

namespace Four_Old_Dudes.Maps
{
    /// <summary>
    /// A game map
    /// </summary>
    public class GameMap : Map
    {
        public Vector2f PlayerInitialPosition { get; }
        public List<Object> FloorObjects { get; private set; }
        public List<EnemySpawn> EnemySpawns { get; private set; }
        public Color BgColor { get; set; }
        public Music BgMusic { get; set; }
        public string Name { get; set; }
        public Vector2f EndOfMap { get; set; }
        public List<MapItem> ItemsOnMap { get; set; }

        /// <summary>
        /// Create an instance of a game map
        /// </summary>
        /// <param name="filename">The name of the map to load</param>
        /// <param name="view">The view to render it too</param>
        public GameMap(string filename, View view) : base(filename, view)
        {
            DevelopGround();
            FindEnemySpawns();
            FindItems();
            try
            {
                var hexColor = Properties["BGC"];
                var convertFromString = ConvertFromString(hexColor);
                if (convertFromString != null)
                {
                    var color = (System.Windows.Media.Color)convertFromString;
                    BgColor = new Color(color.R, color.G, color.B, color.A);
                }
            }
            catch (KeyNotFoundException)
            {
                BgColor = Color.White;
                LogManager.LogWarning("No background color found for " + filename);
            }
            try
            {
                var music = Properties["BGMusic"];
                BgMusic = AssetManager.LoadMusic(music);
            }
            catch (KeyNotFoundException)
            {
                BgMusic = null;
                LogManager.LogWarning("No background music found for " + filename);
            }
            try
            {
                var eom = Objects.Single(obj => obj.Name.Equals("EndOfMap"));
                if (eom != null)
                    EndOfMap = eom.Position;

            }
            catch (Exception)
            {
                EndOfMap = new Vector2f(0.0f, 0.0f);
                LogManager.LogError("No end of map for " + filename);
            }
            try
            {
                var playerObj = Objects.Single(obj => obj.Name.Equals("playerStartLocation"));
                if (playerObj != null)
                    PlayerInitialPosition = playerObj.Position;
            }
            catch (InvalidOperationException)
            {
                LogManager.LogError("No initial player location was found for map: " + filename);
                PlayerInitialPosition = new Vector2f(0f, 0f);
            }
        }

        /// <summary>
        /// Locate the enemy spawn points
        /// </summary>
        private void FindEnemySpawns()
        {
            try
            {
                var eneObjs = Objects.Where(obj => obj.Name.Equals("enemySpawn"));
                EnemySpawns = new List<EnemySpawn>();
                foreach (var obj in eneObjs)
                {
                    EnemySpawns.Add(new EnemySpawn(obj.Properties["enemyName"], obj.Position));
                }
                if (EnemySpawns.Count == 0)
                    throw new Exception("No enemy spawns objects found.");
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Find all the ground object tiles
        /// </summary>
        private void DevelopGround()
        {
            try
            {
                FloorObjects = Objects.Where(obj => obj.Properties.ContainsKey("type") && obj.Properties["type"].Equals("floor")).ToList();
                if (FloorObjects == null || FloorObjects.Count == 0)
                    throw new Exception("No floor objects found.");
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void FindItems()
        {
            ItemsOnMap = new List<MapItem>();
            foreach(var obj in Objects)
            {
                if (obj.Properties.ContainsKey("type") && obj.Properties["type"].Equals("item"))
                {
                    ItemsOnMap.Add(new MapItem(obj.Name, obj.Position, int.Parse(obj.Properties["points"]), obj));
                }
            }
        }

        public struct EnemySpawn
        {
            public string Name { get; }
            public Vector2f Position { get; }
            public EnemySpawn(string name, Vector2f position)
            {
                Name = name;
                Position = position;
            }
        }
    }
}
