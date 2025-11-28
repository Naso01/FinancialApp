namespace InvestmentApp.Models
{
    public enum PortfolioType
    {
        Managed = 1,
        SelfDirected = 2
    }

    public class Portfolio
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public PortfolioType PortfolioType { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public List<PortfolioHolding> Holdings { get; set; }
    }
}
