using System.ComponentModel.DataAnnotations;

namespace NextNews.Models.Database
{
    public class Subscription
    {
        public int Id { get; set; }


        public int SubscriptionTypeId { get; set; }
        public virtual SubscriptionType? SubscriptionType { get; set; }

        public decimal? Price { get; set; }

        public DateTime? Created { get; set;}

        public DateTime? Expired { get; set;}

        public bool? PaymentComplete { get; set;}

        public string? UserId { get; set;}

        //Foreign Key
        public virtual User? User { get; set; }
        public bool IsActive{get; set;}
    }
}
