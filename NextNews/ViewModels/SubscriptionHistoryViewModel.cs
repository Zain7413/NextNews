namespace NextNews.ViewModels
{
    public class SubscriptionHistoryViewModel
    {
        public string Name  { get; set; }
        public int SubscriptionId { get; set; }
        public string SubscriptionTypeName { get; set; }
        public DateTime? SubscriptionHistory { get; set; }
        public decimal? Price { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Expired { get; set; }
        public bool IsActive { get; set; }
    }
}
