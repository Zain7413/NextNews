using Microsoft.EntityFrameworkCore;
using NextNews.Models.Database;
using NextNews.ViewModels;

namespace NextNews.Services
{
    public interface ISubscriptionService
    {
        public List<Subscription> GetSubscriptionsAsync();
        //public Task<List<Subscription>> GetSubscriptionsAsync();
        public Task CreateSubscriptionAsync(Subscription subscription);
        public Task<Subscription> GetSubscriptionByIdAsync(int id);
        public Task UpdateSubscriptionAsync(Subscription subscription);
        public Task DeleteSubscriptionAsync(int id);
        public Task<List<SubscriptionType>> GetSubscriptionTypesAsync();
        public Task CreateSubscriptionTypesAsync(SubscriptionType subscriptionType);
        //public string CreateSubscriptionForUser(string userId, int subscriptionTypeId);
        public Task<SubscriptionType> GetSubscriptionTypeByIdAsync(int id);
        public Task UpdateSubscriptionTypeAsync(SubscriptionType subscriptionType);
        public SubscriptionType GetSubscriptionType(int subscriptionTypeId);
        public string CheckExistingSubscription(string userId, int subscriptionTypeId);
        public void CompleteSubscription(string userId, int subscriptionTypeId);
        public Task DeleteSubscriptionType(int id);
        public Task<List<Subscription>> GetUserSubscriptionsAsync(string userId);

        //count of Subscribers type
        public Task<int> CountBasicSubscribersAsync();


        public Task<int> CountPremiumSubscribersAsync();

        public void UpgradeSubscription(string userId, int newSubscriptionTypeId);
        //public List<Subscription> SubscriberExpiredSoon();
        public List<SubscriptionWithUserEmailVM> SubscriberExpiredSoon();
        bool HasSubscription(string userId, string subscriptionName);

    }
}
