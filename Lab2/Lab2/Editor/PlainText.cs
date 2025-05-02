using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Editor
{
    public class PlainText: IText
    {
        private string _text;
        public PlainText(string text) => _text = text;
        public string Format() => _text;
    }
}
