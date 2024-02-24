using NextNews.Models.Database;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextNews.ViewModels
{
    public class SubscriptionWithUserEmailVM
    {
        public Subscription? Subscription { get; set; }
        public string? UserEmail { get; set; }
    }
}
