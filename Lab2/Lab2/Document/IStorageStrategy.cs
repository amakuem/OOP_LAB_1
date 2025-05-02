using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Document
{
    public interface IStorageStrategy
    {
        Task SaveDocument(DocumentData data, string fileName);
        Task<DocumentData> LoadDocument(string fileName);
    }
}
