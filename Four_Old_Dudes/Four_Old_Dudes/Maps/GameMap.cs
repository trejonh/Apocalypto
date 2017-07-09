using System;
using System.Collections.Generic;
using System.Linq;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using Tiled.SFML;
using System.Windows.Media;
using SFML.Audio;

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
        public SFML.Graphics.Color BGColor { get; set; }
        public Music BGMusic { get; set; }

        /// <summary>
        /// Create an instance of a game map
        /// </summary>
        /// <param name="filename">The name of the map to load</param>
        /// <param name="view">The view to render it too</param>
        public GameMap(string filename, View view) : base(filename, view)
        {
            DevelopGround();
            FindEnemySpawns();
            try
            {
                var hexColor = Properties["BGC"];
                var color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(hexColor);
                BGColor = new SFML.Graphics.Color(color.R, color.G, color.B, color.A);
            }
            catch (KeyNotFoundException)
            {
                BGColor = new SFML.Graphics.Color(SFML.Graphics.Color.White);
                LogManager.LogWarning("No background color found for " + filename);
            }
            try
            {
                var music = Properties["BGMusic"];
                BGMusic = AssetManager.LoadMusic(music);
            }
            catch (KeyNotFoundException)
            {
                BGMusic = null;
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
