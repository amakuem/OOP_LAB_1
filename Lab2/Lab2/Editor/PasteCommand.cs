using Lab2.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Editor
{
    public class PasteCommand: ICommand
    {
        private readonly Document.Document _document;
        private int _position;
        private string _previousContent;

        public PasteCommand(Document.Document document, int position)
        {
            _document = document;
            _position = position;
            _previousContent = document.GetOriginalText();
        }
        public void Execute()
        {
            if (!Session.PermissionStrategy.CanEdit())
                throw new InvalidOperationException("You don't have permission to edit.");
            _document.PasteText(_position);
        }
        public void Undo()
        {
            _document.AddText(_previousContent);
        }
    }
}
