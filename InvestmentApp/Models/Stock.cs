namespace InvestmentApp.Models
{
    //Keegan Erdis
    public class Stock
    {
        public int StockId { get; set; }
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public DateTime LastUpdated { get; set; }

        public decimal Price { get; set; }

        public List<PortfolioHolding> PortfolioHoldings { get; set; }
    }
}
