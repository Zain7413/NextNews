using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NextNews.Services;

namespace TimeTriggerArchiveFunction
{
    public class ArchiveFunction
    {
        private readonly ILogger _logger;
        private readonly IArticleService _articleService;

        public ArchiveFunction(ILoggerFactory loggerFactory, IArticleService articleService)
        {
            _logger = loggerFactory.CreateLogger<ArchiveFunction>();
            _articleService = articleService;
        }

        [Function("ArchiveFunction")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _articleService.ArticlesToArchive();

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
