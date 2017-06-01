using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Four_Old_Dudes.Utils
{
    public static class AssetManager
    {
        private static Dictionary<string, string> _stringAssets = new Dictionary<string, string>(), _textureAssests = new Dictionary<string, string>(),
            _audioAssets = new Dictionary<string, string>(), _mapAssets = new Dictionary<string, string>();
        private static string _assetXMLFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),@"Assets\assets.xml");
        public static void LoadAssets()
        {
            var settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            var reader = XmlReader.Create(_assetXMLFile, settings);

            reader.MoveToContent();
            // Parse the file and display each of the nodes.
            while (reader.Read())
            {
                XmlNode node = (XmlNode)reader.ReadElementContentAsObject();
                string type, location, name;
                foreach(XmlNode asset in node.ChildNodes)
                {
                    if (asset.Name.Equals("type"))
                        type = asset.Value;
                    else if (asset.Name.Equals("name"))
                        name = asset.Value;
                    else if (asset.Name.Equals("location"))
                }
                
            }
            reader.Close();
            /*while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        Console.Write("<{0}>", reader.Name);
                        break;
                    case XmlNodeType.Text:
                        Console.Write(reader.Value);
                        break;
                    case XmlNodeType.CDATA:
                        Console.Write("<![CDATA[{0}]]>", reader.Value);
                        break;
                    case XmlNodeType.ProcessingInstruction:
                        Console.Write("<?{0} {1}?>", reader.Name, reader.Value);
                        break;
                    case XmlNodeType.Comment:
                        Console.Write("<!--{0}-->", reader.Value);
                        break;
                    case XmlNodeType.XmlDeclaration:
                        Console.Write("<?xml version='1.0'?>");
                        break;
                    case XmlNodeType.Document:
                        break;
                    case XmlNodeType.DocumentType:
                        Console.Write("<!DOCTYPE {0} [{1}]", reader.Name, reader.Value);
                        break;
                    case XmlNodeType.EntityReference:
                        Console.Write(reader.Name);
                        break;
                    case XmlNodeType.EndElement:
                        Console.Write("</{0}>", reader.Name);
                        break;
                }
            }*/
        }
    }
}
