using System;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NextNews.Models.Database;
using NextNews.ViewModels;

namespace QueueTriggerEmail
{
    public class EmailMessage
    {
        private readonly ILogger<EmailMessage> _logger;
        private readonly IEmailSender _emailSender;
        public EmailMessage(ILogger<EmailMessage> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        [Function(nameof(EmailMessage))]
        public void Run([QueueTrigger("expiredsubsemail", Connection = "AzureWebJobsStorage")] SubscriptionWithUserEmailVM message)
        {
            _logger.LogInformation($"C# Queue trigger function processed");
            string htmlMessage = $"Your subscription is going to expired in 5 days";
            _emailSender.SendEmailAsync(message.UserEmail, "Subscription Expired Soon", htmlMessage);

        }
    }
}
