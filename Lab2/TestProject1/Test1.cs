using Lab2.Document;
using Lab2.Users;

namespace TestProject1
{
    [TestClass]
    public sealed class Test1
    {
        [TestClass]
        public class DocumentTests
        {
            private Document _document;

            [TestInitialize]
            public void Setup()
            {
                _document = new Document(DocumentType.PlainText);
                _document.AddText("Hello, world!");
            }

            [TestMethod]
            public void InsertText_InsertsTextAtPosition()
            {
                int position = 7; // После "Hello, "
                string textToInsert = "beautiful ";
                _document.InsertText(position, textToInsert);
                Assert.AreEqual("Hello, beautiful world!", _document.GetOriginalText());
            }

            [TestMethod]
            public void DeleteText_DeletesTextAtPosition()
            {
                int position = 5; // Начало ", world!"
                int length = 7; // Длина ", world"
                _document.DeleteText(position, length);
                Assert.AreEqual("Hello!", _document.GetOriginalText());
            }

            [TestMethod]
            public void FormateText_AppliesBoldFormat()
            {
                int position = 0;
                int length = 5; // "Hello"
                string style = "Bold";
                _document.FormateText(position, length, style);
                Assert.AreEqual("<bHello/b>, world!", _document.GetOriginalText());
            }

            [TestMethod]
            public void getRawPosition_IgnoresFormattingTags()
            {
                _document = new Document(DocumentType.PlainText);
                _document.AddText("<bHello/b>, world!");
                int displayPosition = 5; // Позиция после "Hello" в отображаемом тексте
                int rawPosition = _document.getRawPosition(displayPosition);
                Assert.AreEqual(10, rawPosition); // Позиция после "<b>Hello</b>"
            }

            [TestMethod]
            public void SearchWord_FindsWordIgnoringCase()
            {
                string word = "World";
                using (StringWriter sw = new StringWriter())
                {
                    Console.SetOut(sw);
                    _document.SearchWord(word);
                    string output = sw.ToString().Trim();
                    Assert.IsTrue(output.Contains("начинается с индекса: 7"));
                }
            }

            [TestMethod]
            public void CutText_CutsAndStoresInBuffer()
            {
                int position = 6; // Начало "world!"
                int length = 6; // Длина "world!"
                _document.CutText(position, length);
                Assert.AreEqual("Hello,!", _document.GetOriginalText());
                Assert.AreEqual(" world", _document.buffer);
            }

            [TestMethod]
            public void CopyText_CopiesToBuffer()
            {
                int position = 7; // Начало "world!"
                int length = 6; // Длина "world!"
                _document.CopyText(position, length);
                Assert.AreEqual("Hello, world!", _document.GetOriginalText()); // Текст не изменяется
                Assert.AreEqual("world!", _document.buffer);
            }

            [TestMethod]
            public void PasteText_PastesFromBuffer()
            {
                _document.buffer = "beautiful ";
                int position = 7; // После "Hello, "
                _document.PasteText(position);
                Assert.AreEqual("Hello, beautiful world!", _document.GetOriginalText());
            }

            

            [TestMethod]
            public void GetOriginalText_ReturnsTextWithTags()
            {
                _document = new Document(DocumentType.PlainText);
                _document.AddText("<b>Hello</b>, world!");
                string originalText = _document.GetOriginalText();
                Assert.AreEqual("<b>Hello</b>, world!", originalText);
            }
        }
    }
}
