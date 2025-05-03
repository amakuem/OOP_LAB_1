using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;



namespace Lab2.Document
{
    public static class Parser
    {
        public static List<string> ParseUsers(string usersLine)
        {
            return usersLine.Trim()
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
        }
        public static List<string> ParseXmlUsers(XmlDocument doc, string nodeName)
        {
            var users = new List<string>();
            var nodes = doc.SelectNodes($"//{nodeName}/string");

            if (nodes == null) return users;

            foreach (XmlNode node in nodes)
            {
                users.Add(node.InnerText.Trim());
            }

            return users;
        }
    } 
    public class TxtFileSaver
    {
        public async Task SaveAsTxtAsync(string path, DocumentData data)
        {
            string txtContent = $"Type: {data.Type}\n" +
                           $"Editors: {string.Join(", ", data.Editors)}\n" +
                           $"Viewers: {string.Join(", ", data.Viewers)}\n" +
                           $"Content:\n{data.Content}";

            await File.WriteAllTextAsync(path, txtContent);
        }
    }

    public class JsonFileSaver
    {
        public async Task SaveAsJsonAsync(string path, DocumentData data)
        {
            var jsonData = new
            {
                _metadata = new
                {
                    editors = data.Editors,
                    viewers = data.Viewers,
                    type = data.Type.ToString()
                },
                content = data.Content
            };


            string json = JsonConvert.SerializeObject(jsonData, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(path, json);
        }
    }

    public class XmlFileSaver
    {
        public async Task SaveAsXmlAsync(string path, DocumentData data)
        {
            await using var writer = new StreamWriter(path);

            var xmlSerializer = new XmlSerializer(typeof(DocumentData));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            // Создаем временный объект для совместимости с вашей структурой загрузки
            var tempData = new DocumentData
            {
                Editors = data.Editors,
                Viewers = data.Viewers,
                Content = data.Content,
                Type = data.Type
            };

            xmlSerializer.Serialize(writer, tempData, namespaces);
            await writer.FlushAsync();
        }
    }

    public class TxtFileLoader
    {
        public async Task<DocumentData> LoadTxtAsync(string path)
        {
            string content = await File.ReadAllTextAsync(path);
            var data = new DocumentData { Type = DocumentType.PlainText };
            var lines = content.Split('\n');

            foreach (var line in lines)
            {
                if (line.StartsWith("Type:"))
                    data.Type = (DocumentType)Enum.Parse(typeof(DocumentType), line[5..].Trim());

                if (line.StartsWith("Editors:"))
                {
                    data.Editors = Parser.ParseUsers(line["Editors:".Length..]);
                }
                else if (line.StartsWith("Viewers:"))
                {
                    data.Viewers = Parser.ParseUsers(line["Viewers:".Length..]);
                }
                else if (line.StartsWith("Content:"))
                {
                    int contentStart = lines.ToList().IndexOf(line) + 1;
                    data.Content = string.Join("\n", lines.Skip(contentStart));
                    break;
                }
            }

            return data;
        }
    }

    public class JsonFileLoader
    {
        public async Task<DocumentData> LoadJsonAsync(string path)
        {
            string json = await File.ReadAllTextAsync(path);
            dynamic jsonData = JsonConvert.DeserializeObject(json);

            return new DocumentData
            {
                Editors = jsonData._metadata?.editors?.ToObject<List<string>>() ?? new List<string>(),
                Viewers = jsonData._metadata?.viewers?.ToObject<List<string>>() ?? new List<string>(),
                Type = jsonData._metadata?.type,
                Content = jsonData.content,
            };
        }
    }

    public class XmlFileLoader
    {
        public async Task<DocumentData> LoadXmlAsync(string path)
        {
            await using var stream = new FileStream(path, FileMode.Open);
            var doc = new XmlDocument();
            doc.Load(stream);

            return new DocumentData
            {
                Editors = Parser.ParseXmlUsers(doc, "Editors"),
                Viewers = Parser.ParseXmlUsers(doc, "Viewers"),
                Content = doc.SelectSingleNode("//Content")?.InnerText ?? string.Empty,
                Type = DocumentType.Markdown
            };
        }
    }

    public class TxtSaverAdapter : IDocumentSaver
    {
        private readonly TxtFileSaver _txtFileSaver;

        public TxtSaverAdapter(TxtFileSaver txtFileSaver)
        {
            _txtFileSaver = txtFileSaver;
        }

        public async Task Save(string path, DocumentData document)
        {
           await _txtFileSaver.SaveAsTxtAsync(path, document);
        }
    }

    public class JsonSaverAdapter : IDocumentSaver
    {
        private readonly JsonFileSaver _jsonFileSaver;

        public JsonSaverAdapter(JsonFileSaver jsonFileSaver)
        {
            _jsonFileSaver = jsonFileSaver;
        }

        public async Task Save(string path, DocumentData document)
        {
            await _jsonFileSaver.SaveAsJsonAsync(path, document);
        }
    }

    public class XmlSaverAdapter : IDocumentSaver
    {
        private readonly XmlFileSaver _xmlFileSaver;

        public XmlSaverAdapter(XmlFileSaver xmlFileSaver)
        {
            _xmlFileSaver = xmlFileSaver;
        }

        public async Task Save(string path, DocumentData document)
        {
            await _xmlFileSaver.SaveAsXmlAsync(path, document);
        }
    }

    public class TxtLoaderAdapter : IDocumentLoader
    {
        private readonly TxtFileLoader _txtFileLoader;

        public TxtLoaderAdapter(TxtFileLoader txtFileLoader)
        {
            _txtFileLoader = txtFileLoader;
        }

        public async Task<DocumentData> Load(string path)
        {
            var data = await _txtFileLoader.LoadTxtAsync(path); 
            return data;
        }
    }

    public class JsonLoaderAdapter : IDocumentLoader
    {
        private readonly JsonFileLoader _jsonFileLoader;

        public JsonLoaderAdapter(JsonFileLoader jsonFileLoader)
        {
            _jsonFileLoader = jsonFileLoader;
        }

        public async Task <DocumentData> Load(string path)
        {
            var data = await _jsonFileLoader.LoadJsonAsync(path);
            return data;
        }
    }

    public class XmlLoaderAdapter : IDocumentLoader
    {
        private readonly XmlFileLoader _xmlFileLoader;

        public XmlLoaderAdapter(XmlFileLoader xmlFileLoader)
        {
            _xmlFileLoader = xmlFileLoader;
        }

        public async Task<DocumentData> Load(string path)
        {
            var data = await _xmlFileLoader.LoadXmlAsync(path);
            return data;
        }
    }

    public static class DocumentFormatFactory
    {
        public static IDocumentSaver GetSaver(string format)
        {
            switch (format.ToLower())
            {
                case "txt":
                    return new TxtSaverAdapter(new TxtFileSaver());
                case "json":
                    return new JsonSaverAdapter(new JsonFileSaver());
                case "xml":
                    return new XmlSaverAdapter(new XmlFileSaver());
                default:
                    throw new ArgumentException("Unsupported format");
            }
        }

        public static IDocumentLoader GetLoader(string format)
        {
            switch (format.ToLower())
            {
                case "txt":
                    return new TxtLoaderAdapter(new TxtFileLoader());
                case "json":
                    return new JsonLoaderAdapter(new JsonFileLoader());
                case "xml":
                    return new XmlLoaderAdapter(new XmlFileLoader());
                default:
                    throw new ArgumentException("Unsupported format");
            }
        }
    }
}
