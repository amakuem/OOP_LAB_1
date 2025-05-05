using Lab3.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace Lab3.Application
{
    public class QuoteApiAdapter: IQuoteService
    {
        private readonly HttpClient _httpClient;
        public QuoteApiAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<QuoteDTO> GetMotivationalQuoteAsync()
        {
            var response = await _httpClient.GetAsync("http://quotable.io/random/");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var quote = JsonConvert.DeserializeObject<QuoteResponse>(json);
            return new QuoteDTO { Content = quote.Content };
        }

        private class QuoteResponse
        {
            public string Content { get; set; }
        }
    }
}
