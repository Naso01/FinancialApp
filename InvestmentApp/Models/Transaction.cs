namespace InvestmentApp.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int? PortfolioId { get; set; }
        public int? StockId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // buy, sell, etc
        public DateTime CreatedAt { get; set; }
    }

}
