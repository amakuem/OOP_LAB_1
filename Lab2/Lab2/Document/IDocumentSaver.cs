using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Document
{
    public interface IDocumentSaver
    {
        public Task Save(string path, DocumentData document);
    }
}
