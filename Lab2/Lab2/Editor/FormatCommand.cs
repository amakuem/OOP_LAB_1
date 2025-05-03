using Lab2.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Editor
{
    class FormatCommand: ICommand
    {
        private readonly Document.Document _document;
        private string _style;
        private int _position;
        private int _length;
        private string _previousContent;

        public FormatCommand(Document.Document document, int pos, int len,string style)
        {
            _document = document;
            _position = pos;
            _length = len;
            _style = style;
            _previousContent = _document.GetOriginalText();
        }

        public void Execute()
        {
            if (!Session.PermissionStrategy.CanEdit())
                throw new InvalidOperationException("You don't have permission to edit.");
            _document.FormateText(_position, _length, _style);
        }
        public void Undo()
        {
            _document.AddText(_previousContent);
        }
    }
}
