using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using NextNews.Models.Database;
using NextNews.Services;

namespace NextNews.Controllers
{
    
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IEmailSender _emailSender;
        public ContactController(IContactService contactService, IEmailSender emailSender)
        {
                _contactService = contactService;
            _emailSender = emailSender;
        }






        [HttpGet]
        public ActionResult Index()
        {
            

            return View();
        }


        [HttpPost]
        public ActionResult SaveContactFormMessage(ContactFormMessage obj)
        {

            if (ModelState.IsValid)
            {
                // Process the form data (e.g., send an email, save to a database).
                _contactService.SaveContactMessage(obj);

                // For this example, let's just return a view with the submitted data.
                string htmlTemplate = System.IO.File.ReadAllText("C:\\Users\\zain\\source\\repos\\NextNews\\NextNews\\Views\\Shared\\EmailTemplate.cshtml");
                string personalizedContent = $"Thank you for giving us feedback.We will contanct you soon. The NextNews takes your data privacy seriously. To learn more, read our privacy policy and account FAQs.All the best, Regards NextNews";
                string htmlMessage = htmlTemplate.Replace("{{main_content}}", personalizedContent);
                _emailSender.SendEmailAsync(obj.Email, "NextNews Subscription", htmlMessage);
             
                return View("ThankYouMessage");
            }
            else
            {
                // If the model state is not valid, return the form with errors.
                ViewBag.ErrorMessage = "Have all necesary fields information added?";
                return View("SaveContactFormMessage", obj);
            }
        }

    }
}