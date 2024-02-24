using Azure.Data.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextNews.Models.Database;
using Stripe;

namespace NextNews.Controllers
{
    [Route("stock")]
    public class StockController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


      
        [HttpGet("getstockdata")]
        public IActionResult GetStockDataView()
        {
            
            return View("GetStockData");
        }


        [Authorize(Roles = "Premium")]

        [HttpGet("stock/getstockdata")]
        public async Task<IActionResult> GetStockData() 
        {
            var tableServiceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=nextnews;AccountKey=30FZFErz2DLbzMBRH/PzQOlVrk0dCSuwfHp5MkVQR1+cNDSF6qk159ci3zhsAyDTC+fHssV33fw1+ASt9+zWQA==;EndpointSuffix=core.windows.net");
            var tableClient = tableServiceClient.GetTableClient("Stocksdata");
            var queryResult = tableClient.Query<Top10Item>(filter:"PartitionKey eq 'us'");

            var data = new List<StockDataModel>();
            foreach (var entity in queryResult)
            {
                data.Add(new StockDataModel
                {
                    Date = entity.Timestamp,
                    Price = entity.close, // Assuming you have a 'Close' property for closing price
                    Name=entity.name,
                    symbol=entity.symbol
                });
            }
            return Ok(data);

        }
    }

    public class StockDataModel
    {
        public DateTimeOffset? Date { get; set; }
        public double Price { get; set; }
        public string Name{ get; set; }
        public string symbol { get; set; }
    }
}
