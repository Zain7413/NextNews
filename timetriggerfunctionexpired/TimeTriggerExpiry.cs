using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NextNews.Services;

namespace timetriggerfunctionexpired
{
    public class TimeTriggerExpiry
    {
        private readonly ILogger _logger;
        private readonly IArticleService _articleService;
        private readonly IConfiguration _configuration;
        public TimeTriggerExpiry(ILoggerFactory loggerFactory, IArticleService articleService, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<TimeTriggerExpiry>();
            _articleService = articleService;
            _configuration = configuration;
        }

        [Function("TimeTriggerExpiry")]
        public void Run([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _articleService.CheckExpiredSubs();
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
