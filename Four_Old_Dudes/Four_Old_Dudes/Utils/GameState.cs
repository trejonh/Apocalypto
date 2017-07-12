using System;
using System.IO;
using System.Xml;
using Four_Old_Dudes.Maps;
using Four_Old_Dudes.Misc;
using System.Threading;

namespace Four_Old_Dudes.Utils
{
    public static class GameState
    {
        private static readonly string GameSaveLocation = Environment.CurrentDirectory + @"\GameState\";
        private static bool _inputThreadRunning {get;set;}
        private static string _desiredSaveName { get; set; }
        public static bool SaveGame(World worldToSave)
        {
            if (worldToSave == null)
            {
                LogManager.LogError("Cannot save game with a null world");
                return false;
            }
            _desiredSaveName = "";
            var inputThreadStart = new ThreadStart(AskSaveLocation);
            var inputThread = new Thread(inputThreadStart) { Priority = ThreadPriority.Normal};
            _inputThreadRunning = true;
            inputThread.Start();
            while (_inputThreadRunning) { /* do nothing */}
            Console.WriteLine("aborting");
            inputThread.Abort();
            if (string.IsNullOrEmpty(_desiredSaveName))
                _desiredSaveName = "MyGameSave";
            var path = GameSaveLocation + _desiredSaveName;
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

        private static void AskSaveLocation()
        {
            InputBox input = null;
            try
            {
                var saveGame = AssetManager.GetMessage("SaveGameMess");
                var items = new[] {
                new InputBoxItem(AssetManager.GetMessage("SaveGame"), saveGame)
            };
                input = InputBox.Show(AssetManager.GetMessage("SaveGame"), items, InputBoxButtons.SaveCancel);
                if (input.Result == InputBoxResult.Cancel)
                {
                    _inputThreadRunning = false;
                }
                else if (input.Result == InputBoxResult.Save)
                {
                    //desiredSaveName = input.Items[AssetManager.GetMessage("SaveGame")];
                    _inputThreadRunning = false;
                }
            }
            catch (ThreadAbortException)
            {
                LogManager.LogWarning("Aborting save thread");
            }
            finally
            {
                LogManager.LogWarning("Aborting save thread");
                input.Dispose();
            }
        }
    }
}
