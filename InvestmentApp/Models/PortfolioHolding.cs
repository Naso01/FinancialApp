namespace InvestmentApp.Models
{
    public class PortfolioHolding
    {
        public int PortfolioHoldingId { get; set; }
        public int PortfolioId { get; set; }
        public int StockId { get; set; }

        public decimal Quantity { get; set; }
        public decimal AvgPrice { get; set; }

        public DateTime PurchaseDate { get; set; }

        public Portfolio Portfolio { get; set; }
        public Stock Stock { get; set; }
    }
}
