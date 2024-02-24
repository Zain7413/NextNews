
using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;

namespace NextNews.Models.Database
{

    public class Stock 
    {
        public List<Top10Item> top10 { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Top10Item: ITableEntity
    {
      
        public string name { get; set; }
        public string symbol { get; set; }
        public double close { get; set; }
        public double prevClose { get; set; }
        public double percentChange { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

}
