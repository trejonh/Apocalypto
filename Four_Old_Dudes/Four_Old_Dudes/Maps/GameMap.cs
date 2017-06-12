using System;
using System.Collections.Generic;
using System.Linq;
using Four_Old_Dudes.Utils;
using SFML.Graphics;
using SFML.System;
using Tiled.SFML;
using Box2CS;

namespace Four_Old_Dudes.Maps
{
    public class GameMap : Map
    {
        private readonly World _gameWorld;
        public Vector2f PlayerInitialPosition { get; }

        public GameMap(string filename, View view) : base(filename, view)
        {
            _gameWorld = GameWorld.GameWorld.GetInstance();
            DevelopGround();
            try
            {
                var playerObj = Objects.Single(obj => obj.Properties["playerStartLocation"].Equals("true"));
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
            foreach (var layer in Layers)
            {
                Console.WriteLine(layer.Tiles.Count);
                foreach (var tile in layer.Tiles)
                {
                    try
                    {
                       /* var solid = tile.//.Properties["solid"];
                        if (solid.Equals("false"))
                            continue;
                        var objShape = tile.Shape;
                        var solidBodyDef = new BodyDef();
                        solidBodyDef.Position.Set(objShape.Position.X, objShape.Position.Y);
                        _gameWorld.CreateBody(solidBodyDef);
                        var solidShapeDef = new PolygonDef();
                        solidShapeDef.SetAsBox(tile.Size.X / 2, tile.Size.Y / 2);
                        // solidBody.CreateFixture(solidShapeDef);*/
                    }
                    catch (KeyNotFoundException)
                    {
                        LogManager.LogWarning("Map object: " + tile + " does now have solid property set");
                    }
                }
            }
        }


    }
}
