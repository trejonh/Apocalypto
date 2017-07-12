using System;
using System.IO;
using System.Xml;
using Four_Old_Dudes.Maps;
using Four_Old_Dudes.Misc;

namespace Four_Old_Dudes.Utils
{
    public static class GameState
    {
        private static readonly string GameSaveLocation = Environment.CurrentDirectory + @"\GameState\";
        public static bool SaveGame(World worldToSave)
        {
            if (worldToSave == null)
            {
                LogManager.LogError("Cannot save game with a null world");
                return false;
            }
            var saveGame = AssetManager.GetMessage("SaveGameMess");

            var items = new [] {
                new InputBoxItem(AssetManager.GetMessage("SaveGame"), saveGame)
            };

            var input = InputBox.Show(AssetManager.GetMessage("SaveGame"), items, InputBoxButtons.SaveCancel);
            var desiredSaveName = "";
            if (input.Result == InputBoxResult.Cancel)
            {
                return false;
            }
            else
            {
                desiredSaveName = InputBox.Res;
            }
            if (string.IsNullOrEmpty(desiredSaveName))
                desiredSaveName = "MyGameSave";
            var path = GameSaveLocation + desiredSaveName;
            var i = 0 ;
            if (Directory.Exists(GameSaveLocation) == false)
                Directory.CreateDirectory(GameSaveLocation);
            while (File.Exists(path + ".xml"))
            {
                path += i;
                i++;
            }
            var settings = new XmlWriterSettings {Indent = true, CloseOutput = true, WriteEndDocumentOnClose = true};

            using (var writer = XmlWriter.Create(path+".xml", settings))
            {
                writer.WriteStartElement("gameSave");
                //begin world
                writer.WriteStartElement("world");
                writer.WriteAttributeString("currentMap",""+worldToSave.CurrentMap);
                //player and their attr
                writer.WriteStartElement("player");
                writer.WriteAttributeString("name", worldToSave.WorldPlayer.Name);
                writer.WriteAttributeString("health", "" + worldToSave.WorldPlayer.Health);
                writer.WriteAttributeString("position", "" + worldToSave.WorldPlayer.Position.X+","+worldToSave.WorldPlayer.Position.Y);
                writer.WriteEndElement();
                //end player
                //enemies
                writer.WriteStartElement("enemiesOnMap");
                foreach (var enemy in worldToSave.EnemiesOnMap)
                {
                    writer.WriteStartElement("enemy");
                    writer.WriteAttributeString("position",""+enemy.Position.X+","+""+enemy.Position.Y);
                    writer.WriteAttributeString("name",enemy.Name);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                //end enemeies
                writer.WriteEndElement();
                //end world
                writer.WriteEndElement();
                //end save
            }
            return true;
        }
    }
}
