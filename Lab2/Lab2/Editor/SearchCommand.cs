using Lab2.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Editor
{
    public class SearchCommand: ICommand
    {
        private readonly Document.Document _document;
        private string _word;
        private string _previousContent;

        public SearchCommand(Document.Document document, string word)
        {
            _document = document;
            _word = word;
            _previousContent = document.GetOriginalText();
        }
        public void Execute()
        {
            if (!Session.PermissionStrategy.CanEdit())
                throw new InvalidOperationException("You don't have permission to edit.");
            _document.SearchWord(_word);
        }
        public void Undo()
        {
            _document.AddText(_previousContent);
        }
    }
}
