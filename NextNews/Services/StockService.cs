using Newtonsoft.Json;
using NextNews.Models.Database;

namespace NextNews.Services
{
    public class StockService: IStockService
    {
        private readonly HttpClient _httpClient;
        public StockService(IHttpClientFactory httpClientFactory) 
        { 
            _httpClient= httpClientFactory.CreateClient("business");
        }
        public async Task<Stock> GetStockHttpClient(string chosenMarket)
        {
            var content = await _httpClient.GetStringAsync($"summary?region={chosenMarket}&lang=en");

            var result = JsonConvert.DeserializeObject<Stock>(content);
            return result ?? new Stock { top10 = new List<Top10Item>() };


        }
    }
}
