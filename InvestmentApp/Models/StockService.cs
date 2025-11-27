using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace InvestmentApp.Models
{
   
    public class StockService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "d4k9h0hr01qvpdoiqjhgd4k9h0hr01qvpdoiqji0";

        public StockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Stock> GetStockAsync(string symbol)
        {
            var url = $"https://finnhub.io/api/v1/quote?symbol={symbol}&token={_apiKey}";
            var response = await _httpClient.GetFromJsonAsync<FinnhubQuoteResponse>(url);

            return new Stock
            {
                Symbol = symbol,
                Price = response.c
            };
        }
    }

    public class FinnhubQuoteResponse
    {
        public decimal c { get; set; } // current price
        public decimal h { get; set; } // high
        public decimal l { get; set; } //low
        public decimal o { get; set; } //open
    }

}
