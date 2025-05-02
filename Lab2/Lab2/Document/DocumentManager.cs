using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Document
{
    public static class DocumentManager
    {
        public static Document CreateNewDocument(DocumentType type)
        {
            return new Document(type);
        }

        public static void DeleteDocument(string path)
        {
            // Note: Deletion might need strategy-specific logic, but for simplicity:
            if (_storageStrategy is LocalFileStrategy && File.Exists(path))
            {
                var document = OpenDocument(path).Result; // Use await in async context
                document.Notify($"!!! Document deleted: {path} !!!");
                File.Delete(path);
            }
            else
            {
                throw new NotSupportedException("Deletion only supported for local files in this implementation");
            }
        }

        private static IStorageStrategy _storageStrategy = new LocalFileStrategy();
        public static void SetStorageStrategy(IStorageStrategy strategy)
        {
            _storageStrategy = strategy;
        }
        public static async Task<Document> OpenDocument(string fileName)
        {
            var data = await _storageStrategy.LoadDocument(fileName);
            var doc = new Document(data.Type);
            doc.AddText(data.Content);
            doc.filePath = fileName;
            return doc;
        }

        public static async Task SaveDocument(Document document, string fileName)
        {
            var data = new DocumentData { Type = document.type, Content = document.GetOriginalText(), Editors = document.Editors, Viewers = document.Viewers };
            await _storageStrategy.SaveDocument(data, fileName);
            document.filePath = fileName;
            document.Notify($"!!! Document saved to: {fileName} !!!");//
        }
    }
}
