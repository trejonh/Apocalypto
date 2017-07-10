using System;
using System.Collections.Generic;
using System.Linq;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using Tiled.SFML;
using SFML.Audio;
using static System.Windows.Media.ColorConverter;

namespace Four_Old_Dudes.Maps
{
    /// <summary>
    /// A game map
    /// </summary>
    public class GameMap : Map
    {
        public Vector2f PlayerInitialPosition { get; }
        public List<Tiled.SFML.Object> FloorObjects { get; private set; }
        public List<Tiled.SFML.Object> EnemySpawns { get; private set; }
        public Color BgColor { get; set; }
        public Music BgMusic { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Create an instance of a game map
        /// </summary>
        /// <param name="filename">The name of the map to load</param>
        /// <param name="view">The view to render it too</param>
        public GameMap(string filename, View view) : base(filename, view)
        {
            Name = filename;
            DevelopGround();
            FindEnemySpawns();
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
                var playerObj = Objects.Single(obj => obj.Name.Equals("playerStartLocation"));
                if (playerObj != null)
                    PlayerInitialPosition = playerObj.Position;
            }
            catch (InvalidOperationException)
            {
                LogManager.LogError("No initial player location was found for map: "+filename);
                PlayerInitialPosition = new Vector2f(0f,0f);
            }
        }

        /// <summary>
        /// Locate the enemy spawn points
        /// </summary>
        private void FindEnemySpawns()
        {
            try
            {
                EnemySpawns = Objects.Where(obj => obj.Name.Equals("enemySpawn")).ToList();
                if (EnemySpawns == null || EnemySpawns.Count == 0)
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
                FloorObjects = Objects.Where(obj => obj.Name.Equals("groundFloor")).ToList();
                if (FloorObjects == null || FloorObjects.Count == 0)
                    throw new Exception("No floor objects found.");
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
        }


    }
}
