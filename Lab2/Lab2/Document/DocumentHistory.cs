using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Document
{
    public class DocumentHistory
    {
        private readonly List<DocumentSnapshot> _history = new List<DocumentSnapshot>();
        private const int MaxHistoryEntries = 50;

        public void AddEntry(string actionType, string content)
        {
            if (_history.Count >= MaxHistoryEntries)
            {
                _history.RemoveAt(0);
            }

            _history.Add(new DocumentSnapshot(
                DateTime.Now,
                actionType,
                content
            ));
        }

        public IEnumerable<DocumentSnapshot> GetHistory()
        {
            return _history.AsEnumerable().Reverse();
        }

        public void ClearHistory()
        {
            _history.Clear();
        }
    }
}
public record DocumentSnapshot(
      DateTime Timestamp,
      string ActionType,
      string Content
  );

