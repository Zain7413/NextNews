using System;
using System.Text;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using QueueTrggerForNewLetter.Model;

namespace QueueTrggerForNewLetter
{
    public class QTriggerNewsletter
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<QTriggerNewsletter> _logger;

        public QTriggerNewsletter(ILogger<QTriggerNewsletter> logger,IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        [Function(nameof(QTriggerNewsletter))]
        public void Run([QueueTrigger("newsletter", Connection = "AzureWebJobsStorage")]NewsLetterFM  message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.Email}");

            var emailContent = new StringBuilder();
            emailContent.Append("<h1>Weekly Newsletter</h1>");
            foreach (var item in message.News) 
            {
                emailContent.Append($"<h2><a href='{item.ArticleUrl}'>{item.HeadLine}</a></h2>");
                emailContent.Append($"<img src='{item.ImageLink}' alt='Article Image'/>");
                emailContent.Append($"<p>{item.ContentSummary}</p>");
                emailContent.Append($"<a href='{item.ArticleUrl}'>Read more</a><br/><br/>");
            }
            _emailSender.SendEmailAsync(message.Email,"WeeklyNewsLetter", emailContent.ToString());
        }
    }
}
