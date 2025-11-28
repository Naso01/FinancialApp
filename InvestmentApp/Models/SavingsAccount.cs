namespace InvestmentApp.Models
{
    public class SavingsAccount
    {
        public int SavingsAccountId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }

        public Account Account { get; set; }
    }
}
