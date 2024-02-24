using System;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NextNews.Models.Database;
using NextNews.Services;
//using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TimeTriggerExpiredSubsFunction
{
    public class SubsExpiredFunction
    {
        private readonly ILogger _logger;
        private readonly IArticleService _articleService;
        private readonly IConfiguration _configuration;
        private readonly ISubscriptionService _subscriptionService;

        public SubsExpiredFunction(ILoggerFactory loggerFactory, IArticleService articleService, IConfiguration configuration,ISubscriptionService subscriptionService )
        {
            _logger = loggerFactory.CreateLogger<SubsExpiredFunction>();
            _articleService = articleService;
            _configuration = configuration;
            _subscriptionService = subscriptionService;
        }

        [Function("SubsExpiredFunction")]
        public void Run([TimerTrigger("0 */5 * * * *", RunOnStartup =true)] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _articleService.CheckExpiredSubs();
            var connectionString = _configuration["AzureWebJobsStorage"];
            var queueString = "expiredsubscriptionqueue";


            QueueClient queueClient = new QueueClient(connectionString, queueString, new
                QueueClientOptions
            { MessageEncoding = QueueMessageEncoding.Base64 }

            );
            List<Subscription> paidsubscriber = _subscriptionService.GetSubscriptionsAsync();
            foreach (var item in paidsubscriber)
            {
                queueClient.CreateIfNotExists();
                try
                {
                    Subscription queueSubscription = new Subscription();
                    queueSubscription.SubscriptionTypeId = item.SubscriptionTypeId;
                    queueSubscription.Expired = item.Expired;
                    queueSubscription.UserId = item.UserId;
                    queueSubscription.IsActive = item.IsActive;
                    queueSubscription.PaymentComplete = item.PaymentComplete;
                    queueClient.SendMessage(JsonConvert.SerializeObject(queueSubscription));
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
