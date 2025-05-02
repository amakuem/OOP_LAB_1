using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace Lab2.Document
{
    public class LocalFileStrategy: IStorageStrategy
    {
        public async Task SaveDocument(DocumentData data, string fileName)
        {
            string format = Path.GetExtension(fileName).ToLower().TrimStart('.');

            switch (format)
            {
                case "txt":
                    await SaveAsTxtAsync(fileName, data);
                    break;
                case "json":
                    await SaveAsJsonAsync(fileName, data);
                    break;
                case "xml":
                    await SaveAsXmlAsync(fileName, data);
                    break;
                default:
                    throw new ArgumentException("Unsupported file format");
            }
        }

        private async Task SaveAsTxtAsync(string path, DocumentData data)
        {
            string txtContent = $"Type: {data.Type}\n" +
                           $"Editors: {string.Join(", ", data.Editors)}\n" +
                           $"Viewers: {string.Join(", ", data.Viewers)}\n" +
                           $"Content:\n{data.Content}";

            await File.WriteAllTextAsync(path, txtContent);
        }

        private async Task SaveAsJsonAsync(string path, DocumentData data)
        {
            var jsonData = new
            {
                _metadata = new
                {
                    _metadata = new { editors = data.Editors, viewers = data.Viewers },
                    content = data.Content,
                    type = data.Type.ToString()
                },
                content = data.Content
            };
            

            string json = JsonConvert.SerializeObject(jsonData, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(path, json);
        }

        private async Task SaveAsXmlAsync(string path, DocumentData data)
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

        //public async Task<DocumentData> LoadDocument(string fileName)
        //{
        //    if (!File.Exists(fileName))
        //        throw new FileNotFoundException("File not found");

        //    string format = Path.GetExtension(fileName).ToLower().TrimStart('.');
        //    switch (format)
        //    {
        //        case "txt":
        //            string txtContent = await File.ReadAllTextAsync(fileName);
        //            return new DocumentData { Type = DocumentType.PlainText, Content = txtContent };
        //        case "json":
        //            string json = await File.ReadAllTextAsync(fileName);
        //            return JsonConvert.DeserializeObject<DocumentData>(json);
        //        case "xml":
        //            using (var reader = new StreamReader(fileName))
        //            {
        //                var serializer = new XmlSerializer(typeof(DocumentData));
        //                return (DocumentData)serializer.Deserialize(reader);
        //            }
        //        default:
        //            throw new ArgumentException("Unsupported file format");
        //    }
        //}
        public async Task<DocumentData> LoadDocument(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("File not found");

            string format = Path.GetExtension(fileName).ToLower().TrimStart('.');

            return format switch
            {
                "txt" => await LoadTxtAsync(fileName),
                "json" => await LoadJsonAsync(fileName),
                "xml" => await LoadXmlAsync(fileName),
                _ => throw new ArgumentException("Unsupported file format")
            };
        }
        private async Task<DocumentData> LoadTxtAsync(string path)
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
                    data.Editors = ParseUsers(line["Editors:".Length..]);
                }
                else if (line.StartsWith("Viewers:"))
                {
                    data.Viewers = ParseUsers(line["Viewers:".Length..]);
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

        private async Task<DocumentData> LoadJsonAsync(string path)
        {
            string json = await File.ReadAllTextAsync(path);
            dynamic jsonData = JsonConvert.DeserializeObject(json);

            return new DocumentData
            {
                Editors = jsonData._metadata?.editors?.ToObject<List<string>>() ?? new List<string>(),
                Viewers = jsonData._metadata?.viewers?.ToObject<List<string>>() ?? new List<string>(),
                Content = jsonData.content,
                Type = Enum.Parse<DocumentType>(jsonData.type.ToString())
            };
        }

        private async Task<DocumentData> LoadXmlAsync(string path)
        {
            await using var stream = new FileStream(path, FileMode.Open);
            var doc = new XmlDocument();
            doc.Load(stream);

            return new DocumentData
            {
                Editors = ParseXmlUsers(doc, "Editors"),
                Viewers = ParseXmlUsers(doc, "Viewers"),
                Content = doc.SelectSingleNode("//Content")?.InnerText ?? string.Empty,
                Type = DocumentType.Markdown
            };
        }




        private List<string> ParseUsers(string usersLine)
        {
            return usersLine.Trim()
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
        }
        private List<string> ParseXmlUsers(XmlDocument doc, string nodeName)
        {
            var users = new List<string>();
            var nodes = doc.SelectNodes($"//{nodeName}/User");

            if (nodes == null) return users;

            foreach (XmlNode node in nodes)
            {
                users.Add(node.InnerText.Trim());
            }

            return users;
        }
    }
}
