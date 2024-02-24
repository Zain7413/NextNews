using System;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NextNews.Models.Database;
using NextNews.Services;
using NextNews.ViewModels;

namespace TimeTriggerLetterToUser
{
    public class TimeTrigger
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public TimeTrigger(ILoggerFactory loggerFactory, ISubscriptionService subscriptionService, IConfiguration configuration, IUserService userService)
        {
            _logger = loggerFactory.CreateLogger<TimeTrigger>();
            _subscriptionService = subscriptionService;
            _configuration = configuration;
            _userService = userService;
        }

        [Function("TimeTrigger")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var connectionString = _configuration["AzureWebJobsStorage"];
            var queueString = "expiredsubsemail";

            QueueClient queueClient = new QueueClient(connectionString, queueString, new
                QueueClientOptions {MessageEncoding = QueueMessageEncoding.Base64 }
            );

            List<SubscriptionWithUserEmailVM> expiringSubscriber = _subscriptionService.SubscriberExpiredSoon();
            foreach (var item in expiringSubscriber)
            {
                queueClient.CreateIfNotExists();
                try
                {
                    SubscriptionWithUserEmailVM queueSubscriber = new SubscriptionWithUserEmailVM();
                    queueSubscriber.Subscription = item.Subscription;
                    queueSubscriber.UserEmail = item.UserEmail;
                   
                    queueClient.SendMessage(JsonConvert.SerializeObject(queueSubscriber));
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
