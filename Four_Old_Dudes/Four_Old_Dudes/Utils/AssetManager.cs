using SFML;
using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Four_Old_Dudes.Utils
{
    public static class AssetManager
    {
        private static readonly Dictionary<string, string> StringAssets = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> TextureAssests = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> AudioAssets = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> MapAssets = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> FontAssets = new Dictionary<string, string>();

        private static readonly string BaseFileLocation =Environment.CurrentDirectory + @"\Assets";
        private static readonly string AssetXmlFile = BaseFileLocation + @".\assets.xml";
        public static void LoadAssets()
        {
            var fs = new FileStream(AssetXmlFile, FileMode.Open, FileAccess.Read);

            var xdoc = XDocument.Load(fs);
            var assets = from u in xdoc.Descendants("asset")
                        select new
                        {
                            Type = (string)u.Element("type"),
                            Name = (string)u.Element("name"),
                            Location = BaseFileLocation + (string)u.Element("location"),
                            Text = (string)u.Element("text")
                        };

            foreach (var asset in assets)
            {
                switch (asset.Type)
                {
                    case "font":
                        FontAssets.Add(asset.Name, asset.Location);
                        break;
                    case "texture":
                        TextureAssests.Add(asset.Name, asset.Location);
                        break;
                    case "map":
                        MapAssets.Add(asset.Name, asset.Location);
                        break;
                    case "audio":
                        AudioAssets.Add(asset.Name, asset.Location);
                        break;
                    case "text":
                        StringAssets.Add(asset.Name, asset.Text);
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
                text = new Texture(TextureAssests[name]);
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
                text = new Texture(TextureAssests[name],rect);
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
                buffer = new SoundBuffer(AudioAssets[name]);
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
                music = new Music(AudioAssets[name]);
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
                mess = StringAssets[name];
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
                font = new Font(FontAssets[name]);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                // TODO: Add a log manager
            }
            return font;
        }
    }
}
