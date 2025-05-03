using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Lab2.Users;
using Lab2.Editor;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Lab2.Document
{
    public class Document
    {
        public List<string> Editors { get; set; }
        public List<string> Viewers { get; set; }
        public string text;
        public string displayText;
        public string buffer;
        public string filePath { get; set; }
        public DocumentType type { get; set; }
        public readonly DocumentHistory history = new DocumentHistory();
        private List<IObserver> observers = new List<IObserver>();

        public Document(DocumentType type)
        {
            this.type = type;
        }
        public void AddText(string text)
        {
            this.text = text;
        }
        public void InsertText(int position, string? input)
        {
            var beforeContent = GetOriginalText();
            history.AddEntry("INSERT", beforeContent);

            text = text.Insert(getRawPosition(position), input);

            Notify($"!!! Document updated: text appended. !!!");
        }
        public void DeleteText(int position, int length)
        {
            var beforeContent = GetOriginalText();
            history.AddEntry("DELETE", beforeContent);

            text = text.Remove(getRawPosition(position), length);

            Notify($"!!! Document updated: text deleted. !!!");
        }
        public void CutText(int position, int length)
        {
            var beforeContent = GetOriginalText();
            history.AddEntry("CUT", beforeContent);

            buffer = text.Substring(getRawPosition(position), length);
            text = text.Remove(getRawPosition(position), length);

            Notify($"!!! Document updated: cut part of the text. !!!");
        }
        public void CopyText(int position, int length)
        {
            var beforeContent = GetOriginalText();
            history.AddEntry("COPY", beforeContent);

            buffer = text.Substring(getRawPosition(position), length);

            Notify($"!!! Document updated: copy part of the text. !!!");
        }
        public void PasteText(int position)
        {
            var beforeContent = GetOriginalText();
            history.AddEntry("PASTE", beforeContent);

            text = text.Insert(getRawPosition(position), buffer);

            Notify($"!!! Document updated: paste part of the text. !!!");
        }
        public void SearchWord(string word)
        {
            int index = text.IndexOf(word, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
                Console.WriteLine($"Слово '{word}' начинается с индекса: {index}"); 
            else
                Console.WriteLine($"Слово '{word}' не найдено");
        }
        public string GetDisplayText()
        {
            return text;
        }
        public void FormateText(int position, int length, string style)
        {
            var beforeContent = GetOriginalText();
            history.AddEntry("FORMATE", beforeContent);

            IText input = new PlainText(text.Substring(getRawPosition(position), length));
            //text = text.Remove(getRawPosition(position), length);//сделать более точное удаление(чтобы в удалении не участвовали символы форматирование)
            
            IText formatedText = default;
            switch (style)
            {
                case "Bold":
                    formatedText = new BoldText(input);
                    break;
                case "Italic":
                    formatedText = new ItalicText(input);
                    break;
                case "Underline":
                    formatedText = new UnderlineText(input);
                    break; 
            }
            InsertText(position, formatedText.Format());
            text = text.Remove(position + length + 5, length);
            //Console.WriteLine("Choose the formatiotion: ");
            //Console.WriteLine("1.Bold(<b /b>)");
            //Console.WriteLine("2.Italic(<i /i>)");
            //Console.WriteLine("3.Underline(<u /u>)");
            Notify($"!!! Document updated: formate part of the text. !!!");
        }
        public int getRawPosition(int position)
        {
            int rawPosition = -1;
            int result = 0;
            for(int i = 0; i < text.Length; i++)
            {
                if ((text[i] == '<' && (text[i + 1] == 'b' || text[i + 1] == 'i' || text[i + 1] == 'u')))
                {
                    i++;
                    continue;
                }
                else if (text[i] == '/' && (text[i + 1] == 'b' || text[i + 1] == 'i' || text[i + 1] == 'u') && text[i + 2] == '>')
                {
                    i += 2;
                    continue;
                }
                rawPosition++;
                if(rawPosition == position)
                {
                    result = i;
                }
            }
            return result;
        }

        public string GetOriginalText()
        {
            return text;
        }



        public void Subscribe(IObserver observer) => observers.Add(observer);
        public void Unsubscribe(IObserver observer) => observers.Remove(observer);

        public void Notify(string message)
        {
            var fullMessage = $"{message}\nFile: {filePath}";

            foreach (var observer in observers)
            {
                observer.Update(fullMessage);
            }

            foreach (var admin in UserManager.GetAdmins())
            {
                // Чтобы избежать дублей, если админ уже подписан
                if (!observers.Contains(admin))
                {
                    admin.Update($"[ADMIN OVERRIDE] {fullMessage}");
                }
            }
        }

    }
}
