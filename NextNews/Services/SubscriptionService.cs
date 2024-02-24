using Azure.Core;
using MailKit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NextNews.Data;
using NextNews.Models.Database;
using NextNews.ViewModels;
using Org.BouncyCastle.Bcpg;
using Stripe;
using Stripe.Checkout;
using Subscription = NextNews.Models.Database.Subscription;

namespace NextNews.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _context;
        public SubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Subscription> GetSubscriptionsAsync()
        {
            var subscriberList= _context.Subscriptions.Include(s => s.SubscriptionType).ToList();
            return subscriberList;
        }

        public async Task<List<Subscription>> GetUserSubscriptionsAsync(string userId)
        {
            return await _context.Subscriptions.Where(s => s.UserId == userId).ToListAsync();
        }
        //create 
        public async Task CreateSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }
        //details subscription
        public async Task<Subscription> GetSubscriptionByIdAsync(int id)
        {
            return await _context.Subscriptions.FindAsync(id);
        }
        

        //Update subscription
        public async Task UpdateSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

        //delete Subscription

        public async Task DeleteSubscriptionAsync(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }


        //

        public SubscriptionType GetSubscriptionType(int subscriptionTypeId)
        {
            return _context.SubscriptionTypes.FirstOrDefault(st => st.Id == subscriptionTypeId);
        }



        //Get Subscription Types
        public async Task<List<SubscriptionType>> GetSubscriptionTypesAsync()
        {
            return await _context.SubscriptionTypes.ToListAsync();
        }


        //Create Subscription Types
        public async Task CreateSubscriptionTypesAsync(SubscriptionType subscriptionType)
        {
            _context.SubscriptionTypes.Add(subscriptionType);
            await _context.SaveChangesAsync();
        }
        //Details SubscriptionType 
        public async Task<SubscriptionType> GetSubscriptionTypeByIdAsync(int id)
        {
            return await _context.SubscriptionTypes.FindAsync(id);
        }

        //Update/edit Subscription Type

        public async Task UpdateSubscriptionTypeAsync(SubscriptionType subscriptionType)
        {
            _context.SubscriptionTypes.Update(subscriptionType);
            await _context.SaveChangesAsync();
        }


        //Delete
        public async Task DeleteSubscriptionType(int id)
        {
            var subscriptionType = await _context.SubscriptionTypes.FindAsync(id);
            if (subscriptionType != null)
            {
                _context.SubscriptionTypes.Remove(subscriptionType);
                await _context.SaveChangesAsync();
            }
        }

        //Create subscription for user
        //public string CreateSubscriptionForUser(string userId, int subscriptionTypeId)
        //{
        //    var subscriptionType = _context.SubscriptionTypes.FirstOrDefault(st => st.Id == subscriptionTypeId);
        //    if (subscriptionType == null)
        //    {
        //        throw new ArgumentException("Invalid Subscription Type ID");
        //    }
        //    var existingSubscription = _context.Subscriptions.FirstOrDefault(x => x.UserId == userId && x.SubscriptionTypeId == subscriptionTypeId);
        //    if (existingSubscription != null)
        //    {

        //        return "You have already bought selected plan";
        //    }
        //    var subscription = new Subscription()
        //    {

        //        UserId = userId,
        //        SubscriptionTypeId = subscriptionTypeId,
        //        Price = subscriptionType.Price,
        //        Created = DateTime.Now,
        //        Expired = DateTime.Now.AddMonths(1),
        //        PaymentComplete = "No"
        //    };
        //    _context.Subscriptions.Add(subscription);
        //    _context.SaveChanges();
        //    return "You have Successfully Subscribed";
        //}

        public string CheckExistingSubscription(string userId, int subscriptionTypeId)
        {
            var existingSubscription = _context.Subscriptions.FirstOrDefault(x => x.UserId == userId && x.SubscriptionTypeId == subscriptionTypeId && x.IsActive==true);
            if (existingSubscription != null)
            {
                return "You have already bought selected plan";
            }
            return "Eligible for subscription";
        }
        public void CompleteSubscription(string userId, int subscriptionTypeId)
        {
            var subscriptionType = _context.SubscriptionTypes.FirstOrDefault(st => st.Id == subscriptionTypeId);
            if (subscriptionType == null)
            {
                throw new ArgumentException("Invalid Subscription Type ID");
            }
            var subscription = new Subscription()
            {

                UserId = userId,
                SubscriptionTypeId = subscriptionTypeId,
                Price = subscriptionType.Price,
                Created = DateTime.Now,
                Expired = DateTime.Now.AddMonths(1),
                PaymentComplete = true, // Indicating payment is complete
                IsActive = true
            };
            _context.Subscriptions.Add(subscription);
            _context.SaveChanges();
        }



        public async Task<int> CountBasicSubscribersAsync()
        {
            return await _context.Subscriptions
                .Where(subscription => subscription.SubscriptionType.Name == "Basic")
                .CountAsync();
        }

        public async Task<int> CountPremiumSubscribersAsync()
        {
            return await _context.Subscriptions
                .Where(subscription => subscription.SubscriptionType.Name == "Premium")
                .CountAsync();
        }

        ////Here starts method to change subscription 
        public void UpgradeSubscription(string userId, int newSubscriptionTypeId)
        {
            // Retrieve the current active subscription
            var currentSubscription = _context.Subscriptions
                                              .FirstOrDefault(s => s.UserId == userId && s.IsActive);

            if (currentSubscription == null)
            {
                throw new ArgumentException("No active subscription found for the user.");
            }

            // Retrieve the new subscription type
            var newSubscriptionType = _context.SubscriptionTypes
                                              .FirstOrDefault(st => st.Id == newSubscriptionTypeId);

            if (newSubscriptionType == null)
            {
                throw new ArgumentException("Invalid new Subscription Type ID");
            }

            // Deactivate the current subscription
            currentSubscription.IsActive = false;
            _context.SaveChanges();
        }

        //public List<Subscription> SubscriberExpiredSoon()
        //{
        //    var currentDate = DateTime.Now;
        //    var upcomingExpireDate = currentDate.AddDays(5);
        //    var soonExpirySubscription = _context.Subscriptions.Where(
        //        s => s.Expired > currentDate && s.Expired <= upcomingExpireDate).ToList();
        //    return soonExpirySubscription;
        //}
        public List<SubscriptionWithUserEmailVM> SubscriberExpiredSoon()
        {
            var currentDate = DateTime.Now;
            var upcomingExpireDate = currentDate.AddDays(5);
            var soonExpirySubscription = _context.Subscriptions
                .Where(s => s.Expired > currentDate && s.Expired <= upcomingExpireDate)
                .Join(_context.Users,
                    subscription => subscription.UserId,
            user => user.Id,
                    (subscription, user) => new SubscriptionWithUserEmailVM { UserEmail = user.Email, Subscription = subscription })
                .ToList();

            return soonExpirySubscription;
        }

        public bool HasSubscription(string userId, string subscriptionName)
        {
            return _context.Subscriptions.Any(s => s.SubscriptionType!.Name == subscriptionName && s.UserId == userId && s.IsActive == true);
        }

        


    }
}

