namespace InvestmentApp.Models
{
    public class ChequingAccount
    {
        public int ChequingAccountId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }

        public User User { get; set; }
    }
}
