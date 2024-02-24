using System;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NextNews.Models.Database;
using NextNews.Services;

namespace AzureFunctionStockTable
{
    public class StockTable
    {
        private readonly ILogger _logger;
        private readonly IStockService _stockService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        

        public StockTable(ILoggerFactory loggerFactory, IConfiguration configuration, IStockService stockService, IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<StockTable>();
            _configuration = configuration;
            _stockService = stockService;
            _httpClient = httpClientFactory.CreateClient("business");
        }

        [Function("StockTable")]
        public async Task Run([TimerTrigger("0 0 8 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var connectionString = _configuration["AzureWebJobsStorage"];
            var tableName = "Stocksdata";
            var tableClient = new TableClient(connectionString, tableName);
            await tableClient.CreateIfNotExistsAsync();

            string chosenMarket = "us";
            var stockData = await _stockService.GetStockHttpClient(chosenMarket);

            foreach (var item in stockData.top10)
            {
                var entity = new Top10Item
                {

                    PartitionKey = chosenMarket, 
                    RowKey = Guid.NewGuid().ToString(), 
                    name = item.name,
                    symbol = item.symbol,
                    close = item.close,
                    prevClose = item.prevClose,
                    percentChange = item.percentChange,
                    Timestamp = DateTimeOffset.UtcNow,

                };

                await tableClient.AddEntityAsync(entity);
            }

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
          

        }
       


    }
}
