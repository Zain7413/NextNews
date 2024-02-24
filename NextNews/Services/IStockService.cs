using NextNews.Models.Database;

namespace NextNews.Services
{
    public interface IStockService
    {
        public Task<Stock> GetStockHttpClient(string chosenMarket);
    }
}