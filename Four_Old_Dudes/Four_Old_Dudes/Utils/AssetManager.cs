using SFML;
using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Four_Old_Dudes.Utils
{
    public static class AssetManager
    {
        private static Dictionary<string, string> _stringAssets = new Dictionary<string, string>(), _textureAssests = new Dictionary<string, string>(),
            _audioAssets = new Dictionary<string, string>(), _mapAssets = new Dictionary<string, string>(), _fontAssets = new Dictionary<string, string>();
        private static string baseFileLocation = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)).FullName).FullName + @"\Assets\";
        private static string _assetXMLFile = baseFileLocation + @"assets.xml";
        public static void LoadAssets()
        {
            FileStream fs = new FileStream(_assetXMLFile, FileMode.Open, FileAccess.Read);

            var xdoc = XDocument.Load(fs);
            var assets = from u in xdoc.Descendants("asset")
                        select new
                        {
                            Type = (string)u.Element("type"),
                            Name = (string)u.Element("name"),
                            Location = baseFileLocation + (string)u.Element("location"),
                            Text = (string)u.Element("text")
                        };

            foreach (var asset in assets)
            {
                switch (asset.Type)
                {
                    case "font":
                        _fontAssets.Add(asset.Name, asset.Location);
                        break;
                    case "texture":
                        _textureAssests.Add(asset.Name, asset.Location);
                        break;
                    case "map":
                        _mapAssets.Add(asset.Name, asset.Location);
                        break;
                    case "audio":
                        _audioAssets.Add(asset.Name, asset.Location);
                        break;
                    case "text":
                        _stringAssets.Add(asset.Name, asset.Text);
                        break;
                }
            }
            fs.Close();
        }

        public static Texture LoadTexture(string name)
        {
            Texture text = null;
            try
            {
                text = new Texture(_textureAssests[name]);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                // TODO: Add a log manager
            }
            return text;
        }

        public static Texture LoadTexture(string name, IntRect rect)
        {
            Texture text = null;
            try
            {
                text = new Texture(_textureAssests[name],rect);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                // TODO: Add a log manager
            }
            return text;
        }

        public static Sound LoadSound(string name)
        {
            SoundBuffer buffer = null;
            Sound sound = null;
            try
            {
                buffer = new SoundBuffer(_audioAssets[name]);
                sound = new Sound(buffer);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                // TODO: Add a log manager
            }
            return sound;
        }

        public static Music LoadMusic(string name)
        {
            Music music = null;
            try
            {
                music = new Music(_audioAssets[name]);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                // TODO: Add a log manager
            }
            return music;
        }
        public static string GetMessage(string name)
        {
            string mess = "";
            try
            {
                mess = _stringAssets[name];
            }
            catch (KeyNotFoundException kex)
            {
                // TODO: Add a log manager
            }
            return mess;
        }
        public static Font LoadFont(string name)
        {
            Font font = null;
            try
            {
                font = new Font(_fontAssets[name]);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                // TODO: Add a log manager
            }
            return font;
        }
    }
}
