using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab2.Editor
{
    public class TextFormater
    {
        public static string FormatText(string text, string docType)
        {
            if (docType == "Markdown" || docType == "RichText")
            {
                text = Regex.Replace(text, @"^# (.*)$", "\x1b[41m$1\x1b[0m", RegexOptions.Multiline);
                text = Regex.Replace(text, @"^## (.*)$", "\x1b[42m$1\x1b[0m", RegexOptions.Multiline);
                text = Regex.Replace(text, @"^### (.*)$", "\x1b[43m$1\x1b[0m", RegexOptions.Multiline);

                text = Regex.Replace(text, @"<b (.*?) /b>", "\x1b[1m$1\x1b[0m");//bold
                text = Regex.Replace(text, @"<i (.*?) /i>", "\x1b[3m$1\x1b[0m");//italic
                text = Regex.Replace(text, @"<u (.*?) /u>", "\x1b[4m$1\x1b[0m");//underline
            }
            return text;
        }
    }
}
