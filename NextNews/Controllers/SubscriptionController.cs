using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextNews.Models.Database;
using NextNews.Services;
using Stripe.Checkout;
using Subscription = NextNews.Models.Database.Subscription;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Claims;
using Azure.Core;
using NextNews.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using NextNews.Data.Migrations;
using NuGet.Protocol;
using System.IO;
using Microsoft.AspNetCore.Hosting;



namespace NextNews.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        private readonly IWebHostEnvironment _env;
        public SubscriptionController(IWebHostEnvironment env, ISubscriptionService subscriptionService, IUserService userService, UserManager<User> userManager, IEmailSender emailSender)
        {
            _subscriptionService = subscriptionService;
            _userService = userService;
            _userManager = userManager;
            _emailSender = emailSender;
            _env = env;

        }

        public IActionResult Index()
        {

            return View();
        }
       
      
        //Create Subscription For User
        [Authorize]
        public async Task<IActionResult> CreateUserSubscription()
        {
            var subscriptionTypes = await _subscriptionService.GetSubscriptionTypesAsync();
            ViewBag.SubscriptionTypes = subscriptionTypes;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUserSubscription(Subscription input)
        {
            var userId = _userManager.GetUserId(User);
           
            // Check if the user already has this subscription
            string resultMessage = _subscriptionService.CheckExistingSubscription(userId, input.SubscriptionTypeId);
            if (resultMessage != "Eligible for subscription")
            {
                ViewBag.Message = resultMessage;
                return View("Index", "Home");
            }


            // Assign roles based on subscription type
            var subscriptionType = _subscriptionService.GetSubscriptionType(input.SubscriptionTypeId);
            var roleToAssign = subscriptionType.Name == "Basic" ? "Basic" : "Premium";

            // Assign role to user
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.AddToRoleAsync(user, roleToAssign);


            // Add claim based on subscription type
            var claimType = "SubscriptionType";
            var claimValue = subscriptionType.Name;
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var existingSubscriptionClaim = existingClaims.FirstOrDefault(c => c.Type == claimType);
            if (existingSubscriptionClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, existingSubscriptionClaim);
            }
            await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));


           

            // Redirect to Stripe for payment
            return await StripeCheckout(userId, input.SubscriptionTypeId);
        }
        private async Task<IActionResult> StripeCheckout(string userId, int subscriptionTypeId)
        {
            // Retrieve subscription type details
            
            var subscriptionType = _subscriptionService.GetSubscriptionType(subscriptionTypeId);
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            // Create Stripe Checkout session

            var domain = "https://localhost:44349/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(subscriptionType.Price * 100), // Price in cents
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = subscriptionType.Name,
                    },
                },
                Quantity = 1,
            },
        },
                Metadata = new Dictionary<string, string>
                {
                    {"UserId", userId},
                    {"SubscriptionTypeId", subscriptionTypeId.ToString()}
                },
                Mode = "payment",
                CustomerEmail = userEmail,
                SuccessUrl =domain+$"Subscription/CompleteSubscription",
                CancelUrl = "https://yourdomain.com/subscription/cancel"
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            TempData["Session"] = session.Id;
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            //return Redirect(session.Url);
        }


        public IActionResult CompleteSubscription()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());

            if (session.PaymentStatus == "paid")
            {

                var userId = session.Metadata["UserId"];
                var subscriptionTypeId = int.Parse(session.Metadata["SubscriptionTypeId"]);
                _subscriptionService.CompleteSubscription(userId, subscriptionTypeId);


                string templatePath = Path.Combine(_env.ContentRootPath, "Views", "Shared", "EmailTemplate.cshtml");
                string htmlTemplate = System.IO.File.ReadAllText(templatePath);
                /*  string htmlTemplate = System.IO.File.ReadAllText("~/Views/Shared/EmailTemplate.cshtml");*/  //giving error check it

                string personalizedContent = $"Welcome to your NextNews account. You can use your account to sign into the NextNews website and use special features. Subscriptions to the suite of NextNews newsletters can also be managed using your account. The NextNews takes your data privacy seriously. To learn more, read our privacy policy and account FAQs.All the best, Regards NextNews";
                string htmlMessage = htmlTemplate.Replace("{{main_content}}", personalizedContent);

                _emailSender.SendEmailAsync(userEmail, "NextNews Subscription", htmlMessage);





                return View("Success");
            }
            return View("Register");


        }
        public IActionResult Success() 
        
        {
            
         return View();
        }

     

        public IActionResult ListSubscription()
        {
            var subscription =  _subscriptionService.GetSubscriptionsAsync();
            return View(subscription);
        }

        public async Task<IActionResult> Details(int id)
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            return View(subscription);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            return View(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Price, Created, Expired, PaymentComplete, UserId")] Subscription subscription ) 
        {
            if (id != subscription.Id) 
            {
                return NotFound();
            }
            if (ModelState.IsValid) 
            { 
            await _subscriptionService.UpdateSubscriptionAsync(subscription);
                return RedirectToAction("ListSubscription");
            }
            return View(subscription);
        }
        public async Task<IActionResult> Delete(int id) 
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (subscription == null) 
            {
                return NotFound();
            }
            return View(subscription);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id) 
        { 
         await _subscriptionService.DeleteSubscriptionAsync(id);
            return RedirectToAction("ListSubscription");
        }

        //Create Subscriptions Types
        public async Task<IActionResult> CreateTypes() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTypes(SubscriptionType subscriptionType) 
        {
            if (ModelState.IsValid)
            {
               await _subscriptionService.CreateSubscriptionTypesAsync(subscriptionType);
                return RedirectToAction("Index");
            }
            else
                return View ("CreateTypes","subscriptionType");
        }
        //Details of Subscription types
        public async Task<IActionResult> TypesDetails(int id) 
        { 
        var subscriptionType= await _subscriptionService.GetSubscriptionTypeByIdAsync(id);
            return View(subscriptionType);
        }

        //List of Types of Subscription
        public async Task<IActionResult> SubscriptionTypeList() 
        {
            var subscriptionType = await _subscriptionService.GetSubscriptionTypesAsync();
            return View(subscriptionType);
        }

        //Edit Subscription Types
        public async Task<IActionResult> TypesEdit(int id) 
        {
        
            var subscriptionType = await _subscriptionService.GetSubscriptionTypeByIdAsync(id);
            if (subscriptionType == null) 
            {
                return NotFound();
            }
            
            return View(subscriptionType);
        }
        [HttpPost]
        public async Task<IActionResult> TypesEdit(int id,[Bind("Id, Name, Description, Price")] SubscriptionType subscriptionType) 
        {
            if (subscriptionType.Id != id) 
            {
                return NotFound();
            }

            if (ModelState.IsValid) 
            { 
            await _subscriptionService.UpdateSubscriptionTypeAsync(subscriptionType);
                return RedirectToAction("SubscriptionTypeList");
            }
            return View(subscriptionType);
        }

        // Delete Subscription Type
        public async Task<IActionResult> TypesDelete(int id) 
        {
            var subscriptionType = await _subscriptionService.GetSubscriptionTypeByIdAsync(id);
            if (subscriptionType == null) 
            {
                return NotFound();
            }
            return View(subscriptionType);
        }
        [HttpPost, ActionName("TypesDelete")]
        public async Task<IActionResult> TypesDeleteConfirm(int id) 
        { 
        await _subscriptionService.DeleteSubscriptionType(id);
            return RedirectToAction("SubscriptionTypeList");
        }

        //here start changing subscription

       
        public async Task<IActionResult> ChangeSubscription(int newSubscriptionTypeId)
        {
            var userId = _userManager.GetUserId(User);
            _subscriptionService.UpgradeSubscription(userId, newSubscriptionTypeId);

            // Redirect to a confirmation page or display a success message
           
            return await StripeCheckout(userId, newSubscriptionTypeId);
        }

        public async Task<IActionResult> SubscriptionHistory() 
        {
            var userId = _userManager.GetUserId(User); // Assuming you are using ASP.NET Core Identity
            var subscriptions = await _subscriptionService.GetUserSubscriptionsAsync(userId);
            List<SubscriptionHistoryViewModel> vmList = new List<SubscriptionHistoryViewModel>();
            foreach (var item in subscriptions) 
            {
                var vm = new SubscriptionHistoryViewModel()
                {
                    SubscriptionId= item.SubscriptionTypeId,
                  
                    Created =item.Created,
                    Expired = item.Expired,
                    Price = item.Price,
                    IsActive = item.IsActive,
                };
                vmList.Add(vm);
            }
            return View(vmList);
        }














        //[Authorize(Roles = "Basic")]

        //[Authorize(Policy = "BasicWeatherAccess")]
        //public IActionResult BasicContent()
        //{
        //    // Get the categories the user has access to (e.g., from claims or database)
        //    var userCategories = ((ClaimsIdentity)User.Identity).Claims
        //        .Where(c => c.Type == ClaimTypes.Role)
        //        .Select(c => c.Value)
        //        .ToList();

        //    // Check if the user has access to the "Weather" category
        //    if (userCategories.Contains("Weather"))
        //    {
        //        // User has access to the "Weather" category, return the view
        //        return View();
        //    }
        //    else
        //    {
        //        // User does not have access to the "Weather" category, return unauthorized view or redirect
        //        return View("Unauthorized");
        //    }
        //}



        [Authorize(Policy = "BasicWeatherAccess")]
        public IActionResult BasicContent()
        {
            // Only users with the "Basic" role and the "WeatherCategory" claim set to "Weather" will reach here
            return View();
        }





        [Authorize(Roles = "Premium")]
        public IActionResult PremiumContent()
        {
            return View();
        }




    }
}
