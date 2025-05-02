using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Document
{
    public class DocumentData
    {
        public List<string> Editors { get; set; }
        public List<string> Viewers { get; set; }
        public DocumentType Type { get; set; }
        public string Content { get; set; }
    }
}
