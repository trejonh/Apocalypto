using System;
using System.Collections.Generic;
using System.Linq;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using Tiled.SFML;

namespace Four_Old_Dudes.Maps
{
    public class GameMap : Map
    {
        public Vector2f PlayerInitialPosition { get; }
        public List<Tiled.SFML.Object> FloorObjects { get; private set; }

        public GameMap(string filename, View view) : base(filename, view)
        {
            DevelopGround();
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

        private void DevelopGround()
        {
            try
            {
                FloorObjects = Objects.Where(obj => obj.Properties["ground"] == "true").ToList();
                if (FloorObjects == null || FloorObjects.Count == 0)
                    throw new Exception("No floor objects found.");
            }
            catch (Exception ex) when (ex is KeyNotFoundException)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
        }


    }
}
