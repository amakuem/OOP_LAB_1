using Lab3.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Application
{
    public interface IQuoteService
    {
        Task<QuoteDTO> GetMotivationalQuoteAsync();
    }
}
