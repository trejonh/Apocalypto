using System;
using System.IO;
using System.Xml;
using Four_Old_Dudes.Maps;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;
using System.Linq;

namespace Four_Old_Dudes.Utils
{
    public static class GameState
    {
        private static readonly string GameSaveLocation = Environment.CurrentDirectory + @"\GameState\";
        public static bool InputThreadRunning { get; set; } = true;
        private static string DesiredSaveName { get; set; }

        public static void LoadGame()
        {
            var openFileDialog1 = new OpenFileDialog();

            if (Directory.Exists(GameSaveLocation) == false)
                Directory.CreateDirectory(GameSaveLocation);
            openFileDialog1.InitialDirectory = GameSaveLocation;
            openFileDialog1.Filter = "Save Files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            try
            {
                var myStream = openFileDialog1.OpenFile();
                var xdoc = XDocument.Load(myStream);
                var gameSave = from u in xdoc.Descendants("gameSave")
                    select new
                    {
                        World = u.Element("world"),
                    };
                var gameSaves = gameSave.ToArray();
                if (gameSaves.Length == 0)
                    throw new Exception("Not a proper game save file");
                var world = gameSaves[0].World;
                var player = world.Descendants("player").First();
                var enemies = world.Descendants("enemiesOnMap").First();
                GameMaster.LoadGame(world,player,enemies);
            }
            catch (Exception ex)
            {
                LogManager.LogError("Failed to load file\n"+ex.Message);
                MessageBox.Show(AssetManager.GetMessage("LoadError"));
            }
        }
        public static bool SaveGame(World worldToSave)
        {
            if (worldToSave == null)
            {
                LogManager.LogError("Cannot save game with a null world");
                return false;
            }
            DesiredSaveName = "";
            var saveName = DesiredSaveName;
            var res = InputBox(AssetManager.GetMessage("SaveGame"), AssetManager.GetMessage("SaveGame"), ref saveName);
            if (res == DialogResult.OK)
            {
                DesiredSaveName = saveName;
                InputThreadRunning = false;
            }
            if (string.IsNullOrEmpty(DesiredSaveName))
                DesiredSaveName = "MyGameSave";
            var path = GameSaveLocation + DesiredSaveName;
            var i = 0 ;
            var placeHlder = "";
            if (Directory.Exists(GameSaveLocation) == false)
                Directory.CreateDirectory(GameSaveLocation);
            while (File.Exists(path + placeHlder + ".xml"))
            {
                i++;
                placeHlder = "_" + i;
            }
            path += placeHlder;
            if(path.Length >= 260)
            {
                LogManager.LogWarning(path + "is too long of a path name. Resetting");
                path = GameSaveLocation + "Apocalypto";
                while (File.Exists(path + placeHlder + ".xml"))
                {
                    i++;
                    placeHlder = "_" + i;
                }

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

        private static DialogResult InputBox(string title, string promptText, ref string value)
        {
            var form = new Form();
            var label = new Label();
            var textBox = new TextBox();
            var buttonOk = new Button();
            var buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "Save";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            var dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}
