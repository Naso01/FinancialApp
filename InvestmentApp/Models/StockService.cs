using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

// Keegan Erdis

namespace InvestmentApp.Models
{
   
    public class StockService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IConfiguration _configuration;

        public StockService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Finnhub:ApiKey"];
        }

        // Get live quote for a stock (price only)
        public async Task<StockQuote> GetStockAsync(string symbol)
        {
            var url = $"https://finnhub.io/api/v1/quote?symbol={symbol}&token={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<FinnhubQuoteResponse>(url);

            if (response == null)
                return null;

            return new StockQuote
            {
                Symbol = symbol,
                Price = response.c
            };
        }

        // Get stock details for insertion into DB (symbol, company name, type)
        public async Task<StockDetails> GetStockDetailsAsync(string symbol)
        {
            // Use Finnhub Search to get description/company name
            var url = $"https://finnhub.io/api/v1/search?q={symbol}&token={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<FinnhubSearchResponse>(url);

            if (response == null || response.Result.Count == 0)
                return null;

            var result = response.Result[0]; // Take the first match (want to limit unnecessary Api calls)
            var quote = await GetStockAsync(result.Symbol);

            return new StockDetails
            {
                Symbol = result.Symbol,
                CompanyName = result.Description,
                Price = quote?.Price ?? 0
            };
        }

        // Search stocks 
        public async Task<List<FinnhubStock>> SearchStockAsync(string query)
        {
            var url = $"https://finnhub.io/api/v1/search?q={query}&token={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<FinnhubSearchResponse>(url);

            return response?.Result ?? new List<FinnhubStock>();
        }
    }

    // Additional Models
    public class StockQuote
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
    }

    public class StockDetails
    {
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public decimal Price { get; set; }
    }

    public class FinnhubQuoteResponse
    {
        public decimal c { get; set; } // current price
        public decimal h { get; set; } // high
        public decimal l { get; set; } // low
        public decimal o { get; set; } // open
    }

    public class FinnhubSearchResponse
    {
        public int Count { get; set; }
        public List<FinnhubStock> Result { get; set; }
    }

    public class FinnhubStock
    {
        public string Symbol { get; set; }
        public string Description { get; set; }
        public string DisplaySymbol { get; set; }
        public string Type { get; set; }
    }
}