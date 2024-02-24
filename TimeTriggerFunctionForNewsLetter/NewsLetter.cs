using System;
using System.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NextNews.Models.Database;
using NextNews.Services;

namespace TimeTriggerFunctionForNewsLetter
{
    public class NewsLetter
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration; 
        private readonly INewsLetterService _newsLetterService;
        private readonly IArticleService _articleService;
        public NewsLetter(ILoggerFactory loggerFactory, IConfiguration configuration,IArticleService articleService ,INewsLetterService newsLetterService)
        {
            _logger = loggerFactory.CreateLogger<NewsLetter>();
            _configuration = configuration;
            _articleService= articleService;
            _newsLetterService = newsLetterService;
           
        }

        [Function("NewsLetter")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var connectionString = _configuration["AzureWebJobsStorage"];
            var queueString = "newsletter";

            QueueClient queueClient = new QueueClient(connectionString, queueString, new 
                QueueClientOptions {MessageEncoding = QueueMessageEncoding.Base64 }
            );
            
            List<NewsLetterSubscriber> nlSubscriber =_newsLetterService.GetNewsLetterSubscribers();
            foreach (var item in nlSubscriber)
            {
                queueClient.CreateIfNotExists();
                try
                {
                    var recentArticlesTask = _articleService.LatestArticles();
                    var recentArticles = recentArticlesTask.Result;
                    var queueData = new
                    {
                        item.Email,
                        item.FirstName,
                       News= recentArticles
                    };
                    

                    queueClient.SendMessage(JsonConvert.SerializeObject(queueData));
                    _logger.LogInformation($"Message to sent to queue");
                }
                catch (Exception ex) 
                { 
                _logger.LogInformation($"Message could not be sent to queue(error: {ex.Message}) at: {DateTime.Now}");
                }
            }
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
