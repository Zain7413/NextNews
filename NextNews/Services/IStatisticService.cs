namespace NextNews.Services
{
    public interface IStatisticService
    {


        int GetUserCount();
        int GetArticleCount();
        int GetBasicSubscrptionUsers();

        int GetPremiumSubscriptionUsers();


    }
}
