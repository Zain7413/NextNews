using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using NextNews.Models.Database;
using NextNews.Services;

namespace NextNews.Controllers
{
    public class NewsLetterController : Controller
    {
        private readonly INewsLetterService _newsLetterService;
        private readonly IEmailSender _emailSender;
        public NewsLetterController(INewsLetterService newsLetterService, IEmailSender emailSender) 
        { 
        _newsLetterService = newsLetterService;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(NewsLetterSubscriber newsLetterSubscriber) 
        {
            var existingSubscriber = _newsLetterService.GetNewsLetterSubscribers().FirstOrDefault(x=>x.Email==newsLetterSubscriber.Email); ;
            if (existingSubscriber != null)
            {
                ViewBag.Message = "You have already subscribed for the newsletter";
                return RedirectToAction("index", "Home");
            }
            else
            {
                _newsLetterService.CreateNLSubscriber(newsLetterSubscriber);
                string htmlTemplate = System.IO.File.ReadAllText("C:\\Users\\zain\\source\\repos\\NextNews\\NextNews\\Views\\Shared\\EmailTemplate.cshtml");
                string personalizedContent = $"Congrats! You have successfully subscribe to our weekly news letter. You can use your account to sign into the NextNews website and use special features. Subscriptions to the suite of NextNews newsletters can also be managed using your account. The NextNews takes your data privacy seriously. To learn more, read our privacy policy and account FAQs.All the best, Regards NextNews";
                string htmlMessage = htmlTemplate.Replace("{{main_content}}", personalizedContent);
                _emailSender.SendEmailAsync(newsLetterSubscriber.Email, "NextNews Subscription", htmlMessage);
                return RedirectToAction("Index", "Home"); ;
            }
        }
    }
}
