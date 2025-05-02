using Lab2.Document;
using Lab2.Users;
using Lab2.Editor;
using System.Security.Cryptography;
using System.Net.Security;


namespace Lab2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<string> Editors = new List<string> { "admin", "user1" };
            List< string > Viewers = new List<string> { "guest" };
            Lab2.Document.Document doc = new Lab2.Document.Document(DocumentType.Markdown);
            doc.AddText("Hello Babar!");
            doc.Editors = Editors;
            doc.Viewers = Viewers;
            DocumentManager.SetStorageStrategy(new LocalFileStrategy());

            string FileName = "hello.txt";
            await DocumentManager.SaveDocument(doc, FileName);

            doc = await DocumentManager.OpenDocument(FileName);
        }
    }
}
