namespace InvestmentApp.Models
{
    public enum RecommendationAction
    {
        Buy = 1,
        Sell = 2,
        Hold = 3
    }

    public class AdminRecommendation
    {
        public int RecommendationId { get; set; }
        public int UserId { get; set; }
        public int StockId { get; set; }

        public RecommendationAction Action { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Stock Stock { get; set; }
    }
}
